using System.ComponentModel;

namespace LightProto;

public class ProtoContractAttribute : Attribute
{
    public bool SkipConstructor { get; set; } = false;
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
