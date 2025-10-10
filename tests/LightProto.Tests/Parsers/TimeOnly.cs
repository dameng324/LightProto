using LightProto;

namespace LightProto.Tests.Parsers;

#if NET6_0_OR_GREATER
[InheritsTests]
public partial class TimeOnlyTests : BaseTests<TimeOnlyTests.Message, TimeOnlyTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public TimeOnly Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = TimeOnly.MinValue };
        yield return new() { Property = TimeOnly.MaxValue };
        yield return new() { Property = TimeOnly.FromDateTime(DateTime.Today) };
    }

    public override IEnumerable<TimeOnlyTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new TimeOnlyTestsMessage() { Property = o.Property.Ticks });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(TimeOnlyTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property.Ticks);
    }
}
#endif
