using System.Collections.Concurrent;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ReadonlySortedListPropertyTests
    : BaseTests<ReadonlySortedListPropertyTests.Message, MapTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public SortedList<int, string> Property { get; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { };
        yield return new()
        {
            Property =
            {
                [1] = "one",
                [2] = "two",
                [3] = "three",
            },
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<MapTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new MapTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(MapTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
