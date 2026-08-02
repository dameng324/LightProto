using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace LightProto.Generator;

internal static class Helper
{
    internal readonly struct InlineArrayInfo
    {
        public InlineArrayInfo(INamedTypeSymbol type, ITypeSymbol elementType, int length)
        {
            Type = type;
            ElementType = elementType;
            Length = length;
        }

        public INamedTypeSymbol Type { get; }

        public ITypeSymbol ElementType { get; }

        public int Length { get; }
    }

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

        return HasProperty(type, "Length", SpecialType.System_Int32) || HasProperty(type, "Length", SpecialType.System_Int64);
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
        if (TryGetInlineArrayInfo(type, out var inlineArrayInfo))
        {
            itemType = inlineArrayInfo.ElementType;
            return true;
        }

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

        if (IsNullableType(namedType))
            return false;

        if (IsLazyType(namedType))
            return false;

        itemType = namedType.TypeArguments[0];
        return IsCollectionType(compilation, itemType, namedType);
    }

    internal static bool IsCollectionType(Compilation compilation, ITypeSymbol elementType, ITypeSymbol type)
    {
        if (elementType.SpecialType == SpecialType.System_Byte && IsArrayType(type))
            return false;
        if (IsReadOnlyMemoryType(type) || IsMemoryType(type) || IsReadOnlySequenceType(type))
            return true;
        var baseCollectionType = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?.Construct(elementType)!;
        var conversion = compilation.ClassifyConversion(type, baseCollectionType);
        return conversion.IsImplicit;
    }

    internal static bool IsReadOnlyMemoryType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        return namedType.OriginalDefinition.ToDisplayString() == "System.ReadOnlyMemory<T>";
    }

    internal static bool IsMemoryType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        return namedType.OriginalDefinition.ToDisplayString() == "System.Memory<T>";
    }

    internal static bool IsReadOnlySequenceType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        return namedType.OriginalDefinition.ToDisplayString() == "System.Buffers.ReadOnlySequence<T>";
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
        if (TryGetInlineArrayInfo(collectionType, out var inlineArrayInfo))
        {
            return inlineArrayInfo.ElementType;
        }

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

    internal static bool TryGetInlineArrayInfo(ITypeSymbol type, out InlineArrayInfo info)
    {
        info = default;

        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var inlineArrayAttribute = namedType
            .GetAttributes()
            .FirstOrDefault(attribute =>
                attribute.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.InlineArrayAttribute"
            );
        if (
            inlineArrayAttribute is null
            || inlineArrayAttribute.ConstructorArguments.Length != 1
            || inlineArrayAttribute.ConstructorArguments[0].Value is not int length
            || length <= 0
        )
        {
            return false;
        }

        var elementType = GetInlineArrayElementType(namedType);
        if (elementType is null)
        {
            return false;
        }

        info = new InlineArrayInfo(namedType, elementType, length);
        return true;
    }

    internal static bool IsInlineArrayType(ITypeSymbol type) => TryGetInlineArrayInfo(type, out _);

    private static ITypeSymbol? GetInlineArrayElementType(INamedTypeSymbol type)
    {
        var field = type.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(field => !field.IsStatic && !field.IsConst);
        if (field is null)
        {
            return null;
        }

        if (field.Type is ITypeParameterSymbol typeParameter && type.TypeParameters.Length == type.TypeArguments.Length)
        {
            for (var index = 0; index < type.TypeParameters.Length; index++)
            {
                if (SymbolEqualityComparer.Default.Equals(type.TypeParameters[index], typeParameter))
                {
                    return type.TypeArguments[index];
                }
            }
        }

        return field.Type;
    }

    internal static bool IsArrayType(ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Array;
    }

    internal static bool IsArrayImplicitType(Compilation compilation, ITypeSymbol type)
    {
        if (IsArrayType(type))
            return true;
        if (type is not INamedTypeSymbol namedType)
            return false;

        var typeArguments = namedType.TypeArguments;
        if (typeArguments.Length != 1)
        {
            return false;
        }

        return namedType.OriginalDefinition.ToDisplayString()
            is "System.Collections.Generic.IReadOnlyList<T>"
                or "System.Collections.Generic.IReadOnlyCollection<T>";
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

    internal static ITypeSymbol ResolveConcreteTypeSymbol(Compilation compilation, INamedTypeSymbol type)
    {
        // 如果是接口，就手动映射到具体类型
        var constructedFrom = type.OriginalDefinition.ToDisplayString();

        return constructedFrom switch
        {
            "System.Collections.Generic.IReadOnlyList<T>" or "System.Collections.Generic.IReadOnlyCollection<T>" =>
                compilation.CreateArrayTypeSymbol(type.TypeArguments[0]),
            "System.Collections.Generic.IList<T>" or "System.Collections.Generic.ICollection<T>" => compilation
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

    internal static bool TryGetRepeatedProtoParserTypes(
        ITypeSymbol memberType,
        ProtoMember member,
        ITypeSymbol targetType,
        out INamedTypeSymbol protoReaderType,
        out INamedTypeSymbol protoWriterType
    )
    {
        var attribute =
            GetRepeatedParserTypeAttribute(member.AttributeData)
            ?? GetRepeatedParserTypeMapAttribute(targetType.GetAttributes(), memberType)
            ?? GetRepeatedParserTypeMapAttribute(targetType.ContainingModule.GetAttributes(), memberType)
            ?? GetRepeatedParserTypeMapAttribute(targetType.ContainingAssembly.GetAttributes(), memberType)
            ?? GetRepeatedParserTypeAttribute(memberType.GetAttributes());

        if (attribute is null)
        {
            protoReaderType = null!;
            protoWriterType = null!;
            return false;
        }

        var readerIndex = attribute.AttributeClass?.ToDisplayString() == "LightProto.ProtoRepeatedParserTypeMapAttribute" ? 1 : 0;
        protoReaderType = ConstructRepeatedParserType(
            (INamedTypeSymbol)attribute.ConstructorArguments[readerIndex].Value!,
            GetElementType(member.Compilation, memberType)
        );
        protoWriterType = ConstructRepeatedParserType(
            (INamedTypeSymbol)attribute.ConstructorArguments[readerIndex + 1].Value!,
            GetElementType(member.Compilation, memberType)
        );
        return true;
    }

    private static AttributeData? GetRepeatedParserTypeAttribute(ImmutableArray<AttributeData> attributes) =>
        attributes.FirstOrDefault(attribute =>
            attribute.AttributeClass?.ToDisplayString() == "LightProto.ProtoRepeatedParserTypeAttribute"
        );

    private static AttributeData? GetRepeatedParserTypeMapAttribute(ImmutableArray<AttributeData> attributes, ITypeSymbol collectionType) =>
        attributes.FirstOrDefault(attribute =>
            attribute.AttributeClass?.ToDisplayString() == "LightProto.ProtoRepeatedParserTypeMapAttribute"
            && IsCollectionTypeMatch((ITypeSymbol)attribute.ConstructorArguments[0].Value!, collectionType)
        );

    private static bool IsCollectionTypeMatch(ITypeSymbol mappedType, ITypeSymbol collectionType) =>
        SymbolEqualityComparer.Default.Equals(mappedType, collectionType)
        || mappedType is INamedTypeSymbol mappedNamedType
            && collectionType is INamedTypeSymbol collectionNamedType
            && SymbolEqualityComparer.Default.Equals(mappedNamedType.OriginalDefinition, collectionNamedType.OriginalDefinition);

    private static INamedTypeSymbol ConstructRepeatedParserType(INamedTypeSymbol parserType, ITypeSymbol itemType) =>
        parserType.IsUnboundGenericType ? parserType.OriginalDefinition.Construct(itemType) : parserType;

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

    internal static string GetInlineArrayProtoParserTypeName(InlineArrayInfo info, string readerOrWriter, string? memberName = null)
    {
        var typeName = SanitizeIdentifier(info.Type.Name);
        var lengthText = info.Length.ToString();
        if (!typeName.Contains(lengthText))
        {
            typeName += lengthText;
        }

        if (info.Type.TypeArguments.Length > 0)
        {
            typeName += "Of" + string.Join("And", info.Type.TypeArguments.Select(GetTypeNameForIdentifier));
        }

        if (!string.IsNullOrWhiteSpace(memberName))
        {
            typeName = SanitizeIdentifier(memberName!) + typeName;
        }

        return $"{typeName}Proto{readerOrWriter}";
    }

    internal static void GenerateInlineArrayProtoParser(
        CodeWriter writer,
        InlineArrayInfo info,
        string readerOrWriter,
        string? memberName = null
    )
    {
        if (readerOrWriter == "Writer")
        {
            GenerateInlineArrayProtoWriter(writer, info, memberName);
        }
        else
        {
            GenerateInlineArrayProtoReader(writer, info, memberName);
        }
    }

    private static void GenerateInlineArrayProtoWriter(CodeWriter writer, InlineArrayInfo info, string? memberName)
    {
        var parserTypeName = GetInlineArrayProtoParserTypeName(info, "Writer", memberName);
        var inlineArrayType = info.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var elementType = info.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var itemSupportsPacked = SupportsPackedEncoding(info.ElementType).ToString().ToLowerInvariant();
        var shouldCheckNullItem = !info.ElementType.IsValueType || IsNullableType(info.ElementType);

        writer.WriteLine(
            $"private sealed class {parserTypeName}: IProtoWriter,IProtoWriter<{inlineArrayType}>,global::LightProto.Parser.ICollectionWriter"
        );
        using (writer.IndentScope())
        {
            writer.WriteLine($"int IProtoWriter.CalculateSize(object value) => CalculateSize(({inlineArrayType})value);");
            writer.WriteLine($"long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize(({inlineArrayType})value);");
            writer.WriteLine(
                $"void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, ({inlineArrayType})value);"
            );
            writer.WriteLine("public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;");
            writer.WriteLine("public bool IsMessage => false;");
            writer.WriteLine($"private const int Length = {info.Length};");
            writer.WriteLine($"private const bool ItemSupportsPacked = {itemSupportsPacked};");
            writer.WriteLine("WireFormat.WireType global::LightProto.Parser.ICollectionWriter.ItemWireType => ItemWriter.WireType;");
            writer.WriteLine($"private IProtoWriter<{elementType}> ItemWriter {{ get; }}");
            writer.WriteLine("public uint Tag { get; set; }");
            writer.WriteLine("private int ItemFixedSize { get; }");
            writer.WriteLine($"public {parserTypeName}(IProtoWriter<{elementType}> itemWriter, uint tag, int itemFixedSize)");
            using (writer.IndentScope())
            {
                writer.WriteLine("ItemWriter = itemWriter;");
                writer.WriteLine("Tag = tag;");
                writer.WriteLine("ItemFixedSize = itemFixedSize;");
            }

            writer.WriteLine($"private long GetAllItemSize({inlineArrayType} collection)");
            using (writer.IndentScope())
            {
                writer.WriteLine("long size = 0;");
                writer.WriteLine("for (var index = 0; index < Length; index++)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("var item = collection[index];");
                    if (shouldCheckNullItem)
                    {
                        writer.WriteLine("if (item is null)");
                        using (writer.IndentScope())
                        {
                            writer.WriteLine("throw new Exception(\"Sequence contained null element\");");
                        }
                    }
                    writer.WriteLine("size += ItemWriter.CalculateLongMessageSize(item);");
                }
                writer.WriteLine("return size;");
            }

            writer.WriteLine($"private long CalculatePackedDataSize({inlineArrayType} collection)");
            using (writer.IndentScope())
            {
                writer.WriteLine("return ItemFixedSize != 0 ? (long)ItemFixedSize * Length : GetAllItemSize(collection);");
            }

            writer.WriteLine(
                "[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]"
            );
            writer.WriteLine($"public int CalculateSize({inlineArrayType} value)");
            using (writer.IndentScope())
            {
                writer.WriteLine("var longSize = CalculateLongSize(value);");
                writer.WriteLine("if (longSize > int.MaxValue)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("throw new OverflowException(\"Calculated size exceeds Int32.MaxValue\");");
                }
                writer.WriteLine("return (int)longSize;");
            }

            writer.WriteLine(
                "[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]"
            );
            writer.WriteLine($"public long CalculateLongSize({inlineArrayType} value)");
            using (writer.IndentScope())
            {
                writer.WriteLine("if (IsPacked)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("var dataSize = CalculatePackedDataSize(value);");
                    writer.WriteLine(
                        "return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(dataSize) + dataSize;"
                    );
                }
                writer.WriteLine("return CodedOutputStream.ComputeRawVarint32Size(Tag) * Length + GetAllItemSize(value);");
            }

            writer.WriteLine(
                "private bool IsPacked => ItemSupportsPacked && WireFormat.GetTagWireType(Tag) == WireFormat.WireType.LengthDelimited;"
            );
            writer.WriteLine($"public void WriteTo(ref WriterContext output, {inlineArrayType} collection)");
            using (writer.IndentScope())
            {
                writer.WriteLine("if (IsPacked)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("long size = CalculatePackedDataSize(collection);");
                    writer.WriteLine("output.WriteTag(Tag);");
                    writer.WriteLine("output.WriteLongLength(size);");
                    writer.WriteLine(
                        "if (global::LightProto.PackedRepeatedOptimizer.TryWritePackedRepeatedFieldLittleEndian(ref output, global::System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref collection[0], Length), ItemFixedSize))"
                    );
                    using (writer.IndentScope())
                    {
                        writer.WriteLine("return;");
                    }
                    writer.WriteLine("for (var index = 0; index < Length; index++)");
                    using (writer.IndentScope())
                    {
                        writer.WriteLine("ItemWriter.WriteMessageTo(ref output, collection[index]);");
                    }
                    writer.WriteLine("return;");
                }

                writer.WriteLine("for (var index = 0; index < Length; index++)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("var item = collection[index];");
                    if (shouldCheckNullItem)
                    {
                        writer.WriteLine("if (item is null)");
                        using (writer.IndentScope())
                        {
                            writer.WriteLine("throw new Exception(\"Sequence contained null element\");");
                        }
                    }
                    writer.WriteLine("output.WriteTag(Tag);");
                    writer.WriteLine("ItemWriter.WriteMessageTo(ref output, item);");
                }
            }
        }
    }

    private static void GenerateInlineArrayProtoReader(CodeWriter writer, InlineArrayInfo info, string? memberName)
    {
        var parserTypeName = GetInlineArrayProtoParserTypeName(info, "Reader", memberName);
        var inlineArrayType = info.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var elementType = info.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        writer.WriteLine(
            $"private sealed class {parserTypeName}: global::LightProto.Parser.ICollectionReader<{inlineArrayType},{elementType}>"
        );
        using (writer.IndentScope())
        {
            writer.WriteLine($"object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);");
            writer.WriteLine("public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;");
            writer.WriteLine("public bool IsMessage => false;");
            writer.WriteLine($"private const int Length = {info.Length};");
            writer.WriteLine("public WireFormat.WireType ItemWireType => ItemReader.WireType;");
            writer.WriteLine("object global::LightProto.Parser.ICollectionReader.Empty => Empty;");
            writer.WriteLine($"public IProtoReader<{elementType}> ItemReader {{ get; }}");
            writer.WriteLine($"public {inlineArrayType} Empty => new {inlineArrayType}();");
            writer.WriteLine("private int ItemFixedSize { get; }");
            writer.WriteLine($"public {parserTypeName}(IProtoReader<{elementType}> itemReader, uint tag, int itemFixedSize)");
            using (writer.IndentScope())
            {
                writer.WriteLine("ItemReader = itemReader;");
                writer.WriteLine("ItemFixedSize = itemFixedSize;");
            }

            writer.WriteLine($"public {inlineArrayType} ParseFrom(ref ReaderContext input)");
            using (writer.IndentScope())
            {
                writer.WriteLine($"var collection = default({inlineArrayType});");
                writer.WriteLine(
                    "global::LightProto.PackedRepeatedOptimizer.ParseRepeatedFieldIntoSpan(ref input, ItemReader, global::System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref collection[0], Length), ItemFixedSize);"
                );
                writer.WriteLine("return collection;");
            }
        }
    }

    private static string GetTypeNameForIdentifier(ITypeSymbol type)
    {
        var name = type.SpecialType switch
        {
            SpecialType.System_Boolean => "Boolean",
            SpecialType.System_Byte => "Byte",
            SpecialType.System_SByte => "SByte",
            SpecialType.System_Int16 => "Int16",
            SpecialType.System_UInt16 => "UInt16",
            SpecialType.System_Int32 => "Int32",
            SpecialType.System_UInt32 => "UInt32",
            SpecialType.System_Int64 => "Int64",
            SpecialType.System_UInt64 => "UInt64",
            SpecialType.System_Single => "Single",
            SpecialType.System_Double => "Double",
            SpecialType.System_Char => "Char",
            SpecialType.System_String => "String",
            _ => type.Name,
        };

        if (type is INamedTypeSymbol { TypeArguments.Length: > 0 } namedType)
        {
            name += string.Concat(namedType.TypeArguments.Select(GetTypeNameForIdentifier));
        }

        return SanitizeIdentifier(name);
    }

    private static string SanitizeIdentifier(string value)
    {
        var chars = value.Where(ch => char.IsLetterOrDigit(ch) || ch == '_').ToArray();
        if (chars.Length == 0)
        {
            return "InlineArray";
        }

        var result = new string(chars);
        return char.IsDigit(result[0]) ? "_" + result : result;
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

        if (
            rawTag != 0
            && IsCollectionType(compilation, memberType)
            && TryGetRepeatedProtoParserTypes(memberType, member, targetType, out var protoReaderType, out var protoWriterType)
        )
        {
            var elementType = GetElementType(compilation, memberType);
            if (!isPacked)
            {
                rawTag = ProtoMember.GetRawTag(
                    fieldNumber: GetFieldNumber(rawTag),
                    ProtoMember.GetPbWireType(compilation, elementType, format)
                );
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
            var itemFixedSize = GetFixedSize(elementType, format);
            var parserType = readerOrWriter == "Reader" ? protoReaderType : protoWriterType;
            var parserTypeName = parserType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var constructorArguments =
                readerOrWriter == "Reader" ? $"{elementParser},{itemFixedSize}" : $"{elementParser},{rawTag},{itemFixedSize}";
            return $"new {parserTypeName}({constructorArguments})";
        }

        if (IsProtoBufMessage(memberType))
        {
            return memberType.TypeKind is TypeKind.Interface
                ? $"{memberType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}ProtoParser.Proto{readerOrWriter}"
                : $"{memberType.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Proto{readerOrWriter}";
        }

        var fieldNumber = GetFieldNumber(rawTag);
        if (TryGetInlineArrayInfo(memberType, out var inlineArrayInfo))
        {
            if (rawTag == 0)
            {
                throw new Exception("rawTag==0");
            }

            var elementType = inlineArrayInfo.ElementType;
            if (!isPacked)
            {
                rawTag = ProtoMember.GetRawTag(fieldNumber, ProtoMember.GetPbWireType(compilation, elementType, format));
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
            return $"new {GetInlineArrayProtoParserTypeName(inlineArrayInfo, readerOrWriter, member?.Name)}({elementParser},{rawTag},{fixedSize})";
        }

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
            return $"new global::LightProto.Parser.ArrayProto{readerOrWriter}<{elementType}>({elementWriter},{rawTag},{fixedSize})";
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
                if (IsNullableType(namedType) || IsLazyType(namedType))
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

                    if (
                        !isPacked
                        && !(
                            elementType.SpecialType == SpecialType.System_Byte
                            && (IsMemoryType(namedType) || IsReadOnlyMemoryType(namedType) || IsReadOnlySequenceType(namedType))
                        )
                    )
                    {
                        rawTag = ProtoMember.GetRawTag(fieldNumber, ProtoMember.GetPbWireType(compilation, elementType, format));
                    }
                    if (namedType.TypeKind == TypeKind.Interface)
                    {
                        if (readerOrWriter == "Reader")
                        {
                            if (IsArrayImplicitType(compilation, namedType))
                            {
                                return $"new global::LightProto.Parser.ArrayProto{readerOrWriter}<{elementType}>({elementParser},{rawTag},{fixedSize})";
                            }
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

    public static bool SupportsPackedEncoding(ITypeSymbol? itemType)
    {
        if (itemType is null)
        {
            return false;
        }
        if (IsNullableType(itemType) || IsLazyType(itemType))
        {
            return SupportsPackedEncoding(((INamedTypeSymbol)itemType).TypeArguments[0]);
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
            || itemType.TypeKind is TypeKind.Enum;
    }

    internal static bool SupportFixedSize(ITypeSymbol type)
    {
        if (IsNullableType(type) || IsLazyType(type))
        {
            return SupportFixedSize(((INamedTypeSymbol)type).TypeArguments[0]);
        }
        return type.SpecialType
            is SpecialType.System_SByte
                or SpecialType.System_Int32
                or SpecialType.System_Int16
                or SpecialType.System_Int64
                or SpecialType.System_Byte
                or SpecialType.System_UInt16
                or SpecialType.System_UInt32
                or SpecialType.System_UInt64;
    }

    internal static bool SupportZigZag(ITypeSymbol type)
    {
        if (IsNullableType(type) || IsLazyType(type))
        {
            return SupportZigZag(((INamedTypeSymbol)type).TypeArguments[0]);
        }

        return type.SpecialType
            is SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_Int32
                or SpecialType.System_Int64;
    }

    internal static bool IsLazyType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        return namedType.OriginalDefinition.ToDisplayString() == "System.Lazy<T>";
    }

    internal static bool IsNullableType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType)
            return false;

        return namedType.OriginalDefinition.SpecialType is SpecialType.System_Nullable_T;
    }

    public static bool SupportLevel240Types(ITypeSymbol type)
    {
        if (IsNullableType(type) || IsLazyType(type))
        {
            return SupportLevel240Types(((INamedTypeSymbol)type).TypeArguments[0]);
        }
        return type.SpecialType is SpecialType.System_DateTime || IsTimeSpanType(type);
    }

    public static bool SupportLevel300Types(ITypeSymbol type)
    {
        if (IsNullableType(type) || IsLazyType(type))
        {
            return SupportLevel300Types(((INamedTypeSymbol)type).TypeArguments[0]);
        }
        return type.SpecialType is SpecialType.System_Decimal || IsGuidType(type) || SupportLevel240Types(type);
    }
}
