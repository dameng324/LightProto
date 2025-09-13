using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class MapTests: BaseTests<MapTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Dictionary<int, string> Property { get; set; } = [];

        public override string ToString()
        {
            return string.Join(",", Property.Select(x => $"{x.Key}:{x.Value}"));
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = new Dictionary<int, string>() };
        yield return new Message() { Property = new  Dictionary<int, string>()
        {
            [1] = "one",
            [2] = "two",
            [3] = "three"
        } };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}