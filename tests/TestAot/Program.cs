using MemoryPack;
using ProtoBuf;

var proto = ProtoBuf.Serializer.GetProto<TestMessage>();
Console.WriteLine(proto);

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[MemoryPackable]
public partial class TestMessage
{
    [ProtoMember(1)]
    [CompatibilityLevel(CompatibilityLevel.Level300)]
    public DateTime Id { get; set; }
}
