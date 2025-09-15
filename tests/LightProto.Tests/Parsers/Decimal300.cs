using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class Decimal300Tests: BaseTests<Decimal300Tests.Message,Decimal300TestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        [CompatibilityLevel(CompatibilityLevel.Level300)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level300)]
        public decimal Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new () { Property = Decimal.MinValue };
        yield return new () { Property = Decimal.MaxValue };
        yield return new () { Property = 0 };
        yield return new () { Property = 1000M };
        yield return new () { Property = -1000M };
    }

    public override IEnumerable<Decimal300TestsMessage> GetGoogleMessages()
    {
        yield return new () { Property = Decimal.MinValue.ToString() };
        yield return new () { Property = Decimal.MaxValue.ToString() };
        yield return new () { Property = 0.ToString() };
        yield return new () { Property = 1000M.ToString() };
        yield return new () { Property = (-1000M).ToString() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(Decimal300TestsMessage clone, Message message)
    {
        await Assert.That(decimal.Parse(clone.Property)).IsEquivalentTo(message.Property);
    }
}