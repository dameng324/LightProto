using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ListPackedTests : BaseTests<ListPackedTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, IsPacked = true)]
        public List<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = [1, 2, 3, 4, 5] };
        yield return new Message() { Property = [-1, -2, -3, -4, -5] };
        yield return new Message() { Property = [0, 0, 0, 0, 0] };
        // TODO:protobuf-net is wrong here  yield return new Message() { Property = [0] };
        yield return new Message() { Property = [] };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
