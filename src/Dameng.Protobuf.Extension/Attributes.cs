namespace Dameng.Protobuf.Extension;

public class ProtoContractAttribute:Attribute;
public class ProtoMemberAttribute(int tag):Attribute
{
    public int Tag { get; } = tag;
}