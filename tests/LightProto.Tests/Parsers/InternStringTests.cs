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

    public override async Task AssertResult(
        LightProtoMessage lightProto,
        ProtoBufMessage protobuf,
        bool lightProtoToProtoBuf
    )
    {
        await Assert
            .That(String.IsInterned(lightProto.Property))
            .IsEquivalentTo(lightProto.Property);
    }
}
