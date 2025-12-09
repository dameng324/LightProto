namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class LinkedListTests : BaseTests<LinkedListTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public LinkedList<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new LinkedList<int>([1, 2, 3, 4, 5]) };
        yield return new() { Property = new LinkedList<int>([-1, -2, -3, -4, -5]) };
        yield return new() { Property = new LinkedList<int>([0, 0, 0, 0, 0]) };
        yield return new() { Property = new LinkedList<int>([0]) };
        yield return new() { Property = new LinkedList<int>([]) };
    }

    public override IEnumerable<ArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ArrayTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
