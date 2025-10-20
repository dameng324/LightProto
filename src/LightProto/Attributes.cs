using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace LightProto;

[ExcludeFromCodeCoverage]
[AttributeUsage(
    AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Interface
        | AttributeTargets.Enum
)]
public class ProtoContractAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the defined name of the type. This can be fully qualified , for example <c>.foo.bar.someType</c> if required.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// If true, the constructor for the type is bypassed during deserialization, meaning any field initializers
    /// or other initialization code is skipped.
    /// </summary>
    public bool SkipConstructor { get; set; } = false;

    /// <summary>
    /// Gets or sets the mechanism used to automatically infer field tags
    /// for members. This option should be used in advanced scenarios only.
    /// Please review the important notes against the ImplicitFields enumeration.
    /// </summary>
    public ImplicitFields ImplicitFields { get; set; }

    /// <summary>
    /// Gets or sets the first offset to use with implicit field tags;
    /// only used if ImplicitFields is set.
    /// </summary>
    public uint ImplicitFirstTag { get; set; } = 1;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[ExcludeFromCodeCoverage]
public class ProtoMemberAttribute(uint tag) : Attribute
{
    public uint Tag { get; } = tag;
    public DataFormat DataFormat { get; set; } = DataFormat.Default;

    /// <summary>
    /// If true, the member must be present when deserializing; if it is not, an exception will be thrown.
    /// </summary>
    public bool IsRequired { get; set; } = false;
    public bool IsPacked { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public Type? ParserType { get; set; } = null;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ProtoMapAttribute : Attribute
{
    public DataFormat KeyFormat { get; set; } = DataFormat.Default;
    public DataFormat ValueFormat { get; set; } = DataFormat.Default;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ProtoIgnoreAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ProtoSurrogateForAttribute<T> : Attribute;

// donot support ProtoInclude for now
// [Obsolete("compatibility protobuf-net only, no effect")]
// public class ProtoIncludeAttribute(Type type, uint tag) : Attribute
// {
//     public Type Type { get; } = type;
//     public uint Tag { get; } = tag;
// }

/// <summary>
/// Defines the compatibiltiy level to use for an element
/// </summary>
[AttributeUsage(
    AttributeTargets.Assembly
        | AttributeTargets.Module
        | AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Field
        | AttributeTargets.Property
)]
[ExcludeFromCodeCoverage]
public sealed class CompatibilityLevelAttribute(CompatibilityLevel level) : Attribute
{
    /// <summary>
    /// The compatibiltiy level to use for this element
    /// </summary>
    public CompatibilityLevel Level { get; } = level;
}

[AttributeUsage(
    AttributeTargets.Assembly
        | AttributeTargets.Module
        | AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Field
        | AttributeTargets.Property
)]
[ExcludeFromCodeCoverage]
public sealed class StringInternAttribute : Attribute;

/// <summary>
/// Indicates the known-types to support for an individual
/// message. This serializes each level in the hierarchy as
/// a nested message to retain wire-compatibility with
/// other protocol-buffer implementations.
/// <param name="tag">The unique index (within the type) that will identify this data.</param>
/// <param name="knownType">The additional type to serialize/deserialize.</param>
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Interface,
    AllowMultiple = true,
    Inherited = false
)]
[ExcludeFromCodeCoverage]
public sealed class ProtoIncludeAttribute(uint tag, Type knownType) : Attribute
{
    public uint Tag { get; } = tag;
    public Type KnownType { get; } = knownType;
}

/// <summary>
/// Specifies a parser type to use for serializing/deserializing the target type.
/// <param name="parserType">The type that implements the parser logic for serialization/deserialization.</param>
/// </summary>
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ProtoParserTypeAttribute(Type parserType) : Attribute
{
    public Type ParserType { get; } = parserType;
}

/// <summary>
/// Specifies the ProtoParser type to use for a given message type for serialization/deserialization.
/// <param name="messageType">The message type to be serialized/deserialized.</param>
/// <param name="parserType">The parser type for serialization/deserialization.</param>
/// </summary>
[ExcludeFromCodeCoverage]
[AttributeUsage(
    AttributeTargets.Assembly
        | AttributeTargets.Module
        | AttributeTargets.Class
        | AttributeTargets.Struct,
    AllowMultiple = true
)]
public sealed class ProtoParserTypeMapAttribute(Type messageType, Type parserType) : Attribute
{
    public Type MessageType { get; } = messageType;
    public Type ParserType { get; } = parserType;
}
