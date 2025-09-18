namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ArrayEquivalentTypeTests
    : BaseEquivalentTypeTests<ArrayEquivalentTypeTests.Message, int[]>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public int[] Values { get; set; } = Array.Empty<int>();
    }

    public override IEnumerable<Message> GetLightProtoMessages()
    {
        yield return new() { Values = new[] { 1, 2, 3 } };
        yield return new() { Values = Array.Empty<int>() };
        yield return new() { Values = new[] { 0 } };
        yield return new() { Values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } };
    }

    public override IEnumerable<int[]> GetProtoNetMessages()
    {
        yield return new[] { 1, 2, 3 };
        yield return Array.Empty<int>();
        yield return new[] { 0 };
        yield return new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }

    public override async Task AssertResult(
        Message lightProto,
        int[] protobuf,
        bool lightProtoToProtoBuf
    )
    {
        await Assert.That(lightProto.Values).IsEquivalentTo(protobuf);
    }
}
