namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class IntEquivalentTypeTests : BaseEquivalentTypeTests<IntEquivalentTypeTests.Message, int>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public int Property { get; set; }
    }

    public override IEnumerable<Message> GetLightProtoMessages()
    {
        yield return new() { Property = 0 };
        yield return new() { Property = 10 };
        yield return new() { Property = int.MaxValue };
        yield return new() { Property = int.MinValue };
    }

    public override IEnumerable<int> GetProtoNetMessages()
    {
        yield return 0;
        yield return 10;
        yield return int.MaxValue;
        yield return int.MinValue;
    }

    public override async Task AssertResult(Message lightProto, int protobuf, bool lightProtoToProtoBuf)
    {
        await Assert.That(lightProto.Property).IsEquivalentTo(protobuf);
    }
}
