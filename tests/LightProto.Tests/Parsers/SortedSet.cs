using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class SortedSetPackedTests
    : BaseTests<SortedSetPackedTests.Message, ISetPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required SortedSet<int> Property { get; set; } = new();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new()
        {
            Property = new SortedSet<int>() { 1, 2, 3, 4, 5 },
        };
        yield return new()
        {
            Property = new SortedSet<int>() { -1, -2, -3, -4, -5 },
        };
        yield return new() { Property = new SortedSet<int>() { 0 } };
        yield return new() { Property = new SortedSet<int>() { } };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That((ISet<int>)clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ISetPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ISetPackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ISetPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
