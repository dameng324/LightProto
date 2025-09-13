using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class TimeSpanTests: BaseTests<TimeSpanTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public TimeSpan Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = TimeSpan.MinValue };
        yield return new Message() { Property = TimeSpan.MaxValue };
        yield return new Message() { Property = DateTime.Now.TimeOfDay };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}