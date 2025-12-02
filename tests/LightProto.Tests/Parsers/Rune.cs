using LightProto;

namespace LightProto.Tests.Parsers;

#if NET5_0_OR_GREATER
[InheritsTests]
public partial class RuneTests : BaseTests<RuneTests.Message, RuneTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public System.Text.Rune Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        // Test various Unicode scalar values
        yield return new() { Property = new System.Text.Rune('A') };
        yield return new() { Property = new System.Text.Rune('z') };
        yield return new() { Property = new System.Text.Rune('0') };
        yield return new() { Property = new System.Text.Rune('€') }; // Euro sign
        yield return new() { Property = new System.Text.Rune('中') }; // Chinese character
        yield return new() { Property = new System.Text.Rune(0x1F600) }; // Emoji (grinning face) by code point
        yield return new() { Property = new System.Text.Rune(0x0041) }; // 'A' by code point
        yield return new() { Property = new System.Text.Rune(0x1F4A9) }; // Another emoji (pile of poo) by code point
    }

    public override IEnumerable<RuneTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(m => new RuneTestsMessage { Property = (uint)m.Property.Value });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Value).IsEquivalentTo(message.Property.Value);
    }

    public override async Task AssertGoogleResult(RuneTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo((uint)message.Property.Value);
    }
}
#endif
