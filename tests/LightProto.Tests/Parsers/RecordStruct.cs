using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class RecordStructTests
    : BaseTests<RecordStructTests.Message, RecordStructTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record struct Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override IEnumerable<RecordStructTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(RecordStructTestsMessage clone, Message message)
    {
        await Assert
            .That(clone.Property ?? string.Empty)
            .IsEquivalentTo(message.Property ?? string.Empty);
    }
}
