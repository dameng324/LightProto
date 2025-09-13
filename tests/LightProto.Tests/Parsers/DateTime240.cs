using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateTime240Tests:BaseTests<DateTime240Tests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        [CompatibilityLevel(CompatibilityLevel.Level240)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level240)]
        public DateTime Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = DateTime.MaxValue.ToUniversalTime() };
        yield return new Message() { Property = DateTime.UtcNow };
        yield return new Message() { Property = DateTime.Now.ToUniversalTime() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Utc);
    }
}