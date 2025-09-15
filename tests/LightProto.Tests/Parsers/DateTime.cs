using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class DateTimeTests : BaseTests<DateTimeTests.Message, DateTimeTestsMessage>
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
        yield return new() { Property = DateTime.MinValue };
        yield return new() { Property = DateTime.MaxValue };
        yield return new() { Property = DateTime.UtcNow };
        yield return new() { Property = DateTime.Now };
    }

    public override IEnumerable<DateTimeTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new DateTimeTestsMessage() { Property = o.Property.ToProtobuf() });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        //await Assert.That(clone.Property.Kind).IsEquivalentTo(message.Property.Kind); // Kind is not include by default in protobuf-net
    }

    public override async Task AssertGoogleResult(DateTimeTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToDateTime()).IsEquivalentTo(message.Property);
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
            Kind = (ProtoBuf.Bcl.DateTime.Types.DateTimeKind) dt.Kind,
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
                ticks = proxy.Value* TimeSpan.TicksPerDay;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Hours:
                ticks = proxy.Value* TimeSpan.TicksPerHour;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minutes:
                ticks = proxy.Value* TimeSpan.TicksPerMinute;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Seconds:
                ticks = proxy.Value* TimeSpan.TicksPerSecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Milliseconds:
                ticks = proxy.Value* TimeSpan.TicksPerMillisecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Ticks:
                ticks= proxy.Value;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minmax:
                if (proxy.Value == -1)
                    return DateTime.MinValue;
                else if (proxy.Value == 1)
                    return DateTime.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(
                        nameof(proxy.Value),
                        $"Invalid ticks for MINMAX scale: {proxy.Value}"
                    );
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(proxy.Scale),
                    $"Unknown scale: {proxy.Scale}"
                );
        }
        return new DateTime(
            ticks: ticks + EpochOriginsTicks[(int)proxy.Kind],
            kind: (System.DateTimeKind) proxy.Kind
        );
    }
}
