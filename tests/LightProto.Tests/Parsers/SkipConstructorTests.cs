namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class SkipConstructorTests
    : BaseTests<SkipConstructorTests.Message, StructTestsMessage>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Property { get; set; }

        public int IgnoredProperty { get; set; }

        public Message()
        {
            IgnoredProperty = 10;
            Property = string.Empty;
        }
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
        await Assert.That(clone.IgnoredProperty).IsEquivalentTo(0);
    }

    public override async Task AssertGoogleResult(StructTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
    }
}
