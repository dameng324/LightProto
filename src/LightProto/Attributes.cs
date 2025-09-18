using System.ComponentModel;

namespace LightProto;

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
    /// Gets or sets the fist offset to use with implicit field tags;
    /// only uesd if ImplicitFields is set.
    /// </summary>
    public uint ImplicitFirstTag { get; set; } = 1;
}

public class ProtoMemberAttribute(uint tag) : Attribute
{
    public uint Tag { get; } = tag;
    public DataFormat DataFormat { get; set; } = DataFormat.Default;

    // [Obsolete("compatibility protobuf-net only, no effect")]
    // public bool IsRequired { get; set; } = false;
    public bool IsPacked { get; set; } = false;

    // [Obsolete("compatibility protobuf-net only, no effect")]
    // public bool OverwriteList { get; set; } = false;

    public string Name { get; set; } = string.Empty;
}

public class ProtoMapAttribute : Attribute
{
    public DataFormat KeyFormat { get; set; } = DataFormat.Default;
    public DataFormat ValueFormat { get; set; } = DataFormat.Default;
}

[Obsolete("compatibility protobuf-net only, no effect")]
public class ProtoIgnoreAttribute : Attribute;

public class ProtoProxyAttribute<T> : Attribute;

public class ProtoProxyForAttribute<T> : Attribute;

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
public sealed class StringInternAttribute : Attribute;
