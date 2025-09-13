using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateTimeTests : BaseTests<DateTimeTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public DateTime Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = DateTime.MinValue };
        yield return new Message() { Property = DateTime.MaxValue };
        yield return new Message() { Property = DateTime.UtcNow };
        yield return new Message() { Property = DateTime.Now };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Unspecified);
    }
}