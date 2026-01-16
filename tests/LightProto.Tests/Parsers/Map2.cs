using Google.Protobuf.Collections;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Map2Tests : BaseTests<Map2Tests.Message, Map2TestsMessage>
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
        yield return new() { Property = new() };
        yield return new()
        {
            Property = new()
            {
                [10] = new Dictionary<int, string>()
                {
                    [1] = "one",
                    [2] = "two",
                    [3] = "three",
                },
                [2] = new Dictionary<int, string>()
                {
                    [1] = "one",
                    [2] = "two",
                    [3] = "three",
                },
                [3] = new Dictionary<int, string>()
                {
                    [1] = "one",
                    [2] = "two",
                    [3] = "three",
                },
            },
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Keys.ToArray()).IsEquivalentTo(message.Property.Keys.ToArray());
        foreach (var kv in message.Property)
        {
            await Assert.That(clone.Property[kv.Key]).IsEquivalentTo(kv.Value);
        }
    }

    public override IEnumerable<Map2TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o =>
            {
                var map2TestsMessage = new Map2TestsMessage();

                foreach (var kv in o.Property)
                {
                    map2TestsMessage.Property.Add(new Map2NestMessage() { Key = kv.Key, Value = { kv.Value } });
                }

                return map2TestsMessage;
            });
    }

    public override async Task AssertGoogleResult(Map2TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.Count).IsEqualTo(message.Property.Count);
        foreach (var kv in message.Property)
        {
            await Assert.That(clone.Property.First(o => o.Key == kv.Key).Value).IsEquivalentTo(kv.Value);
        }
    }
}
