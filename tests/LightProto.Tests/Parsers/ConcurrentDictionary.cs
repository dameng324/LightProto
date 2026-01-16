using System.Collections.Concurrent;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ConcurrentDictionaryTests : BaseTests<ConcurrentDictionaryTests.Message, MapTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ConcurrentDictionary<int, string> Property { get; set; } = [];

        public override string ToString()
        {
            return string.Join(",", Property.Select(x => $"{x.Key}:{x.Value}"));
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new ConcurrentDictionary<int, string>() };
        yield return new()
        {
            Property = new ConcurrentDictionary<int, string>()
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
