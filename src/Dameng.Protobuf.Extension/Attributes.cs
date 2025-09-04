namespace Dameng.Protobuf.Extension;

public class ProtoContractAttribute : Attribute;

public class ProtoMemberAttribute(uint tag) : Attribute
{
    public uint Tag { get; } = tag;
    public DataFormat DataFormat { get; set; } = DataFormat.Default;
}

public class ProtoMapAttribute : Attribute
{
    public DataFormat KeyFormat { get; set; } = DataFormat.Default;
    public DataFormat ValueFormat { get; set; } = DataFormat.Default;
}
