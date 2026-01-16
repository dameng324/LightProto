using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace LightProto.Generator;

internal static class Helper
{
    internal static bool IsGuidType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Guid" || displayString == "Guid";
    }

    internal static bool IsTimeSpanType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.TimeSpan" || displayString == "TimeSpan";
    }

    internal static bool IsDateOnlyType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.DateOnly" || displayString == "DateOnly";
    }

    internal static bool IsTimeOnlyType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.TimeOnly" || displayString == "TimeOnly";
    }

    internal static bool IsRuneType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Text.Rune" || displayString == "Rune";
    }

    internal static bool IsStringBuilderType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Text.StringBuilder" || displayString == "StringBuilder";
    }

    internal static bool IsHalfType(ITypeSymbol type)
    {
        var displayString = type.ToDisplayString();
        return displayString == "System.Half" || displayString == "Half";
    }

    internal static bool IsDecimalType(ITypeSymbol memberType)
    {
        return memberType.OriginalDefinition.SpecialType == SpecialType.System_Decimal;
    }

    public static bool HasCountProperty(ITypeSymbol type)
    {
        return HasProperty(type, "Count", SpecialType.System_Int32);
    }

    public static bool HasLengthProperty(ITypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Array)
        {
            return true;
        }

        return HasProperty(type, "Length", SpecialType.System_Int32);
    }

    public static bool HasProperty(ITypeSymbol type, string name, SpecialType specialType)
    {
        if (type.GetMembers().OfType<IPropertySymbol>().Any(o => o.Name == name && o.Type.SpecialType == specialType))
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

    internal static bool IsCollectionType(Compilation compilation, ITypeSymbol type) => IsCollectionType(compilation, type, out _);

    internal static bool IsCollectionType(Compilation compilation, ITypeSymbol type, out ITypeSymbol? itemType)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            itemType = arrayType.ElementType;
            return IsCollectionType(compilation, arrayType.ElementType, type);
        }

        itemType = null;
        if (type is not INamedTypeSymbol namedType)
            return false;

        if (namedType.TypeArguments.Length != 1)
            return false;

        if (namedType.OriginalDefinition.SpecialType is SpecialType.System_Nullable_T)
            return false;

        if (namedType.OriginalDefinition.ToDisplayString() == "System.Lazy<T>")
            return false;

        itemType = namedType.TypeArguments[0];
        return IsCollectionType(compilation, itemType, namedType);
    }

    internal static bool IsCollectionType(Compilation compilation, ITypeSymbol elementType, ITypeSymbol type)
    {
        if (elementType.SpecialType == SpecialType.System_Byte && IsArrayType(type))
            return false;
        var baseCollectionType = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?.Construct(elementType)!;
        var conversion = compilation.ClassifyConversion(type, baseCollectionType);
        return conversion.IsImplicit;
    }

    internal static bool IsStackType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var constructedFrom = namedType.OriginalDefinition.ToDisplayString();
        return constructedFrom is "System.Collections.Generic.Stack<T>" or "System.Collections.Concurrent.ConcurrentStack<T>";
    }

    internal static bool IsQueueType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var constructedFrom = namedType.OriginalDefinition.ToDisplayString();
        return constructedFrom is "System.Collections.Generic.Queue<T>" or "System.Collections.Concurrent.ConcurrentQueue<T>";
    }

    internal static bool IsDictionaryType(Compilation compilation, ITypeSymbol keyType, ITypeSymbol valueType, INamedTypeSymbol namedType)
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

    internal static bool IsDictionaryType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;
        if (namedType.TypeArguments.Length != 2)
            return false;
        var keyType = namedType.TypeArguments[0];
        var valueType = namedType.TypeArguments[1];

        return IsDictionaryType(compilation, keyType, valueType, namedType);
    }

    internal static ITypeSymbol GetElementType(Compilation compilation, ITypeSymbol collectionType)
    {
        if (IsArrayType(collectionType))
        {
            return ((IArrayTypeSymbol)collectionType).ElementType;
        }

        if (collectionType is INamedTypeSymbol { TypeArguments.Length: 1 } namedType)
        {
            return namedType.TypeArguments[0];
        }

        throw new ArgumentException("Type is not an array, list, set, or concurrent collection type", nameof(collectionType));
    }

    internal static bool IsArrayType(ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Array;
    }

    internal static bool IsListType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var typeArguments = namedType.TypeArguments;
        if (typeArguments.Length != 1)
        {
            return false;
        }

        var elementType = typeArguments[0];

        var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")?.Construct(elementType)!;
        var conversion = CSharpExtensions.ClassifyConversion(compilation, listType, type);
        return conversion.IsImplicit;
    }

    internal static bool IsSetType(Compilation compilation, ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        var typeArguments = namedType.TypeArguments;
        if (typeArguments.Length != 1)
        {
            return false;
        }

        var elementType = typeArguments[0];

        var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.HashSet`1")?.Construct(elementType)!;
        var conversion = CSharpExtensions.ClassifyConversion(compilation, listType, type);
        return conversion.IsImplicit;
    }

    internal static INamedTypeSymbol ResolveConcreteTypeSymbol(Compilation compilation, INamedTypeSymbol type)
    {
        // 如果是接口，就手动映射到具体类型
        var constructedFrom = type.OriginalDefinition.ToDisplayString();

        return constructedFrom switch
        {
            "System.Collections.Generic.IList<T>"
            or "System.Collections.Generic.IReadOnlyList<T>"
            or "System.Collections.Generic.ICollection<T>"
            or "System.Collections.Generic.IReadOnlyCollection<T>" => compilation
                .GetTypeByMetadataName("System.Collections.Generic.List`1")
                ?.Construct(type.TypeArguments.ToArray())
                ?? type,

            "System.Collections.Generic.IDictionary<TKey, TValue>" or "System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>" =>
                compilation.GetTypeByMetadataName("System.Collections.Generic.Dictionary`2")?.Construct(type.TypeArguments.ToArray())
                    ?? type,

            "System.Collections.Generic.ISet<T>" => compilation
                .GetTypeByMetadataName("System.Collections.Generic.HashSet`1")
                ?.Construct(type.TypeArguments.ToArray())
                ?? type,

            _ => type, // 如果本身就是具体类，就直接返回
        };
    }

    internal static INamedTypeSymbol? ResolveParserTypeFromAttribute(
        INamedTypeSymbol memberType,
        ImmutableArray<AttributeData> attributes,
        bool isAssembly
    )
    {
        var mapAttributes = attributes
            .Where(o =>
                o.AttributeClass?.ToDisplayString() == "LightProto.ProtoParserTypeMapAttribute"
                && SymbolEqualityComparer.Default.Equals(o.ConstructorArguments[0].Value as INamedTypeSymbol, memberType)
            )
            .ToArray();
        if (mapAttributes.Length > 1)
        {
            throw LightProtoGeneratorException.Duplicate_ProtoParserTypeMapAttribute(
                memberType.ToDisplayString(),
                mapAttributes.Select(x => x.ApplicationSyntaxReference?.GetSyntax().GetLocation()).OfType<Location>().ToArray()
            );
        }

        var attribute = mapAttributes.FirstOrDefault();
        if (attribute is null)
            return null;

        var parser = (attribute.ConstructorArguments[1].Value as INamedTypeSymbol)!;

        var isProtoContract = parser
            .GetAttributes()
            .Any(o => o.AttributeClass?.ToDisplayString() == LightProtoGenerator.ProtoContractAttributeFullName);

        var memberTypeDisplayString = memberType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString();
        // if parser does not contain static member named ProtoReader and is IProtoReader<T> or ProtoWriter and is IProtoWriter<T>, then error
        var hasProtoReader = parser
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Any(o =>
                o.IsStatic
                && o.Name == "ProtoReader"
                && o.Type is INamedTypeSymbol returnType
                && (
                    returnType.AllInterfaces.Any(i => i.ToDisplayString() == $"LightProto.IProtoReader<{memberTypeDisplayString}>")
                    || returnType.ToDisplayString() == $"LightProto.IProtoReader<{memberTypeDisplayString}>"
                )
            );
        if (!isProtoContract && !hasProtoReader)
        {
            throw LightProtoGeneratorException.ProtoParserTypeMustContainProtoReaderWriter(
                parser.ToDisplayString(),
                memberTypeDisplayString,
                attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
            );
        }
        var hasProtoWriter = parser
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Any(o =>
                o.IsStatic
                && o.Name == "ProtoWriter"
                && o.Type is INamedTypeSymbol returnType
                && (
                    returnType.AllInterfaces.Any(i => i.ToDisplayString() == $"LightProto.IProtoWriter<{memberTypeDisplayString}>")
                    || returnType.ToDisplayString() == $"LightProto.IProtoWriter<{memberTypeDisplayString}>"
                )
            );

        if (!isProtoContract && hasProtoWriter is false)
        {
            throw LightProtoGeneratorException.ProtoParserTypeMustContainProtoReaderWriter(
                parser.ToDisplayString(),
                memberTypeDisplayString,
                attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
            );
        }

        if (isAssembly && SymbolEqualityComparer.Default.Equals(memberType.ContainingAssembly, parser.ContainingAssembly))
        {
            throw LightProtoGeneratorException.MessageTypeAndParserTypeCannotInSameAssemblyWhenUsingAssemblyLevelProtoParserMapAttribute(
                memberType.ToDisplayString(),
                parser.ToDisplayString(),
                attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
            );
        }

        return parser;
    }

    internal static int GetFixedSize(ITypeSymbol elementType, DataFormat dataFormat)
    {
        return elementType.SpecialType switch
        {
            SpecialType.System_Boolean => 1,
            SpecialType.System_Int32
            or SpecialType.System_UInt32
            or SpecialType.System_Int16
            or SpecialType.System_UInt16
            or SpecialType.System_Byte when dataFormat is DataFormat.FixedSize => 4,
            SpecialType.System_Int64 or SpecialType.System_UInt64 when dataFormat is DataFormat.FixedSize => 8,
            SpecialType.System_Single => 4,
            SpecialType.System_Double => 8,
            _ => 0,
        };
    }

    internal static bool HasProtoIncludeAttribute(INamedTypeSymbol baseType, INamedTypeSymbol derivedType)
    {
        return baseType
            .GetAttributes()
            .Any(attr =>
                attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoIncludeAttribute"
                && SymbolEqualityComparer.Default.Equals(attr.ConstructorArguments[1].Value as INamedTypeSymbol, derivedType)
            );
    }

    internal static INamedTypeSymbol? GetBaseProtoType(INamedTypeSymbol type)
    {
        if (type.AllInterfaces.FirstOrDefault(interf => IsProtoBufMessage(interf) && HasProtoIncludeAttribute(interf, type)) is { } it)
        {
            return it;
        }

        var baseType = type.BaseType;
        while (true)
        {
            if (baseType is null)
            {
                return null;
            }

            if (IsProtoBufMessage(baseType) && HasProtoIncludeAttribute(baseType, type))
            {
                return baseType;
            }

            baseType = baseType.BaseType;
        }
    }

    internal static bool IsProtoBufMessage(ITypeSymbol? memberType)
    {
        if (memberType is null)
        {
            return false;
        }
        return memberType.TypeKind != TypeKind.Enum
                && memberType
                    .GetAttributes()
                    .Any(o => o.AttributeClass?.ToDisplayString() == LightProtoGenerator.ProtoContractAttributeFullName)
            || (
                memberType is INamedTypeSymbol namedType
                && namedType.AllInterfaces.Any(i => i.ToDisplayString().StartsWith("LightProto.IProtoParser<"))
            );
    }

    internal static void GetProtoParserMember(
        CodeWriter writer,
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

        writer.WriteLine(
            $"private IProto{readOrWriter}<{memberType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}> _{member.Name}_Proto{readOrWriter};"
        );
        writer.WriteLine(
            $"internal IProto{readOrWriter}<{memberType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}> {member.Name}_Proto{readOrWriter} {{ get => _{member.Name}_Proto{readOrWriter} ??= {protoParser};}}"
        );
    }

    static uint GetFieldNumber(uint rawTag)
    {
        return rawTag >> 3;
    }

    private static string GetProtoParser(
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
        var proxyType = ProtoContract.GetProxyType(memberType);
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
            return memberType.TypeKind is TypeKind.Interface
                ? $"{memberType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}ProtoParser.Proto{readerOrWriter}"
                : $"{memberType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Proto{readerOrWriter}";
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
                return $"global::LightProto.Parser.ByteArrayProtoParser.Proto{readerOrWriter}";
            }

            if (!isPacked)
            {
                rawTag = ProtoMember.GetRawTag(fieldNumber, ProtoMember.GetPbWireType(compilation, elementType, format));
            }

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
            return $"new LightProto.Parser.ArrayProto{readerOrWriter}<{elementType}>({elementWriter},{rawTag},{fixedSize})";
        }

        if (memberType is INamedTypeSymbol namedType)
        {
            var typeArguments = namedType.TypeArguments;

            if (typeArguments.Length == 0)
            {
                // member level specified parser type
                INamedTypeSymbol? parserType =
                    member
                        .AttributeData.FirstOrDefault(o => o.AttributeClass?.ToDisplayString() == "LightProto.ProtoMemberAttribute")
                        ?.NamedArguments.FirstOrDefault(o => o.Key == "ParserType")
                        .Value.Value as INamedTypeSymbol;
                if (parserType is null)
                {
                    // target type / assembly level specified parser type
                    parserType =
                        ResolveParserTypeFromAttribute(namedType, targetType.GetAttributes(), false)
                        ?? ResolveParserTypeFromAttribute(namedType, targetType.ContainingModule.GetAttributes(), false)
                        ?? ResolveParserTypeFromAttribute(namedType, targetType.ContainingAssembly.GetAttributes(), true);
                }

                if (parserType is null)
                {
                    // type level specified parser type
                    parserType =
                        namedType
                            .GetAttributes()
                            .FirstOrDefault(o => o.AttributeClass?.ToDisplayString() == "LightProto.ProtoParserTypeAttribute")
                            ?.ConstructorArguments[0]
                            .Value as INamedTypeSymbol;
                }

                if (parserType is null && namedType.AllInterfaces.Any(o => o.ToDisplayString() == $"LightProto.IProtoParser<{namedType}>"))
                {
                    // use self implemented parser type
                    parserType = namedType;
                }

                if (parserType is not null)
                {
                    return $"{parserType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Proto{readerOrWriter}";
                }

                if (namedType.TypeKind == TypeKind.Enum)
                {
                    return $"global::LightProto.Parser.EnumProtoParser<{namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>.Proto{readerOrWriter}";
                }

                var name = namedType.SpecialType switch
                {
                    SpecialType.System_SByte when format is DataFormat.ZigZag => "SSByte",
                    SpecialType.System_Int16 when format is DataFormat.ZigZag => "SInt16",
                    SpecialType.System_Int32 when format is DataFormat.ZigZag => "SInt32",
                    SpecialType.System_Int64 when format is DataFormat.ZigZag => "SInt64",
                    SpecialType.System_SByte when format is DataFormat.FixedSize => "SFixedByte",
                    SpecialType.System_Int32 when format is DataFormat.FixedSize => "SFixed32",
                    SpecialType.System_Int16 when format is DataFormat.FixedSize => "SFixed16",
                    SpecialType.System_Int64 when format is DataFormat.FixedSize => "SFixed64",
                    SpecialType.System_Byte when format is DataFormat.FixedSize => "FixedByte",
                    SpecialType.System_UInt16 when format is DataFormat.FixedSize => "Fixed16",
                    SpecialType.System_UInt32 when format is DataFormat.FixedSize => "Fixed32",
                    SpecialType.System_UInt64 when format is DataFormat.FixedSize => "Fixed64",
                    _ => namedType.Name,
                };
                if (
                    compatibilityLevel >= CompatibilityLevel.Level240
                    && (namedType.SpecialType is SpecialType.System_DateTime || IsTimeSpanType(namedType))
                )
                {
                    name = $"{name}240";
                }

                if (
                    compatibilityLevel >= CompatibilityLevel.Level300
                    && (IsGuidType(namedType) || namedType.SpecialType == SpecialType.System_Decimal)
                )
                {
                    name = $"{name}300";
                }
                if (namedType.SpecialType == SpecialType.System_String && stringIntern)
                {
                    name = "InternedString";
                }

                return $"global::LightProto.Parser.{name}ProtoParser.Proto{readerOrWriter}";
            }

            if (typeArguments.Length == 1)
            {
                if (
                    namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
                    || namedType.OriginalDefinition.ToDisplayString() == "System.Lazy<T>"
                )
                {
                    var elementType = typeArguments[0];
                    var elementParser = GetProtoParser(
                        compilation,
                        elementType,
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
                    return $"new global::LightProto.Parser.{memberType.Name}Proto{readerOrWriter}<{elementType}>({elementParser})";
                }
                else
                {
                    if (rawTag == 0)
                    {
                        throw new Exception("does not support collection of collection.");
                    }

                    var elementType = typeArguments[0];
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

                    if (!isPacked)
                    {
                        rawTag = ProtoMember.GetRawTag(fieldNumber, ProtoMember.GetPbWireType(compilation, elementType, format));
                    }
                    if (namedType.TypeKind == TypeKind.Interface)
                    {
                        if (readerOrWriter == "Reader")
                        {
                            if (IsListType(compilation, namedType))
                            {
                                return $"new global::LightProto.Parser.ListProto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize})";
                            }

                            if (IsSetType(compilation, namedType))
                            {
                                return $"new global::LightProto.Parser.HashSetProto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize})";
                            }
                        }
                        else if (readerOrWriter == "Writer")
                        {
                            if (IsCollectionType(compilation, elementType, namedType))
                            {
                                string count;
                                if (HasCountProperty(memberType))
                                    count = "Count";
                                else if (HasLengthProperty(memberType))
                                    count = "Length";
                                else
                                    count = "Count()";
                                return $"new global::LightProto.Parser.IEnumerableProto{readerOrWriter}<{memberType},{elementType}>({elementParser},{rawTag},static (d)=>d.{count},{fixedSize})";
                            }
                        }
                    }

                    if (namedType.TypeKind == TypeKind.Class || namedType.TypeKind == TypeKind.Struct)
                    {
                        return $"new global::LightProto.Parser.{memberType.Name}Proto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize})";
                    }
                }
            }

            if (typeArguments.Length == 2)
            {
                if (rawTag == 0)
                {
                    throw new Exception("rawTag==0");
                }

                var keyType = typeArguments[0];
                var keyTag = ProtoMember.GetRawTag(1, ProtoMember.GetPbWireType(compilation, keyType, mapFormat.keyFormat));
                var keyWriter = GetProtoParser(
                    compilation,
                    ProtoContract.GetProxyType(keyType) ?? keyType,
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
                var valueTag = ProtoMember.GetRawTag(2, ProtoMember.GetPbWireType(compilation, valueType, mapFormat.valueFormat));
                var valueWriter = GetProtoParser(
                    compilation,
                    ProtoContract.GetProxyType(valueType) ?? valueType,
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
                            return $"new global::LightProto.Parser.DictionaryProto{readerOrWriter}<{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag})";
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
                            return $"new global::LightProto.Parser.IEnumerableKeyValuePairProto{readerOrWriter}<{memberType},{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag},static (d)=>d.{count})";
                        }
                    }
                }

                if (namedType.TypeKind is TypeKind.Class or TypeKind.Struct)
                {
                    return $"new global::LightProto.Parser.{memberType.Name}Proto{readerOrWriter}<{keyType},{valueType}>({keyWriter},{valueWriter},{rawTag})";
                }
            }
        }

        throw LightProtoGeneratorException.Member_Type_Not_Supported(member);
    }

    public static IDisposable GenerateNestedClassStructure(CodeWriter writer, INamedTypeSymbol targetType)
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
        StackDisposable disposable = new StackDisposable();
        for (int i = containers.Count - 1; i >= 0; i--)
        {
            var container = containers[i];
            writer.WriteLine($"partial class {container.Name}");
            writer.IndentScope().AddTo(disposable);
        }

        return disposable;
    }

    public static bool SupportsPackedEncoding(Compilation compilation, ITypeSymbol? itemType)
    {
        if (itemType is null)
        {
            return false;
        }

        return itemType.SpecialType
            is SpecialType.System_Boolean
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Int32
                or SpecialType.System_UInt32
                or SpecialType.System_Int64
                or SpecialType.System_UInt64
                or SpecialType.System_Byte
                or SpecialType.System_SByte
                or SpecialType.System_Single
                or SpecialType.System_Double
                or SpecialType.System_Char
                or SpecialType.System_Enum;
    }

    internal static bool SupportFixedSize(ITypeSymbol type) =>
        type.SpecialType
            is SpecialType.System_SByte
                or SpecialType.System_Int32
                or SpecialType.System_Int16
                or SpecialType.System_Int64
                or SpecialType.System_Byte
                or SpecialType.System_UInt16
                or SpecialType.System_UInt32
                or SpecialType.System_UInt64;

    internal static bool SupportZigZag(ITypeSymbol type) =>
        type.SpecialType is SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_Int32 or SpecialType.System_Int64;
}
