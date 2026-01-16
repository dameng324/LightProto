using System.Collections.Immutable;
using System.Collections.ObjectModel;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ImmutableDictionaryTests : BaseTests<ImmutableDictionaryTests.Message, MapTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ImmutableDictionary<int, string> Property { get; set; } =
            System.Collections.Immutable.ImmutableDictionary<int, string>.Empty;

        public override string ToString()
        {
            return string.Join(",", Property.Select(x => $"{x.Key}:{x.Value}"));
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = ImmutableDictionary<int, string>.Empty };
        yield return new()
        {
            Property = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two",
                [3] = "three",
            }.ToImmutableDictionary(),
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

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(
            bytes,
            new ImmutableDictionaryProtoReader<int, string>(Int32ProtoParser.ProtoReader, StringProtoParser.ProtoReader, 0)
        );
        await Assert.That(deserialized.Count).IsEqualTo(0);
    }
}
