using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class FieldTests : BaseTests<FieldTests.Message, StructTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial struct Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Property;
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override IEnumerable<StructTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(StructTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
    }
}
