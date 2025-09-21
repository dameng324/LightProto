using Google.Protobuf;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ByteArrayTests : BaseTests<ByteArrayTests.Message, ByteArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public byte[] Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Byte[] { 1, 2, 3, 4, 5 } };
        yield return new() { Property = new Byte[] { 0, 0, 0, 0, 0 } };
        yield return new() { Property = new Byte[] { 0 } };
        yield return new() { Property = [] };
    }

    public override IEnumerable<ByteArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ByteArrayTestsMessage()
            {
                Property = ByteString.CopyFrom(o.Property),
            });
    }

    public override async Task AssertGoogleResult(ByteArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToByteArray()).IsEquivalentTo(message.Property);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
