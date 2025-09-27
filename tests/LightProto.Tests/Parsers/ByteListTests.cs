using Google.Protobuf;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ByteListTests
    : BaseGoogleProtobufTests<ByteListTests.Message, ByteArrayTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public List<byte> Property { get; set; } = [];

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
            Property = new() { 0, 0, 0, 0, 0 },
        };
        yield return new() { Property = new() { 0 } };
        yield return new() { Property = [] };
    }

    public override IEnumerable<ByteArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ByteArrayTestsMessage()
            {
                Property = ByteString.CopyFrom(o.Property.ToArray()),
            });
    }

    public override async Task AssertGoogleResult(ByteArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToByteArray()).IsEquivalentTo(message.Property);
    }
}
