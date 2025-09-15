namespace LightProto.Parser;

// message TimeSpan {
// sint64 value = 1; // the size of the timespan (in units of the selected scale)
// TimeSpanScale scale = 2; // the scale of the timespan [default = DAYS]
// enum TimeSpanScale {
//     DAYS = 0;
//     HOURS = 1;
//     MINUTES = 2;
//     SECONDS = 3;
//     MILLISECONDS = 4;
//     TICKS = 5;
//
//     MINMAX = 15; // dubious
// }
// }

[ProtoProxyFor<TimeSpan>]
[ProtoContract]
public partial struct TimeSpanProxy
{
    [ProtoMember(1, DataFormat = DataFormat.ZigZag)]
    internal long Ticks { get; set; }

    [ProtoMember(2)]
    internal TimeSpanScale Scale { get; set; }

    public static implicit operator TimeSpan(TimeSpanProxy proxy)
    {
        long ticks;
        switch (proxy.Scale)
        {
            case TimeSpanScale.Days:
                ticks=proxy.Ticks* TimeSpan.TicksPerDay;
                break;
            case TimeSpanScale.Hours:
                ticks=proxy.Ticks* TimeSpan.TicksPerHour;
                break;
            case TimeSpanScale.Minutes:
                ticks=proxy.Ticks* TimeSpan.TicksPerMinute;
                break;
            case TimeSpanScale.Seconds:
                ticks=proxy.Ticks* TimeSpan.TicksPerSecond;
                break;
            case TimeSpanScale.Milliseconds:
                ticks=proxy.Ticks* TimeSpan.TicksPerMillisecond;
                break;
            case TimeSpanScale.Ticks:
                ticks = proxy.Ticks;
                break;
            case TimeSpanScale.Minmax:
                if (proxy.Ticks == -1)
                    return TimeSpan.MinValue;
                else if (proxy.Ticks == 1)
                    return TimeSpan.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(
                        nameof(proxy.Ticks),
                        $"Invalid ticks for MINMAX scale: {proxy.Ticks}"
                    );
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(proxy.Scale),
                    $"Unknown scale: {proxy.Scale}"
                );
        }
        return new TimeSpan(ticks);
    }

    public static implicit operator TimeSpanProxy(TimeSpan dt)
    {
        if (dt == TimeSpan.MinValue)
        {
            return new TimeSpanProxy
            {
                Ticks = -1,
                Scale = TimeSpanScale.Minmax,
            };
        }

        if (dt == TimeSpan.MaxValue)
        {
            return new TimeSpanProxy
            {
                Ticks = 1,
                Scale = TimeSpanScale.Minmax,
            };
        }

        var ticks = dt.Ticks;
        var left = Math.DivRem(ticks, TimeSpan.TicksPerDay, out var reminder);
        if (reminder == 0)
        {
            return new()
            {
                Ticks = left,
                Scale = TimeSpanScale.Days,
            };
        }

        left = Math.DivRem(ticks, TimeSpan.TicksPerHour, out reminder);
        if (reminder == 0)
        {
            return new()
            {
                Ticks = left,
                Scale = TimeSpanScale.Hours,
            };
        }
        left = Math.DivRem(ticks, TimeSpan.TicksPerMinute, out reminder);
        if (reminder == 0)
        {
            return new()
            {
                Ticks = left,
                Scale = TimeSpanScale.Minutes,
            };
        }
        left = Math.DivRem(ticks, TimeSpan.TicksPerSecond, out reminder);
        if (reminder == 0)
        {
            return new()
            {
                Ticks = left,
                Scale = TimeSpanScale.Seconds,
            };
        }
        left = Math.DivRem(ticks, TimeSpan.TicksPerMillisecond, out reminder);
        if (reminder == 0)
        {
            return new()
            {
                Ticks = left,
                Scale = TimeSpanScale.Milliseconds,
            };
        }

        return new()
        {
            Ticks = ticks,
            Scale = TimeSpanScale.Ticks,
        };
    }
}

public sealed class TimeSpanProtoParser : IProtoParser<TimeSpan>
{
    public static IProtoReader<TimeSpan> Reader { get; } = LightProto.Parser.TimeSpanProxy.Reader;
    public static IProtoWriter<TimeSpan> Writer { get; } = LightProto.Parser.TimeSpanProxy.Writer;
}
