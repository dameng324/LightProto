namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MapEquivalentTypeTests
    : BaseEquivalentTypeTests<MapEquivalentTypeTests.Message, Dictionary<int, string>>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Dictionary<int, string> Property { get; set; } = [];
    }

    public override IEnumerable<Message> GetLightProtoMessages()
    {
        yield return new() { Property = new Dictionary<int, string>() };
        yield return new()
        {
            Property = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
                [3] = "three",
            },
        };
    }

    public override IEnumerable<Dictionary<int, string>> GetProtoNetMessages()
    {
        yield return new Dictionary<int, string>();
        yield return new Dictionary<int, string>()
        {
            [1] = "one",
            [2] = "two",
            [3] = "three",
        };
    }

    public override async Task AssertResult(
        Message lightProto,
        Dictionary<int, string> protobuf,
        bool lightProtoToProtoBuf
    )
    {
        await Assert.That(lightProto.Property).IsEquivalentTo(protobuf);
    }
}
