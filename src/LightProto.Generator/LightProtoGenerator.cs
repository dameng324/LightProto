using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace LightProto.Generator;

[Generator]
public class LightProtoGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        var processedTypes = new HashSet<string>();

        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);

            foreach (var node in syntaxTree.GetRoot().DescendantNodesAndSelf())
            {
                INamedTypeSymbol? targetType;

                TypeDeclarationSyntax targetTypeSyntax;
                // Support class, record, record struct, and struct declarations
                if (node is ClassDeclarationSyntax classDeclaration)
                {
                    targetType =
                        ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclaration)
                        as INamedTypeSymbol;
                    targetTypeSyntax = classDeclaration;
                }
                else if (node is StructDeclarationSyntax structDeclaration)
                {
                    targetType =
                        ModelExtensions.GetDeclaredSymbol(semanticModel, structDeclaration)
                        as INamedTypeSymbol;
                    targetTypeSyntax = structDeclaration;
                }
                else if (node is RecordDeclarationSyntax recordDeclaration)
                {
                    targetType =
                        ModelExtensions.GetDeclaredSymbol(semanticModel, recordDeclaration)
                        as INamedTypeSymbol;
                    targetTypeSyntax = recordDeclaration;
                }
                else
                {
                    // // Handle records using reflection for compatibility
                    // var nodeTypeName = node.GetType().Name;
                    // if (nodeTypeName.Contains("RecordDeclaration"))
                    // {
                    //     var symbolInfo = semanticModel.GetDeclaredSymbol(node);
                    //     targetType = symbolInfo as INamedTypeSymbol;
                    // }
                    // else
                    {
                        continue;
                    }
                }

                if (targetType is null)
                    continue;

                // Prevent duplicate processing
                var typeKey =
                    $"{targetType}@{targetType.Locations.FirstOrDefault()?.SourceTree?.FilePath}";
                if (!processedTypes.Add(typeKey))
                    continue;

                // Look for ProtoContract attribute
                var protoContractAttribute = targetType
                    .GetAttributes()
                    .FirstOrDefault(attr =>
                        attr.AttributeClass?.ToDisplayString()
                        == "LightProto.ProtoContractAttribute"
                    );

                if (protoContractAttribute is null)
                    continue;

                try
                {
                    // Generate the basic IMessage implementation
                    var sourceCode = GenerateBasicProtobufMessage(
                        context.Compilation,
                        targetType,
                        targetTypeSyntax,
                        protoContractAttribute
                    );
                    var fileName = $"{targetType}.g.cs";
                    context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
                }
                catch (LightProtoGeneratorException e)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor(
                                e.Id,
                                e.Title,
                                e.Message,
                                e.Category,
                                e.Severity,
                                isEnabledByDefault: true
                            ),
                            e.Location ?? Location.None
                        )
                    );
                }
            }
        }
    }

    private string GenerateBasicProtobufMessage(
        Compilation compilation,
        INamedTypeSymbol targetType,
        TypeDeclarationSyntax typeDeclaration,
        AttributeData protoContractAttribute
    )
    {
        var namespaceName = targetType.ContainingNamespace.ToDisplayString();
        var className = targetType.Name;

        var typeDeclarationString = targetType.IsValueType
            ? targetType.IsRecord
                ? "partial record struct"
                : "partial struct"
            : targetType.IsRecord
                ? "partial record"
                : "partial class";

        var proxyFor = GetProxyFor(targetType.GetAttributes());
        bool skipConstructor =
            protoContractAttribute
                .NamedArguments.FirstOrDefault(arg => arg.Key == "SkipConstructor")
                .Value.Value?.ToString() == "True";

        var sourceBuilder = new StringBuilder();

        sourceBuilder.AppendLine(
            $$"""
              // <auto-generated>
              //     Generated by {{nameof(
                  LightProtoGenerator
              )}} at {{DateTime.Now:yyyy-MM-dd HH:mm:ss}}
              // </auto-generated>

              #pragma warning disable 1591, 0612, 3021, 8981, CS9035, CS0109,CS8669
              using System;
              using System.Linq;
              using LightProto;
              using LightProto.Parser;
              namespace {{namespaceName}};
              """
        );

        var protoMembers = GetProtoMembers(compilation, targetType, typeDeclaration);

        var classBody = (
            $$"""
              [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
              {{typeDeclarationString}} {{className}} :{{(proxyFor is null ?$"IProtoMessage<{className}>":$"IProtoParser<{proxyFor.ToDisplayString()}>")}}
              {
                  public static IProtoReader<{{proxyFor?.ToDisplayString()??className}}> Reader {get; } = new ProtoReader();
                  public static IProtoWriter<{{proxyFor?.ToDisplayString()??className}}> Writer {get; } = new ProtoWriter();
                  public sealed new class ProtoWriter:IProtoWriter<{{proxyFor?.ToDisplayString()??className}}>
                  {
                      public bool IsMessage => true;
                      public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
                      {{string.Join(Environment.NewLine + GetIntendedSpace(2),
                          protoMembers.SelectMany(member =>
                          {
                              if (TryGetInternalTypeName(member.Type, member.DataFormat,member.StringIntern, out _)==false)
                              {
                                  return GetProtoParserMember(compilation, member, "Writer", targetType);
                              }
                              else
                              {
                                  return [];
                              }
                          }))
                      }}
                      public void WriteTo(ref WriterContext output, {{proxyFor?.ToDisplayString()??className}} value)
                      {
                          {{className}} message = value;
                          {{string.Join(Environment.NewLine + GetIntendedSpace(3),
                              protoMembers.SelectMany(member => {
                                  return Gen();

                                  IEnumerable<string> Gen()
                                  {
                                      var checkIfNotEmpty = GetCheckIfNotEmpty(member,"message");

                                      if (IsCollectionType(compilation, member.Type) || IsDictionaryType(compilation, member.Type))
                                      {
                                          yield return $"if({checkIfNotEmpty})";
                                          yield return $"{{";
                                          yield return $"    {member.Name}_ProtoWriter.WriteTo(ref output, message.{member.Name});";
                                          yield return $"}} ";
                                      }
                                      else if (TryGetInternalTypeName(member.Type, member.DataFormat,member.StringIntern, out var name))
                                      {
                                          yield return $"if({checkIfNotEmpty})";
                                          yield return $"{{";
                                          yield return $"    output.WriteTag({member.RawTag}); ";
                                          yield return $"    output.Write{name}(message.{member.Name});";
                                          yield return $"}}";
                                      }
                                      else
                                      {
                                          yield return $"if({checkIfNotEmpty})";
                                          yield return $"{{";
                                          yield return $"    output.WriteTag({member.RawTag}); ";
                                          yield return $"    {member.Name}_ProtoWriter.WriteMessageTo(ref output, message.{member.Name});";
                                          yield return $"}}";
                                      }
                                  }
                              }))
                          }}
                      }
                      
                      public int CalculateSize({{proxyFor?.ToDisplayString()??className}} value) {
                          {{className}} message = value;
                          int size=0;
                          {{string.Join(Environment.NewLine + GetIntendedSpace(3),
                              protoMembers.SelectMany(member => {
                                  return Gen();

                                  IEnumerable<string> Gen()
                                  {
                                      var tagSize = member.RawTagBytes.Length;
                                      var checkIfNotEmpty = GetCheckIfNotEmpty(member,"message");

                                      if (IsCollectionType(compilation, member.Type) || IsDictionaryType(compilation, member.Type))
                                      {
                                          yield return $"if(message.{member.Name}!=null)";
                                          yield return $"    size += {member.Name}_ProtoWriter.CalculateSize(message.{member.Name}); ";
                                      }
                                      else if (TryGetInternalTypeName(member.Type, member.DataFormat,member.StringIntern, out var name))
                                      {
                                          yield return $"if({checkIfNotEmpty})";
                                          yield return $"    size += {tagSize} + CodedOutputStream.Compute{name}Size(message.{member.Name});";
                                      }
                                      else
                                      {
                                          yield return $"if({checkIfNotEmpty})";
                                          yield return $"    size += {tagSize} + {member.Name}_ProtoWriter.CalculateMessageSize(message.{member.Name});";
                                      }
                                  }
                              }))
                          }}
                          return size;
                      }
                  }
                  
                  public sealed new class ProtoReader:IProtoReader<{{proxyFor?.ToDisplayString()??className}}>
                  {
                      public bool IsMessage => true;
                      public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
                      {{string.Join(Environment.NewLine + GetIntendedSpace(2),
                          protoMembers.SelectMany(member => {
                              if (TryGetInternalTypeName(member.Type, member.DataFormat,member.StringIntern, out _)==false)
                              {
                                  return GetProtoParserMember(compilation, member, "Reader", targetType);
                              }
                              else
                              {
                                  return [];
                              }
                          }))
                      }}
                      public {{proxyFor?.ToDisplayString()??className}} ParseFrom(ref ReaderContext input)
                      {
                          {{string.Join(Environment.NewLine + GetIntendedSpace(3),
                              protoMembers.Select(member => $"{member.Type} _{member.Name} = {member.Initializer};"))
                          }}
                          uint tag;
                          while ((tag = input.ReadTag()) != 0) 
                          {
                              if ((tag & 7) == 4) {
                                break;
                              }
                              switch(tag) 
                              {
                                  default:
                                  break;
                                  {{string.Join(Environment.NewLine + GetIntendedSpace(5), protoMembers.SelectMany(member =>
                                      {
                                          return Gen();

                                          IEnumerable<string> Gen()
                                          {
                                              yield return $"case {member.RawTag}:";
                                              if (IsCollectionType(compilation, member.Type))
                                              {
                                                  var elementType = GetElementType(compilation, member.Type);
                                                  var tag2 = ProtoMember.GetRawTag(member.FieldNumber, ProtoMember.GetPbWireType(compilation, elementType, member.DataFormat));
                                                  if (tag2 != member.RawTag)
                                                  {
                                                      yield return $"case {tag2}:";
                                                  }
                                              }

                                              if (TryGetInternalTypeName(member.Type, member.DataFormat,member.StringIntern, out var name))
                                              {
                                                  yield return $"{{";
                                                  yield return $"    _{member.Name} = input.Read{name}();";
                                                  yield return $"    break;";
                                                  yield return $"}}";
                                              }
                                              else if (IsCollectionType(compilation, member.Type)||IsDictionaryType(compilation, member.Type))
                                              {
                                                  yield return $"{{";
                                                  yield return $"    _{member.Name} = {member.Name}_ProtoReader.ParseFrom(ref input);";
                                                  yield return $"    break;";
                                                  yield return $"}}";
                                              }
                                              else
                                              {
                                                  yield return $"{{";
                                                  yield return $"    _{member.Name} = {member.Name}_ProtoReader.ParseMessageFrom(ref input);";
                                                  yield return $"    break;";
                                                  yield return $"}}";
                                              }
                                          }
                                      }))
                                  }}
                              }
                          }
                          {{
                              (skipConstructor
                                  ?GenSkipConstructor()
                                  :GenGeneralConstructor())
                          }}
                          {{string.Join(Environment.NewLine + GetIntendedSpace(3),
                              protoMembers.SelectMany(member => {
                                  return Gen();
                                  IEnumerable<string> Gen()
                                  {   
                                      if (member.IsReadOnly && (IsCollectionType(compilation, member.Type)||IsDictionaryType(compilation, member.Type)))
                                      {
                                          yield return $"if(parsed.{member.Name}!=null) {{";
                                          yield return $"    parsed.{member.Name}.Clear();";
                                          yield return $"    if(_{member.Name}!=null) {{";
                                          if(IsStackType(member.Type))
                                              yield return $"        foreach(var v in _{member.Name}.Reverse()) {{";
                                          else 
                                              yield return $"        foreach(var v in _{member.Name}) {{";
                                          if(IsStackType(member.Type))
                                              yield return $"            parsed.{member.Name}.Push(v);";
                                          else if(IsQueueType(member.Type))
                                              yield return $"            parsed.{member.Name}.Enqueue(v);";
                                          else if(IsDictionaryType(compilation, member.Type))
                                              yield return $"            parsed.{member.Name}[v.Key]=v.Value;";
                                          else 
                                              yield return $"            parsed.{member.Name}.Add(v);";
                                          yield return $"        }}";
                                          yield return $"    }}";
                                          yield return $"}}";
                                      }
                                  }
                              }).Where(x=>string.IsNullOrWhiteSpace(x)==false))
                          }}
                          return parsed;
                      }
                  }
              }
              """
        );
        string GenSkipConstructor()
        {
            return $$"""
                     var parsed = ({{className}})System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof({{className}}));
                     {{string.Join(Environment.NewLine + GetIntendedSpace(4),
                         protoMembers.Select(member => {
                             if (member.IsReadOnly && (IsCollectionType(compilation, member.Type)||IsDictionaryType(compilation, member.Type)))
                             {
                                 throw new LightProtoGeneratorException("Member should not be readonly when SkipConstructor as we can't assign a value to it") { Id = "LIGHT_PROTO_002", Title = $"{member.Name} is readonly", Category = "Usage", Severity = DiagnosticSeverity.Error, Location = member.DelclarationSyntax.GetLocation() };
                             }
                             else if (member.IsInitOnly)
                             {
                                 throw new LightProtoGeneratorException("Member should not be initonly when SkipConstructor as we can't assign a value to it") { Id = "LIGHT_PROTO_001", Title = $"{member.Name} is InitOnly", Category = "Usage", Severity = DiagnosticSeverity.Error, Location = member.DelclarationSyntax.GetLocation() };
                             }
                             else
                             {
                                 return $"parsed.{member.Name} = _{member.Name};";
                             }
                         }))
                     }}
                     """;
        }

        string GenGeneralConstructor()
        {
            return $$"""
                 var parsed = new {{className}}()
                 {
                     {{string.Join(Environment.NewLine + GetIntendedSpace(4),
                         protoMembers.Select(member => {
                             if (member.IsReadOnly && (IsCollectionType(compilation, member.Type)||IsDictionaryType(compilation, member.Type)))
                             {
                                 return $"// {member.Name} is readonly";
                             }
                             else
                             {
                                 return $"{member.Name} = _{member.Name},";
                             }
                         }))
                     }}
                 };
                 """;
        }
        var nestedClassStructure = GenerateNestedClassStructure(targetType, classBody);
        sourceBuilder.AppendLine(nestedClassStructure);
        return sourceBuilder.ToString();
    }

    private static string GenerateNestedClassStructure(
        INamedTypeSymbol targetType,
        string classMemberBody
    )
    {
        // Build the list of containing classes from outermost to innermost
        var containers = new List<INamedTypeSymbol>();
        var current = targetType.ContainingType;
        while (current is not null)
        {
            containers.Add(current);
            current = current.ContainingType;
        }

        // Reverse to get from outermost to innermost
        containers.Reverse();

        // Wrap in container classes
        var result = classMemberBody;
        for (int i = containers.Count - 1; i >= 0; i--)
        {
            var container = containers[i];
            var containerAccessibility =
                container.DeclaredAccessibility is Accessibility.Public ? "public" : "internal";
            var indent = new string(' ', (containers.Count - 1 - i) * 4);

            // Add indentation to current result
            var indentedResult = string.Join(
                "\n",
                result
                    .Split('\n')
                    .Select(line => string.IsNullOrWhiteSpace(line) ? line : "    " + line)
            );

            result = $$"""
                {{indent}}{{containerAccessibility}} partial class {{container.Name}}
                {{indent}}{
                {{indentedResult}}
                {{indent}}}
                """;
        }

        return result;
    }

    private bool TryGetInternalTypeName(
        ITypeSymbol memberType,
        DataFormat format,
        bool stringIntern,
        out string name
    )
    {
        name = memberType.SpecialType switch
        {
            SpecialType.System_Boolean => "Bool",
            SpecialType.System_Int32 => format == DataFormat.FixedSize ? "SFixed32"
            : format == DataFormat.ZigZag ? "SInt32"
            : "Int32",
            SpecialType.System_UInt32 => format == DataFormat.FixedSize ? "Fixed32" : "UInt32",
            SpecialType.System_Int64 => format == DataFormat.FixedSize ? "SFixed64"
            : format == DataFormat.ZigZag ? "SInt64"
            : "Int64",
            SpecialType.System_UInt64 => format == DataFormat.FixedSize ? "Fixed64" : "UInt64",
            SpecialType.System_Single => "Float",
            SpecialType.System_Double => "Double",
            SpecialType.System_String when stringIntern is false => "String",
            _ => "",
        };
        return string.IsNullOrWhiteSpace(name) == false;
    }

    private string GetCheckIfNotEmpty(ProtoMember member, string messageName)
    {
        if (member.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return $"{messageName}.{member.Name}.HasValue";
        }

        if (member.Type.SpecialType == SpecialType.System_DateTime)
        {
            return "true";
        }

        if (
            member.CompatibilityLevel >= CompatibilityLevel.Level300
            && (IsGuidType(member.Type) || IsDecimalType(member.Type))
        )
        {
            return "true";
        }

        if (member.Type.IsValueType)
        {
            return $"{messageName}.{member.Name} != default";
        }

        var check = $"{messageName}.{member.Name} != null";

        if (HasCountProperty(member.Type))
        {
            return $"{check} && {messageName}.{member.Name}.Count > 0";
        }
        if (HasLengthProperty(member.Type))
        {
            return $"{check} && {messageName}.{member.Name}.Length > 0";
        }

        return check;
    }

    private bool IsDecimalType(ITypeSymbol memberType)
    {
        return memberType.OriginalDefinition.SpecialType == SpecialType.System_Decimal;
    }

    public static bool HasCountProperty(ITypeSymbol type)
    {
        return HasProperty(type, "Count", SpecialType.System_Int32);
    }

    public static bool HasLengthProperty(ITypeSymbol type)
    {
        return HasProperty(type, "Length", SpecialType.System_Int32);
    }

    public static bool HasProperty(ITypeSymbol type, string name, SpecialType specialType)
    {
        if (
            type.GetMembers()
                .OfType<IPropertySymbol>()
                .Any(o => o.Name == name && o.Type.SpecialType == specialType)
        )
        {
            return true;
        }

        if (type.TypeKind == TypeKind.Interface)
        {
            if (type.AllInterfaces.Any(x => HasProperty(x, name, specialType)))
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<string> GetProtoParserMember(
        Compilation compilation,
        ProtoMember member,
        string readOrWriter,
        ITypeSymbol targetType
    )
    {
        var protoParser = GetProtoParser(
            compilation,
            member.Type,
            member.DataFormat,
            member.MapFormat,
            readOrWriter,
            member.RawTag,
            targetType,
            member.IsPacked,
            depth: 0,
            member.CompatibilityLevel,
            member.StringIntern,
            member
        );

        var memberType = member.Type.WithNullableAnnotation(NullableAnnotation.None);

        yield return $"private IProto{readOrWriter}<{memberType}> _{member.Name}_Proto{readOrWriter};";
        yield return $"private IProto{readOrWriter}<{memberType}> {member.Name}_Proto{readOrWriter} {{ get => _{member.Name}_Proto{readOrWriter} ??= {protoParser};}}";
    }

    public uint GetFieldNumber(uint rawTag)
    {
        return rawTag >> 3;
    }

    private string GetProtoParser(
        Compilation compilation,
        ITypeSymbol memberType,
        DataFormat format,
        (DataFormat keyFormat, DataFormat valueFormat) mapFormat,
        string readerOrWriter,
        uint rawTag,
        ITypeSymbol targetType,
        bool isPacked,
        int depth,
        CompatibilityLevel compatibilityLevel,
        bool stringIntern,
        ProtoMember member
    )
    {
        depth++;
        if (SymbolEqualityComparer.IncludeNullability.Equals(targetType, memberType))
        {
            return "this";
        }
        var proxyType = GetProxyType(memberType);
        if (proxyType is not null)
        {
            return GetProtoParser(
                compilation,
                proxyType,
                format,
                mapFormat,
                readerOrWriter,
                rawTag,
                targetType,
                isPacked,
                depth,
                compatibilityLevel,
                stringIntern,
                member
            );
        }

        if (IsProtoBufMessage(memberType))
        {
            return $"{memberType.WithNullableAnnotation(NullableAnnotation.None)}.{readerOrWriter}";
        }

        var fieldNumber = GetFieldNumber(rawTag);
        if (memberType is IArrayTypeSymbol arrayType)
        {
            if (rawTag == 0)
            {
                throw new Exception("rawTag==0");
            }

            var elementType = arrayType.ElementType;
            if (elementType.SpecialType == SpecialType.System_Byte)
            {
                return $"ByteArrayProtoParser.{readerOrWriter}";
            }
            var tag2 = ProtoMember.GetRawTag(
                fieldNumber,
                ProtoMember.GetPbWireType(compilation, elementType, format)
            );
            var elementWriter = GetProtoParser(
                compilation,
                elementType,
                format,
                mapFormat,
                readerOrWriter,
                0,
                targetType,
                isPacked,
                depth,
                compatibilityLevel,
                stringIntern,
                member
            );
            var fixedSize = GetFixedSize(elementType, format);
            return $"new ArrayProto{readerOrWriter}<{elementType}>({elementWriter},{rawTag},{fixedSize},{(isPacked ? "true" : "false")},{tag2})";
        }

        if (memberType is INamedTypeSymbol namedType)
        {
            var typeArguments = namedType.TypeArguments;

            if (typeArguments.Length == 0)
            {
                if (namedType.TypeKind == TypeKind.Enum)
                {
                    return $"EnumProtoParser<{namedType}>.{readerOrWriter}";
                }

                var name = namedType.SpecialType switch
                {
                    SpecialType.System_Int32 when format is DataFormat.ZigZag => "SInt32",
                    SpecialType.System_Int64 when format is DataFormat.ZigZag => "SInt64",
                    SpecialType.System_Int32 when format is DataFormat.FixedSize => "SFixed32",
                    SpecialType.System_Int64 when format is DataFormat.FixedSize => "SFixed64",
                    SpecialType.System_UInt32 when format is DataFormat.FixedSize => "Fixed32",
                    SpecialType.System_UInt64 when format is DataFormat.FixedSize => "Fixed64",
                    _ => namedType.Name,
                };
                if (compatibilityLevel >= CompatibilityLevel.Level240)
                {
                    if (
                        namedType.SpecialType is SpecialType.System_DateTime
                        || IsTimeSpanType(namedType)
                    )
                    {
                        name = $"{name}240";
                    }
                }

                if (compatibilityLevel >= CompatibilityLevel.Level300)
                {
                    if (
                        IsGuidType(namedType)
                        || namedType.SpecialType == SpecialType.System_Decimal
                    )
                    {
                        name = $"{name}300";
                    }
                }
                if (namedType.SpecialType == SpecialType.System_String && stringIntern)
                {
                    name = "InternedString";
                }

                return $"{name}ProtoParser.{readerOrWriter}";
            }

            if (typeArguments.Length == 1)
            {
                if (rawTag == 0)
                {
                    throw new Exception("rawTag==0");
                }

                var elementType = typeArguments[0];
                if (elementType.SpecialType == SpecialType.System_Byte)
                {
                    return $"ByteListProtoParser.{readerOrWriter}";
                }
                var elementParser = GetProtoParser(
                    compilation,
                    elementType,
                    format,
                    mapFormat,
                    readerOrWriter,
                    0,
                    targetType,
                    isPacked,
                    depth,
                    compatibilityLevel,
                    stringIntern,
                    member
                );
                var fixedSize = GetFixedSize(elementType, format);
                var tag2 = ProtoMember.GetRawTag(
                    fieldNumber,
                    ProtoMember.GetPbWireType(compilation, elementType, format)
                );
                if (namedType.TypeKind == TypeKind.Interface)
                {
                    if (readerOrWriter == "Reader")
                    {
                        if (IsListType(compilation, namedType))
                        {
                            return $"new ListProto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize},{(isPacked ? "true" : "false")},{tag2})";
                        }

                        if (IsSetType(compilation, namedType))
                        {
                            return $"new HashSetProto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize},{(isPacked ? "true" : "false")},{tag2})";
                        }
                    }
                    else if (readerOrWriter == "Writer")
                    {
                        if (IsCollectionType(compilation, elementType, namedType))
                        {
                            var count = "Count()";
                            if (HasCountProperty(memberType))
                            {
                                count = "Count";
                            }
                            if (HasLengthProperty(memberType))
                            {
                                count = "Length";
                            }
                            return $"new IEnumerableProto{readerOrWriter}<{memberType},{elementType}>({elementParser},{rawTag},static (d)=>d.{count},{fixedSize},{(isPacked ? "true" : "false")},{tag2})";
                        }
                    }
                }

                if (namedType.TypeKind == TypeKind.Class || namedType.TypeKind == TypeKind.Struct)
                {
                    if (namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    {
                        return $"new {memberType.Name}Proto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize})";
                    }
                    return $"new {memberType.Name}Proto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize},{(isPacked ? "true" : "false")},{tag2})";
                }
            }

            if (typeArguments.Length == 2)
            {
                if (rawTag == 0)
                {
                    throw new Exception("rawTag==0");
                }

                var keyType = typeArguments[0];
                var keyTag = ProtoMember.GetRawTag(
                    1,
                    ProtoMember.GetPbWireType(compilation, keyType, mapFormat.keyFormat)
                );
                var keyWriter = GetProtoParser(
                    compilation,
                    GetProxyType(keyType) ?? keyType,
                    mapFormat.keyFormat,
                    mapFormat,
                    readerOrWriter,
                    keyTag,
                    targetType,
                    isPacked: false,
                    depth: depth,
                    compatibilityLevel,
                    stringIntern,
                    member
                );
                var valueType = typeArguments[1];
                var valueTag = ProtoMember.GetRawTag(
                    2,
                    ProtoMember.GetPbWireType(compilation, valueType, mapFormat.valueFormat)
                );
                var valueWriter = GetProtoParser(
                    compilation,
                    GetProxyType(valueType) ?? valueType,
                    mapFormat.valueFormat,
                    mapFormat,
                    readerOrWriter,
                    valueTag,
                    targetType,
                    isPacked: false,
                    depth: depth,
                    compatibilityLevel,
                    stringIntern,
                    member
                );
                var tag2 = ProtoMember.GetRawTag(
                    fieldNumber,
                    ProtoMember.PbWireType.LengthDelimited
                );
                if (namedType.TypeKind == TypeKind.Interface)
                {
                    if (readerOrWriter == "Reader")
                    {
                        var mapType = compilation
                            .GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")
                            ?.Construct(keyType, valueType)!;
                        var conversion = compilation.ClassifyConversion(mapType, namedType);
                        if (conversion.IsImplicit)
                        {
                            return $"new DictionaryProto{readerOrWriter}<{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag},{keyTag},{valueTag},{tag2})";
                        }
                    }
                    else if (readerOrWriter == "Writer")
                    {
                        if (IsDictionaryType(compilation, keyType, valueType, namedType))
                        {
                            var count = "Count()";

                            if (HasCountProperty(memberType))
                            {
                                count = "Count";
                            }
                            return $"new IEnumerableKeyValuePairProto{readerOrWriter}<{memberType},{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag},{keyTag},{valueTag},static (d)=>d.{count},{tag2})";
                        }
                    }
                }

                if (namedType.TypeKind == TypeKind.Class || namedType.TypeKind == TypeKind.Struct)
                {
                    return $"new {memberType.Name}Proto{readerOrWriter}<{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag},{keyTag},{valueTag},{tag2})";
                }
            }
        }

        throw new LightProtoGeneratorException(
            $"Member:{member.Name} Type: {memberType} is not supported"
        )
        {
            Id = "LIGHT_PROTO_004",
            Title = $"MemberType is not supported",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = member.DelclarationSyntax.GetLocation(),
        };
    }

    private static bool IsCollectionType(Compilation compilation, ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            return IsCollectionType(compilation, arrayType.ElementType, type);
        }

        if (type is not INamedTypeSymbol namedType)
            return false;

        if (namedType.TypeArguments.Length != 1)
            return false;

        if (namedType.OriginalDefinition.SpecialType is SpecialType.System_Nullable_T)
            return false;

        var elementType = namedType.TypeArguments[0];

        return IsCollectionType(compilation, elementType, namedType);
    }

    private static bool IsCollectionType(
        Compilation compilation,
        ITypeSymbol elementType,
        ITypeSymbol type
    )
    {
        if (elementType.SpecialType == SpecialType.System_Byte)
            return false;
        var baseCollectionType = compilation
            .GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")
            ?.Construct(elementType)!;
        var conversion = compilation.ClassifyConversion(type, baseCollectionType);
        return conversion.IsImplicit;
    }

    private static bool IsStackType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var constructedFrom = namedType.OriginalDefinition.ToDisplayString();
        return constructedFrom
            is "System.Collections.Generic.Stack<T>"
                or "System.Collections.Concurrent.ConcurrentStack<T>";
    }

    private static bool IsQueueType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var constructedFrom = namedType.OriginalDefinition.ToDisplayString();
        return constructedFrom
            is "System.Collections.Generic.Queue<T>"
                or "System.Collections.Concurrent.ConcurrentQueue<T>";
    }

    private static bool IsDictionaryType(
        Compilation compilation,
        ITypeSymbol keyType,
        ITypeSymbol valueType,
        INamedTypeSymbol namedType
    )
    {
        var keyValuePairType = compilation
            .GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2")
            ?.Construct(keyType, valueType)!;
        var baseDictionaryType = compilation
            .GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")
            ?.Construct(keyValuePairType)!;
        var conversion = compilation.ClassifyConversion(namedType, baseDictionaryType);
        return conversion.IsImplicit;
    }

    static bool IsDictionaryType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;
        if (namedType.TypeArguments.Length != 2)
            return false;
        var keyType = namedType.TypeArguments[0];
        var valueType = namedType.TypeArguments[1];

        return IsDictionaryType(compilation, keyType, valueType, namedType);
    }

    static ITypeSymbol GetElementType(Compilation compilation, ITypeSymbol collectionType)
    {
        if (IsArrayType(collectionType))
        {
            return ((IArrayTypeSymbol)collectionType).ElementType;
        }

        if (collectionType is INamedTypeSymbol { TypeArguments.Length: 1 } namedType)
        {
            return namedType.TypeArguments[0];
        }

        throw new ArgumentException(
            "Type is not an array, list, set, or concurrent collection type",
            nameof(collectionType)
        );
    }

    private int GetFixedSize(ITypeSymbol elementType, DataFormat dataFormat)
    {
        return elementType.SpecialType switch
        {
            SpecialType.System_Boolean => 1,
            SpecialType.System_Int32
            or SpecialType.System_UInt32 when dataFormat is DataFormat.FixedSize => 4,
            SpecialType.System_Int64
            or SpecialType.System_UInt64 when dataFormat is DataFormat.FixedSize => 8,
            SpecialType.System_Single => 4,
            SpecialType.System_Double => 8,
            _ => 0,
        };
    }

    private bool IsProtoBufMessage(ITypeSymbol memberType)
    {
        return memberType.TypeKind != TypeKind.Enum
                && memberType
                    .GetAttributes()
                    .Any(o =>
                        o.AttributeClass?.ToDisplayString() == "LightProto.ProtoContractAttribute"
                    )
            || (
                memberType is INamedTypeSymbol namedType
                && namedType.AllInterfaces.Any(i =>
                    i.ToDisplayString().StartsWith("LightProto.IProtoParser<")
                )
            );
    }

    static bool IsArrayType(ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Array;
    }

    static bool IsListType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var typeArguments = namedType.TypeArguments;
        if (typeArguments.Length != 1)
        {
            return false;
        }

        var elementType = typeArguments[0];

        var listType = compilation
            .GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?.Construct(elementType)!;
        var conversion = CSharpExtensions.ClassifyConversion(compilation, listType, type);
        return conversion.IsImplicit;
    }

    static bool IsSetType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var typeArguments = namedType.TypeArguments;
        if (typeArguments.Length != 1)
        {
            return false;
        }

        var elementType = typeArguments[0];

        var listType = compilation
            .GetTypeByMetadataName("System.Collections.Generic.HashSet`1")
            ?.Construct(elementType)!;
        var conversion = CSharpExtensions.ClassifyConversion(compilation, listType, type);
        return conversion.IsImplicit;
    }

    static INamedTypeSymbol ResolveConcreteTypeSymbol(
        Compilation compilation,
        INamedTypeSymbol type
    )
    {
        // 如果是接口，就手动映射到具体类型
        var constructedFrom = type.OriginalDefinition.ToDisplayString();

        return constructedFrom switch
        {
            "System.Collections.Generic.IList<T>"
            or "System.Collections.Generic.ICollection<T>"
            or "System.Collections.Generic.IReadOnlyCollection<T>" => compilation
                .GetTypeByMetadataName("System.Collections.Generic.List`1")
                ?.Construct(type.TypeArguments.ToArray()) ?? type,

            "System.Collections.Generic.IDictionary<TKey, TValue>"
            or "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>" => compilation
                .GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")
                ?.Construct(type.TypeArguments.ToArray()) ?? type,

            "System.Collections.Generic.ISet<T>" => compilation
                .GetTypeByMetadataName("System.Collections.Generic.HashSet`1")
                ?.Construct(type.TypeArguments.ToArray()) ?? type,

            _ => type, // 如果本身就是具体类，就直接返回
        };
    }

    string GetIntendedSpace(int i)
    {
        return new string(' ', i * 4);
    }

    static bool IsGuidType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Guid" || displayString == "Guid";
    }

    static bool IsTimeSpanType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.TimeSpan" || displayString == "TimeSpan";
    }

    static bool IsDateOnlyType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.DateOnly" || displayString == "DateOnly";
    }

    static bool IsTimeOnlyType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.TimeOnly" || displayString == "TimeOnly";
    }

    static bool IsStringBuilderType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Text.StringBuilder" || displayString == "StringBuilder";
    }

    private ITypeSymbol? GetProxyType(IEnumerable<AttributeData> attributeDatas)
    {
        if (
            attributeDatas.FirstOrDefault(o =>
                o.AttributeClass?.ToDisplayString().StartsWith("LightProto.ProtoProxyAttribute<")
                == true
            ) is
            { } proxyAttr2
        )
        {
            return proxyAttr2.AttributeClass!.TypeArguments[0];
        }

        return null;
    }

    private ITypeSymbol? GetProxyFor(IEnumerable<AttributeData> attributeDatas)
    {
        if (
            attributeDatas.FirstOrDefault(o =>
                o.AttributeClass?.ToDisplayString().StartsWith("LightProto.ProtoProxyForAttribute<")
                == true
            ) is
            { } proxyAttr2
        )
        {
            return proxyAttr2.AttributeClass!.TypeArguments[0];
        }

        return null;
    }

    private ITypeSymbol? GetProxyType(ITypeSymbol type)
    {
        return GetProxyType(type.GetAttributes());
    }

    private List<ProtoMember> GetProtoMembers(
        Compilation compilation,
        INamedTypeSymbol targetType,
        TypeDeclarationSyntax typeDeclaration
    )
    {
        var members = new List<ProtoMember>();

        foreach (var member in targetType.GetMembers())
        {
            if (member.IsStatic)
            {
                continue;
            }

            if (member.IsImplicitlyDeclared)
            {
                continue;
            }

            AttributeData? protoMemberAttr = member
                .GetAttributes()
                .FirstOrDefault(attr =>
                    attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoMemberAttribute"
                );
            if (protoMemberAttr == null)
                continue;

            string memberName;
            string? initializer;
            NullableAnnotation nullableAnnotation;
            ITypeSymbol memberType;
            bool isReadOnly;
            bool isRequired;
            bool isInitOnly;
            MemberDeclarationSyntax? memberDeclarationSyntax;
            if (member is IPropertySymbol property)
            {
                memberName = property.Name;
                var propertyDeclarationSyntax = typeDeclaration
                    .Members.OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(m => m.Identifier.Text == memberName);
                memberDeclarationSyntax = propertyDeclarationSyntax;
                initializer = propertyDeclarationSyntax?.Initializer?.Value.ToString();
                nullableAnnotation = property.NullableAnnotation;
                memberType = property.Type;
                isReadOnly = property.IsReadOnly;
                isRequired = property.IsRequired;
                isInitOnly = property.SetMethod?.IsInitOnly == true;
            }
            else if (member is IFieldSymbol field)
            {
                memberName = field.Name;
                var fieldDeclarationSyntax = typeDeclaration
                    .Members.OfType<FieldDeclarationSyntax>()
                    .FirstOrDefault(m =>
                        m.Declaration.Variables.Any(v => v.Identifier.Text == memberName)
                    );
                memberDeclarationSyntax = fieldDeclarationSyntax;
                initializer = fieldDeclarationSyntax
                    ?.Declaration.Variables.FirstOrDefault()
                    ?.Initializer?.Value.ToString();
                nullableAnnotation = field.NullableAnnotation;
                memberType = field.Type;
                isReadOnly = field.IsReadOnly;
                isRequired = field.IsRequired;
                isInitOnly = false;
            }
            else
            {
                continue;
            }

            if (memberDeclarationSyntax is null)
            {
                throw new LightProtoGeneratorException(
                    $"can not find Member:{member.Name} DelclarationSyntax"
                )
                {
                    Id = "LIGHT_PROTO_003",
                    Title = "DelclarationSyntax not found",
                    Category = "Usage",
                    Severity = DiagnosticSeverity.Warning,
                    Location = null,
                };
            }

            if (initializer is null)
            {
                if (nullableAnnotation == NullableAnnotation.Annotated)
                {
                    initializer = "null";
                }
                else if (HasEmptyStaticField(memberType))
                {
                    initializer = $"{memberType}.Empty";
                }
                else if (
                    IsCollectionType(compilation, memberType)
                    || IsDictionaryType(compilation, memberType)
                )
                {
                    if (memberType is IArrayTypeSymbol arrayTypeSymbol)
                    {
                        initializer = $"Array.Empty<{arrayTypeSymbol.ElementType}>()";
                    }
                    else if (memberType.TypeKind == TypeKind.Interface || memberType.IsAbstract)
                    {
                        var concreteType = ResolveConcreteTypeSymbol(
                            compilation,
                            (memberType as INamedTypeSymbol)!
                        );
                        initializer = $"new {concreteType.ToDisplayString()}()";
                    }
                    else
                    {
                        initializer = HasParameterlessConstructor(memberType)
                            ? $"new ()"
                            : "default";
                    }
                }
                else
                {
                    initializer = HasParameterlessConstructor(memberType) ? $"new ()" : "default";
                }
            }

            bool HasEmptyStaticField(ITypeSymbol type)
            {
                return type.GetMembers()
                    .OfType<IFieldSymbol>()
                    .Any(o => o.IsStatic && o.Name == "Empty");
            }
            bool HasParameterlessConstructor(ITypeSymbol type)
            {
                if (type is INamedTypeSymbol namedType)
                {
                    return namedType.InstanceConstructors.Any(c =>
                        c.Parameters.Length == 0 && c.DeclaredAccessibility == Accessibility.Public
                    );
                }
                return false;
            }
            ITypeSymbol? ProxyType =
                GetProxyType(member.GetAttributes()) ?? GetProxyType(memberType);

            var tag = (uint)protoMemberAttr.ConstructorArguments[0].Value!;

            var dataFormat = Enum.TryParse<DataFormat>(
                protoMemberAttr
                    .NamedArguments.FirstOrDefault(kv => kv.Key == "DataFormat")
                    .Value.Value?.ToString(),
                out var _dataFormat
            )
                ? _dataFormat
                : DataFormat.Default;

            bool HasStringInternAttribute(IEnumerable<AttributeData> attributeDatas)
            {
                return attributeDatas.Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == "LightProto.StringInternAttribute"
                );
            }

            bool stringIntern =
                HasStringInternAttribute(member.GetAttributes())
                || HasStringInternAttribute(targetType.GetAttributes())
                || HasStringInternAttribute(targetType.ContainingModule.GetAttributes())
                || HasStringInternAttribute(targetType.ContainingAssembly.GetAttributes());

            AttributeData? GetCompatibilityLevelAttribute(IEnumerable<AttributeData> attributeDatas)
            {
                return attributeDatas.FirstOrDefault(attr =>
                    attr.AttributeClass?.ToDisplayString()
                    == "LightProto.CompatibilityLevelAttribute"
                );
            }

            AttributeData? compatibilityLevelAttr =
                GetCompatibilityLevelAttribute(member.GetAttributes())
                ?? GetCompatibilityLevelAttribute(targetType.GetAttributes())
                ?? GetCompatibilityLevelAttribute(targetType.ContainingModule.GetAttributes())
                ?? GetCompatibilityLevelAttribute(targetType.ContainingAssembly.GetAttributes());

            CompatibilityLevel compatibilityLevel = Enum.TryParse<CompatibilityLevel>(
                compatibilityLevelAttr?.ConstructorArguments[0].Value?.ToString(),
                out var _compatibilityLevel
            )
                ? _compatibilityLevel
                : CompatibilityLevel.Level200;

            if (
#pragma warning disable CS0618 // Type or member is obsolete
                dataFormat is DataFormat.WellKnown
#pragma warning restore CS0618 // Type or member is obsolete
                && compatibilityLevel <= CompatibilityLevel.Level200
            )
            {
                compatibilityLevel = CompatibilityLevel.Level240;
            }
            bool isPacked =
                bool.TryParse(
                    protoMemberAttr
                        .NamedArguments.FirstOrDefault(kv => kv.Key == "IsPacked")
                        .Value.Value?.ToString(),
                    out var _isPacked
                ) && _isPacked;
            AttributeData? protoMapAttr = member
                .GetAttributes()
                .FirstOrDefault(attr =>
                    attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoMapAttribute"
                );

            var keyFormat = Enum.TryParse<DataFormat>(
                protoMapAttr
                    ?.NamedArguments.FirstOrDefault(kv => kv.Key == "KeyFormat")
                    .Value.Value?.ToString(),
                out var _keyFormat
            )
                ? _keyFormat
                : DataFormat.Default;

            var valueFormat = Enum.TryParse<DataFormat>(
                protoMapAttr
                    ?.NamedArguments.FirstOrDefault(kv => kv.Key == "ValueFormat")
                    .Value.Value?.ToString(),
                out var _valueFormat
            )
                ? _valueFormat
                : DataFormat.Default;

            members.Add(
                new ProtoMember
                {
                    Name = memberName,
                    Type = memberType,
                    FieldNumber = tag,
                    Compilation = compilation,
                    IsRequired = isRequired,
                    IsInitOnly = isInitOnly,
                    AttributeData = member.GetAttributes(),
                    DataFormat = dataFormat,
                    MapFormat = (keyFormat, valueFormat),
                    ProxyType = ProxyType,
                    Initializer = initializer,
                    IsPacked = isPacked,
                    CompatibilityLevel = compatibilityLevel,
                    IsReadOnly = isReadOnly,
                    StringIntern = stringIntern,
                    DelclarationSyntax = memberDeclarationSyntax,
                }
            );
        }

        return members.OrderBy(m => m.FieldNumber).ToList();
    }

    private class ProtoMember
    {
        public enum PbWireType
        {
            Varint = 0, // int32, int64, bool, enum ,uint32, uint64
            Fixed64 = 1, // double, fixed64, sfixed64
            LengthDelimited = 2, // string, bytes, message, repeated packed
            Fixed32 = 5, // float, fixed32, sfixed32
        }

        public Compilation Compilation { get; set; } = null!;

        public ITypeSymbol? ProxyType { get; set; }
        public string Name { get; set; } = "";
        public ITypeSymbol Type { get; set; } = null!;
        public DataFormat DataFormat { get; set; }
        public (DataFormat keyFormat, DataFormat valueFormat) MapFormat { get; set; }
        public uint FieldNumber { get; set; }
        public bool IsRequired { get; set; }
        public bool IsInitOnly { get; set; }
        public string Initializer { get; set; } = "default";
        public bool StringIntern { get; set; }

        public ImmutableArray<AttributeData> AttributeData { get; set; } =
            ImmutableArray<AttributeData>.Empty;

        public PbWireType WireType => GetPbWireType(Compilation, Type, DataFormat);

        public static PbWireType GetPbWireType(
            Compilation compilation,
            ITypeSymbol Type,
            DataFormat DataFormat
        )
        {
            // Handle nullable value types by getting the underlying type
            if (
                Type is INamedTypeSymbol namedType
                && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
            )
            {
                return GetPbWireType(compilation, namedType.TypeArguments[0], DataFormat);
            }

            // Handle arrays, lists and sets
            if (IsCollectionType(compilation, Type))
            {
                return PbWireType.LengthDelimited;
            }

            // Handle dictionaries
            if (IsDictionaryType(compilation, Type))
            {
                return PbWireType.LengthDelimited;
            }

            if (Type.TypeKind == TypeKind.Enum)
            {
                return PbWireType.Varint;
            }

            switch (Type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Int32:
                {
                    return DataFormat == DataFormat.FixedSize
                        ? PbWireType.Fixed32
                        : PbWireType.Varint;
                }
                case SpecialType.System_UInt32:
                {
                    return DataFormat == DataFormat.FixedSize
                        ? PbWireType.Fixed32
                        : PbWireType.Varint;
                }
                case SpecialType.System_Int64:
                {
                    return DataFormat == DataFormat.FixedSize
                        ? PbWireType.Fixed64
                        : PbWireType.Varint;
                }
                case SpecialType.System_UInt64:
                {
                    return DataFormat == DataFormat.FixedSize
                        ? PbWireType.Fixed64
                        : PbWireType.Varint;
                }
                case SpecialType.System_Enum:
                    return PbWireType.Varint;
                case SpecialType.System_Single:
                    return PbWireType.Fixed32;
                case SpecialType.System_Double:
                    return PbWireType.Fixed64;
                case SpecialType.None when IsDateOnlyType(Type): //int32
                    return PbWireType.Varint;
                case SpecialType.None when IsTimeOnlyType(Type): //int64
                    return PbWireType.Varint;
                case SpecialType.System_String:
                case SpecialType.None when IsGuidType(Type):
                case SpecialType.None when IsStringBuilderType(Type):
                case SpecialType.None
                    when Type.TypeKind == TypeKind.Class
                        || Type.TypeKind == TypeKind.Interface
                        || Type.TypeKind == TypeKind.Array:
                    return PbWireType.LengthDelimited;
                default:
                    // Default to LengthDelimited for other complex types
                    return PbWireType.LengthDelimited;
            }
        }

        public uint RawTag => GetRawTag(FieldNumber, WireType);
        public byte[] RawTagBytes => GetRawBytes(FieldNumber, WireType);
        public bool IsPacked { get; set; }
        public CompatibilityLevel CompatibilityLevel { get; set; }
        public bool IsReadOnly { get; set; }
        public MemberDeclarationSyntax DelclarationSyntax { get; set; } = null!;

        public static uint GetRawTag(uint Tag, PbWireType WireType)
        {
            return (Tag << 3) | (uint)WireType;
        }

        public static byte[] GetRawBytes(uint fieldNumber, PbWireType wireType)
        {
            uint tag = (fieldNumber << 3) | (uint)wireType;
            return EncodeVarint(tag);
        }

        private static byte[] EncodeVarint(uint value)
        {
            var bytes = new List<byte>();
            while (value > 127)
            {
                bytes.Add((byte)((value & 0x7F) | 0x80)); // 低7位 + 最高位1，表示后面还有字节
                value >>= 7;
            }

            bytes.Add((byte)value); // 最后一个字节
            return bytes.ToArray();
        }
    }
}

internal class LightProtoGeneratorException(string message) : Exception(message)
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DiagnosticSeverity Severity { get; set; }
    public Location? Location { get; set; }
}
