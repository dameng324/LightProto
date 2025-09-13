using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ArrayTests : BaseTests<ArrayTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public int[] Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = new int[] { 1, 2, 3, 4, 5 } };
        yield return new Message() { Property = new int[] { -1, -2, -3, -4, -5 } };
        yield return new Message() { Property = new int[] { 0, 0, 0, 0, 0 } };
        yield return new Message() { Property = new int[] { 0} };
        yield return new Message() { Property = [] };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
