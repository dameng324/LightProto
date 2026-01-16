namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class SkipConstructorWithInitializerTests : BaseTests<SkipConstructorWithInitializerTests.Message, StructTestsMessage>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Property { get; set; } = "PropertyInitializer";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public int Number { get; set; } = 42;

        public int IgnoredProperty { get; set; } = 100;

        public Message()
        {
            IgnoredProperty = 10;
            Property = "ConstructorValue";
            Number = 99;
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = string.Empty, Number = 0 };
        yield return new() { Property = Guid.NewGuid().ToString("N"), Number = 123 };
    }

    public override IEnumerable<StructTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
        await Assert.That(clone.Number).IsEquivalentTo(message.Number);
        // IgnoredProperty should be default (0), not the initializer (100) or constructor value (10)
        await Assert.That(clone.IgnoredProperty).IsEquivalentTo(0);
    }

    public override async Task AssertGoogleResult(StructTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property ?? string.Empty);
    }
}
