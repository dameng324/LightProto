using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class Map2Tests: BaseTests<Map2Tests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Dictionary<int, Dictionary<int, string>> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {Property.Count}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = new () };
        yield return new Message() { Property = new  ()
        {
            [10] = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            },
            [2] =new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            },
            [3] = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            }
        } };
    }
    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}