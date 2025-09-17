using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InternStringTests
    : BaseEquivalentTypeTests<
        InternStringTests.LightProtoMessage,
        InternStringTests.ProtoBufMessage
    >
{
    [ProtoContract]
    public partial struct LightProtoMessage
    {
        [ProtoMember(1)]
        [StringIntern]
        public string Property;
    }

    [ProtoBuf.ProtoContract]
    public partial struct ProtoBufMessage
    {
        [ProtoBuf.ProtoMember(1)]
        public string Property;
    }

    public override IEnumerable<LightProtoMessage> GetLightProtoMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = string.Intern(Guid.NewGuid().ToString("N")) };
    }

    public override IEnumerable<ProtoBufMessage> GetProtoNetMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(LightProtoMessage clone, ProtoBufMessage message)
    {
        await Assert.That(String.IsInterned(clone.Property)).IsEquivalentTo(clone.Property);
    }
}
