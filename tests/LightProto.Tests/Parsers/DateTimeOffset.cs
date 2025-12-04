using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateTimeOffsetTests
    : BaseTests<DateTimeOffsetTests.Message, DateTimeOffsetTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public DateTimeOffset Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = DateTimeOffset.MinValue };
        yield return new() { Property = DateTimeOffset.MaxValue };
        yield return new() { Property = DateTimeOffset.UtcNow };
        yield return new() { Property = DateTimeOffset.Now };
        yield return new()
        {
            Property = new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromHours(1)),
        };
        yield return new()
        {
            Property = new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromHours(-1)),
        };
        yield return new()
        {
            Property = new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromMinutes(-1)),
        };
        yield return new()
        {
            Property = new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromMinutes(1)),
        };
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<DateTimeOffsetTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new DateTimeOffsetTestsMessage()
            {
                Property = new DateTimeOffsetMessage()
                {
                    Ticks = o.Property.UtcTicks,
                    OffsetTicks = o.Property.Offset.Ticks,
                },
            });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(DateTimeOffsetTestsMessage clone, Message message)
    {
        clone.Property ??= new DateTimeOffsetMessage();
        var dateTimeOffset = new DateTimeOffset(clone.Property.Ticks, TimeSpan.Zero).ToOffset(
            new TimeSpan(clone.Property.OffsetTicks)
        );
        await Assert.That(dateTimeOffset).IsEquivalentTo(message.Property);
    }
}
