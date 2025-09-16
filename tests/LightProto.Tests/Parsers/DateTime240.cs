using Google.Protobuf.WellKnownTypes;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateTime240Tests : BaseTests<DateTime240Tests.Message, DateTime240TestsMessage>
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
        //yield return new () { Property = DateTime.MinValue.ToUniversalTime() };
        yield return new() { Property = DateTime.MaxValue.ToUniversalTime() };
        yield return new() { Property = DateTime.UtcNow };
        yield return new() { Property = DateTime.Now.ToUniversalTime() };
    }

    public override IEnumerable<DateTime240TestsMessage> GetGoogleMessages()
    {
        yield return new DateTime240TestsMessage()
        {
            Property = DateTime.MaxValue.ToUniversalTime().ToTimestamp(),
        };
        yield return new DateTime240TestsMessage()
        {
            Property = DateTime.MinValue.ToUniversalTime().ToTimestamp(),
        };
        yield return new DateTime240TestsMessage() { Property = DateTime.UtcNow.ToTimestamp() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Utc);
    }

    public override async Task AssertGoogleResult(DateTime240TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToDateTime().Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.ToDateTime().Kind).IsEquivalentTo(DateTimeKind.Utc);
    }
}
