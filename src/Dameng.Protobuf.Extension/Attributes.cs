namespace Dameng.Protobuf.Extension;

public class ProtoContractAttribute:Attribute;
public class ProtoMemberAttribute(uint tag):Attribute
{
    public uint Tag { get; } = tag;
    public DataFormat DataFormat { get; set; } = DataFormat.Default;
}
public class MapKeyDataFormatAttribute(DataFormat dataFormat):Attribute
{
    public DataFormat DataFormat { get; } = dataFormat;
}
public class MapValueDataFormatAttribute(DataFormat dataFormat):Attribute
{
    public DataFormat DataFormat { get; } = dataFormat;
}
