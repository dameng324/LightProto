using Google.Protobuf.Collections;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Map4Tests : BaseTests<Map4Tests.Message, Map3TestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Dictionary<int[], long[]> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {Property.Count}";
        }
    }

    [Test]
    [Explicit]
    public void GetProto()
    {
        var proto = ProtoBuf.Serializer.GetProto<Message>();
        Console.WriteLine(proto);
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new() };
        yield return new()
        {
            Property = new() { [[1, 2]] = [11, 22], [[3, 4, 5]] = [33, 44, 55] },
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Keys.ToArray()).IsEquivalentTo(message.Property.Keys.ToArray());
        foreach (var kv in message.Property)
        {
            await Assert.That(clone.Property.FirstOrDefault(o => o.Key.SequenceEqual(kv.Key)).Value).IsEquivalentTo(kv.Value);
        }
    }

    public override IEnumerable<Map3TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o =>
            {
                var Map3TestsMessage = new Map3TestsMessage();

                foreach (var kv in o.Property)
                {
                    Map3TestsMessage.Property.Add(new Map3NestMessage() { Key = { kv.Key }, Value = { kv.Value } });
                }

                return Map3TestsMessage;
            });
    }

    public override async Task AssertGoogleResult(Map3TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.Count).IsEqualTo(message.Property.Count);
        foreach (var kv in message.Property)
        {
            await Assert.That(clone.Property.First(o => o.Key.SequenceEqual(kv.Key)).Value).IsEquivalentTo(kv.Value);
        }
    }
}
