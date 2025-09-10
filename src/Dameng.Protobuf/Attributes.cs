namespace Dameng.Protobuf;

public class ProtoContractAttribute : Attribute;

public class ProtoMemberAttribute(uint tag) : Attribute
{
    public uint Tag { get; } = tag;
    public DataFormat DataFormat { get; set; } = DataFormat.Default;
    [Obsolete("compatibility only, no effect")]
    public bool IsRequired { get; set; } = false;
    [Obsolete("compatibility only, no effect")]
    public bool IsPacked { get; set; } = false;
    [Obsolete("compatibility only, no effect")]
    public bool OverwriteList { get; set; } = false;
    [Obsolete("compatibility only, no effect")]
    public string Name { get; set; } = string.Empty;
}

public class ProtoMapAttribute : Attribute
{
    public DataFormat KeyFormat { get; set; } = DataFormat.Default;
    public DataFormat ValueFormat { get; set; } = DataFormat.Default;
}

public class ProtoIgnoreAttribute : Attribute;

public class ProtoParserAttribute<T> : Attribute;