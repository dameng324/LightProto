using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MemoryBytesTests : BaseTests<MemoryBytesTests.Message, ByteArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Memory<byte> Property { get; set; } = Memory<byte>.Empty;

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property.ToArray())}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new byte[] { 1, 2, 3, 4, 5 } };
        yield return new() { Property = new byte[] { 0, 0, 0, 0, 0 } };
        yield return new() { Property = new byte[] { 0 } };
        yield return new() { Property = Memory<byte>.Empty };
    }

    public override IEnumerable<ByteArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ByteArrayTestsMessage() { Property = ByteString.CopyFrom(o.Property.ToArray()) });
    }

    public override async Task AssertGoogleResult(ByteArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToByteArray()).IsEquivalentTo(message.Property.ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(bytes, new MemoryProtoReader<byte>(ByteProtoParser.ProtoReader, 0));
        await Assert.That(deserialized.IsEmpty).IsTrue();
    }
}
