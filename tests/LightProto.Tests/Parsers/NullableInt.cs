using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class NullableIntTests : BaseTests<NullableIntTests.Message, NullableIntTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public int? Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = 3 };
        yield return new() { Property = 2 };
        yield return new() { Property = 1 };
        yield return new() { Property = 0 };
        yield return new() { Property = null };
    }

    public override IEnumerable<NullableIntTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new NullableIntTestsMessage() { Property = o.Property ?? 0 });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.HasValue).IsEquivalentTo(message.Property.HasValue);
        if (clone.Property.HasValue && message.Property.HasValue)
            await Assert.That(clone.Property.Value).IsEquivalentTo(message.Property.Value);
    }

    public override async Task AssertGoogleResult(NullableIntTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property is 0).IsEquivalentTo(message.Property is null or 0);
        if (message.Property is not null)
            await Assert.That(clone.Property).IsEquivalentTo(message.Property.Value);
    }
}

file static class Extension
{
    public static ProtoBuf.Bcl.DateTime ToProtobuf(this DateTime dt)
    {
        return new ProtoBuf.Bcl.DateTime()
        {
            Value = dt.Ticks - EpochOriginsTicks[(int)dt.Kind],
            Scale = ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Ticks,
            Kind = (ProtoBuf.Bcl.DateTime.Types.DateTimeKind)dt.Kind,
        };
    }

    internal static readonly long[] EpochOriginsTicks =
    [
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).Ticks,
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks,
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).Ticks,
    ];

    public static DateTime ToDateTime(this ProtoBuf.Bcl.DateTime proxy)
    {
        long ticks;
        switch (proxy.Scale)
        {
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Days:
                ticks = proxy.Value * TimeSpan.TicksPerDay;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Hours:
                ticks = proxy.Value * TimeSpan.TicksPerHour;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minutes:
                ticks = proxy.Value * TimeSpan.TicksPerMinute;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Seconds:
                ticks = proxy.Value * TimeSpan.TicksPerSecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Milliseconds:
                ticks = proxy.Value * TimeSpan.TicksPerMillisecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Ticks:
                ticks = proxy.Value;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minmax:
                if (proxy.Value == -1)
                    return DateTime.MinValue;
                else if (proxy.Value == 1)
                    return DateTime.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(nameof(proxy.Value), $"Invalid ticks for MINMAX scale: {proxy.Value}");
            default:
                throw new ArgumentOutOfRangeException(nameof(proxy.Scale), $"Unknown scale: {proxy.Scale}");
        }
        return new DateTime(ticks: ticks + EpochOriginsTicks[(int)proxy.Kind], kind: (System.DateTimeKind)proxy.Kind);
    }
}
