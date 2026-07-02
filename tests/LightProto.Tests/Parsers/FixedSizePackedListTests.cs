using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class FixedSizePackedListTests : BaseTests<FixedSizePackedListTests.Message, FixedSizeArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, DataFormat = ProtoBuf.DataFormat.FixedSize, IsPacked = true)]
        public List<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new()
        {
            Property = new() { 1, 2, 3, 4, 5 },
        };
        yield return new()
        {
            Property = new() { -1, -2, -3, -4, -5 },
        };
        yield return new()
        {
            Property = new() { 0, 0, 0, 0, 0 },
        };
        //yield return new() { Property = new int[] { 0 } }; // protobuf-net is wrong here
        yield return new() { Property = [] };
    }

    [Test]
    public async Task LightProto_Serialize_WritesPackedFixed32Bytes()
    {
        var bytes = new Message { Property = [1, -2] }.ToByteArray(Message.ProtoWriter);

        await Assert.That(bytes).IsEquivalentTo(new byte[] { 10, 8, 1, 0, 0, 0, 254, 255, 255, 255 });
    }

    [Test]
    public async Task LightProto_Deserialize_ReadOnlySequenceSplitInsideFixed32Values()
    {
        var bytes = new Message { Property = [1, -2, 3] }.ToByteArray(Message.ProtoWriter);
        var sequence = LightProto.Tests.SerializerTests.GetReadonlySequence(bytes.Chunk(1).ToArray());

        var parsed = Serializer.Deserialize(sequence, Message.ProtoReader);

        await Assert.That(parsed.Property).IsEquivalentTo(new[] { 1, -2, 3 });
    }

    public override IEnumerable<FixedSizeArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new FixedSizeArrayTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(FixedSizeArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
