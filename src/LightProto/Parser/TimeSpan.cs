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
        switch (proxy.Scale)
        {
            case TimeSpanScale.DAYS:
                proxy.Ticks *= TimeSpan.TicksPerDay;
                break;
            case TimeSpanScale.HOURS:
                proxy.Ticks *= TimeSpan.TicksPerHour;
                break;
            case TimeSpanScale.MINUTES:
                proxy.Ticks *= TimeSpan.TicksPerMinute;
                break;
            case TimeSpanScale.SECONDS:
                proxy.Ticks *= TimeSpan.TicksPerSecond;
                break;
            case TimeSpanScale.MILLISECONDS:
                proxy.Ticks *= TimeSpan.TicksPerMillisecond;
                break;
            case TimeSpanScale.TICKS:
                break;
            case TimeSpanScale.MINMAX:
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
        return new TimeSpan(ticks: proxy.Ticks);
    }

    public static implicit operator TimeSpanProxy(TimeSpan dt) =>
        new TimeSpanProxy { Ticks = dt.Ticks, Scale = TimeSpanScale.TICKS };
}

public sealed class TimeSpanProtoParser : IProtoParser<TimeSpan>
{
    public static IProtoReader<TimeSpan> Reader { get; } = LightProto.Parser.TimeSpanProxy.Reader;
    public static IProtoWriter<TimeSpan> Writer { get; } = LightProto.Parser.TimeSpanProxy.Writer;
}
