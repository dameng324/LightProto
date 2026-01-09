namespace LightProto.Tests.Parsers;

#if NET8_0_OR_GREATER
[InheritsTests]
public partial class SkipConstructorFalseWithReadonlyFieldTests
    : BaseTests<SkipConstructorFalseWithReadonlyFieldTests.Message, StructTestsMessage>
{
    [ProtoContract()]
    [ProtoBuf.ProtoContract()]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Property { get; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public int Number;

        public int IgnoredProperty { get; set; } = 100;

        public Message() { }

        public Message(string property, int number)
        {
            Property = property;
            Number = number;
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new(string.Empty, 10);
        yield return new();
        yield return new(Guid.NewGuid().ToString("N"), 123);
    }

    public override IEnumerable<StructTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
        await Assert.That(clone.Number).IsEquivalentTo(message.Number);
        await Assert.That(clone.IgnoredProperty).IsEquivalentTo(100);
    }

    public override async Task AssertGoogleResult(StructTestsMessage clone, Message message)
    {
        await Assert
            .That(clone.Property ?? string.Empty)
            .IsEquivalentTo(message.Property ?? string.Empty);
    }
}
#endif
