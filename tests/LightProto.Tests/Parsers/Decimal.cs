using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class DecimalTests: BaseTests<DecimalTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public decimal Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = Decimal.MinValue };
        yield return new Message() { Property = Decimal.MaxValue };
        yield return new Message() { Property = 0 };
        yield return new Message() { Property = 1000M };
        yield return new Message() { Property = -1000M };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

}