using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightProto.Generator;

internal class ProtoMember
{
    public enum PbWireType
    {
        Varint = 0, // int32, int64, bool, enum ,uint32, uint64
        Fixed64 = 1, // double, fixed64, sfixed64
        LengthDelimited = 2, // string, bytes, message, repeated packed
        Fixed32 = 5, // float, fixed32, sfixed32
    }

    public Compilation Compilation { get; set; } = null!;
    public ITypeSymbol ContractType { get; set; } = null!;
    public ITypeSymbol? ProxyType { get; set; }
    public string Name { get; set; } = "";
    public ITypeSymbol Type { get; set; } = null!;
    public DataFormat DataFormat { get; set; }
    public (DataFormat keyFormat, DataFormat valueFormat) MapFormat { get; set; }
    public uint FieldNumber { get; set; }
    public bool IsRequired { get; set; }

    /// <summary>
    /// When deserializing, if the field is required and the type is a reference type or nullable value type,
    /// we need to check if the value is null and throw an exception if it is.
    /// This is to prevent null reference exceptions when accessing the field later.
    /// </summary>
    public bool IsProtoMemberRequired { get; set; }
    public bool IsInitOnlyOrRequired => IsInitOnly || IsRequired;
    public bool IsInitOnly { get; set; }
    public string Initializer { get; set; } = "default";
    public bool StringIntern { get; set; }

    public ImmutableArray<AttributeData> AttributeData { get; set; } = ImmutableArray<AttributeData>.Empty;

    public PbWireType WireType => GetPbWireType(Compilation, Type, DataFormat);

    public static PbWireType GetPbWireType(Compilation compilation, ITypeSymbol Type, DataFormat DataFormat)
    {
        // Handle nullable value types by getting the underlying type
        if (Type is INamedTypeSymbol namedType && (Helper.IsNullableType(Type) || Helper.IsLazyType(Type)))
        {
            return GetPbWireType(compilation, namedType.TypeArguments[0], DataFormat);
        }

        // Handle arrays, lists and sets
        if (Helper.IsCollectionType(compilation, Type))
        {
            return PbWireType.LengthDelimited;
        }

        // Handle dictionaries
        if (Helper.IsDictionaryType(compilation, Type))
        {
            return PbWireType.LengthDelimited;
        }

        if (Type.TypeKind == TypeKind.Enum)
        {
            return PbWireType.Varint;
        }

        switch (Type.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            {
                return DataFormat == DataFormat.FixedSize ? PbWireType.Fixed32 : PbWireType.Varint;
            }
            case SpecialType.System_Char:
            {
                return PbWireType.Varint;
            }
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            {
                return DataFormat == DataFormat.FixedSize ? PbWireType.Fixed64 : PbWireType.Varint;
            }
            case SpecialType.System_Boolean:
            case SpecialType.System_Enum:
                return PbWireType.Varint;
            case SpecialType.System_Single:
            case SpecialType.None when Helper.IsHalfType(Type): // float16
                return PbWireType.Fixed32;
            case SpecialType.System_Double:
                return PbWireType.Fixed64;
            case SpecialType.None when Helper.IsDateOnlyType(Type): //int32
                return PbWireType.Varint;
            case SpecialType.None when Helper.IsTimeOnlyType(Type): //int64
                return PbWireType.Varint;
            case SpecialType.None when Helper.IsRuneType(Type): //uint32
                return PbWireType.Varint;
            case SpecialType.System_String:
            case SpecialType.None when Helper.IsGuidType(Type):
            case SpecialType.None when Helper.IsStringBuilderType(Type):
            case SpecialType.None
                when Type.TypeKind == TypeKind.Class || Type.TypeKind == TypeKind.Interface || Type.TypeKind == TypeKind.Array:
                return PbWireType.LengthDelimited;
            default:
                // Default to LengthDelimited for other complex types
                return PbWireType.LengthDelimited;
        }
    }

    public uint RawTag => GetRawTag(FieldNumber, WireType);
    public int RawTagSize => GetRawTagSize(RawTag);
    public bool IsPacked { get; set; }
    public CompatibilityLevel CompatibilityLevel { get; set; }
    public bool IsReadOnly { get; set; }
    public MemberDeclarationSyntax DeclarationSyntax { get; set; } = null!;

    public static uint GetRawTag(uint fieldNumber, PbWireType WireType)
    {
        return (fieldNumber << 3) | (uint)WireType;
    }

    public static int GetRawTagSize(uint value)
    {
        if ((value & (0xffffffff << 7)) == 0)
        {
            return 1;
        }
        if ((value & (0xffffffff << 14)) == 0)
        {
            return 2;
        }
        if ((value & (0xffffffff << 21)) == 0)
        {
            return 3;
        }
        if ((value & (0xffffffff << 28)) == 0)
        {
            return 4;
        }
        return 5;
    }

    public string GetCheckIfNotEmpty(string messageName)
    {
        if (IsProtoMemberRequired)
            return "true";

        if (Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return $"{messageName}.{Name}.HasValue";
        }

        if (Type.SpecialType == SpecialType.System_DateTime)
        {
            return "true";
        }

        if (CompatibilityLevel >= CompatibilityLevel.Level300 && (Helper.IsGuidType(Type) || Helper.IsDecimalType(Type)))
        {
            return "true";
        }

        if (Type.IsValueType)
        {
            return $"{messageName}.{Name} != default";
        }

        var check = $"{messageName}.{Name} != null";

        if (Helper.IsCollectionType(Compilation, Type) || Helper.IsDictionaryType(Compilation, Type))
        {
            if (Helper.HasCountProperty(Type))
            {
                return $"{check} && {messageName}.{Name}.Count > 0";
            }
            if (Helper.HasLengthProperty(Type))
            {
                return $"{check} && {messageName}.{Name}.Length > 0";
            }
        }

        return check;
    }
}
