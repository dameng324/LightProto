using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateOnlyTests : BaseTests<DateOnlyTests.Message, DateOnlyTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public DateOnly Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = DateOnly.MinValue };
        yield return new() { Property = DateOnly.MaxValue };
        yield return new() { Property = DateOnly.FromDateTime(DateTime.Today) };
    }

    public override IEnumerable<DateOnlyTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new DateOnlyTestsMessage() { Property = o.Property.DayNumber });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(DateOnlyTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property.DayNumber);
    }
}
