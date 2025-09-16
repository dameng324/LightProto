using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ReadonlyListPropertyTests
    : BaseTests<ReadonlyListPropertyTests.Message, ListUnPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public List<int> Property { get; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = { 1, 2, 3, 4, 5 } };
        yield return new() { Property = { -1, -2, -3, -4, -5 } };
        yield return new() { Property = { 0 } };
        yield return new() { Property = { } };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ListUnPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ListUnPackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ListUnPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
