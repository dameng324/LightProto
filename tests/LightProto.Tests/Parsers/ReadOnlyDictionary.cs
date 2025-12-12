using System.Collections.ObjectModel;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ReadOnlyMapTests : BaseTests<ReadOnlyMapTests.Message, MapTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required ReadOnlyDictionary<int, string> Property { get; set; } =
            new(new Dictionary<int, string>());

        public override string ToString()
        {
            return string.Join(",", Property.Select(x => $"{x.Key}:{x.Value}"));
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new()
        {
            Property = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()),
        };
        yield return new()
        {
            Property = new ReadOnlyDictionary<int, string>(
                new Dictionary<int, string>()
                {
                    [1] = "one",
                    [2] = "two",
                    [3] = "three",
                }
            ),
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
            new ReadOnlyDictionaryProtoReader<int, string>(
                Int32ProtoParser.ProtoReader,
                StringProtoParser.ProtoReader,
                0
            )
        );
        await Assert.That(deserialized.Count).IsEqualTo(0);
    }
}
