using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class LazyTests : BaseTests<LazyTests.Message, LazyTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public required Lazy<int> Property { get; set; }

        [ProtoMember(2)]
        public required Lazy<string> Property2 { get; set; }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Lazy<int>(() => 1), Property2 = new Lazy<string>(() => "test") };
        yield return new() { Property = new Lazy<int>(() => 0), Property2 = new Lazy<string>(() => "") };
        yield return new() { Property = new Lazy<int>(() => -1), Property2 = new Lazy<string>(() => " ") };
    }

    public override IEnumerable<LazyTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new LazyTestsMessage() { Property = o.Property.Value, Property2 = o.Property2.Value });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Value).IsEquivalentTo(message.Property.Value);
        await Assert.That(clone.Property2.Value).IsEquivalentTo(message.Property2.Value);
    }

    public override async Task AssertGoogleResult(LazyTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property?.Value ?? 0);
        await Assert.That(clone.Property2).IsEquivalentTo(message.Property2?.Value ?? string.Empty);
    }
}
