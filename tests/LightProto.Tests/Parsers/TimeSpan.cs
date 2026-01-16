using Google.Protobuf.WellKnownTypes;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class TimeSpanTests : BaseTests<TimeSpanTests.Message, TimeSpanTestsMessage>
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
        yield return new() { Property = TimeSpan.MinValue };
        yield return new() { Property = TimeSpan.MaxValue };
        yield return new() { Property = DateTime.Now.TimeOfDay };
        yield return new() { Property = TimeSpan.FromDays(1) };
        yield return new() { Property = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1)) };
        yield return new() { Property = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1)).Add(TimeSpan.FromMinutes(1)) };
        yield return new()
        {
            Property = TimeSpan.FromDays(1).Add(TimeSpan.FromHours(1)).Add(TimeSpan.FromMinutes(1)).Add(TimeSpan.FromSeconds(1)),
        };
        yield return new()
        {
            Property = TimeSpan
                .FromDays(1)
                .Add(TimeSpan.FromHours(1))
                .Add(TimeSpan.FromMinutes(1))
                .Add(TimeSpan.FromSeconds(1))
                .Add(TimeSpan.FromMilliseconds(1)),
        };
    }

    public override IEnumerable<TimeSpanTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new TimeSpanTestsMessage() { Property = o.Property.ToProtobuf() });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(TimeSpanTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToTimeSpan()).IsEquivalentTo(message.Property);
    }
}

file static class Extension
{
    public static ProtoBuf.Bcl.TimeSpan ToProtobuf(this TimeSpan value)
    {
        return new ProtoBuf.Bcl.TimeSpan { Value = value.Ticks, Scale = ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Ticks };
    }

    public static TimeSpan ToTimeSpan(this ProtoBuf.Bcl.TimeSpan proxy)
    {
        long ticks;
        switch (proxy.Scale)
        {
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Days:
                ticks = proxy.Value * TimeSpan.TicksPerDay;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Hours:
                ticks = proxy.Value * TimeSpan.TicksPerHour;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Minutes:
                ticks = proxy.Value * TimeSpan.TicksPerMinute;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Seconds:
                ticks = proxy.Value * TimeSpan.TicksPerSecond;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Milliseconds:
                ticks = proxy.Value * TimeSpan.TicksPerMillisecond;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Ticks:
                ticks = proxy.Value;
                break;
            case global::ProtoBuf.Bcl.TimeSpan.Types.TimeSpanScale.Minmax:
                if (proxy.Value == -1)
                    return TimeSpan.MinValue;
                else if (proxy.Value == 1)
                    return TimeSpan.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(nameof(proxy.Value), $"Invalid ticks for MINMAX scale: {proxy.Value}");
            default:
                throw new ArgumentOutOfRangeException(nameof(proxy.Scale), $"Unknown scale: {proxy.Scale}");
        }
        return new TimeSpan(ticks);
    }
}
