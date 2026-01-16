using System.Collections.Concurrent;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ConcurrentQueuePackedTests : BaseTests<ConcurrentQueuePackedTests.Message, ConcurrentQueuePackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ConcurrentQueue<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new([1, 2, 3, 4, 5]) };
        yield return new() { Property = new([-1, -2, -3, -4, -5]) };
        yield return new() { Property = new([0]) };
        yield return new() { Property = new() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ConcurrentQueuePackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ConcurrentQueuePackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ConcurrentQueuePackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
