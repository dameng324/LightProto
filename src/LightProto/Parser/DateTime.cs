

namespace LightProto.Parser;
//
//
// message DateTime {
// sint64 value = 1; // the offset (in units of the selected scale) from 1970/01/01
// TimeSpanScale scale = 2; // the scale of the timespan [default = DAYS]
// DateTimeKind kind = 3; // the kind of date/time being represented [default = UNSPECIFIED]
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
// enum DateTimeKind
// {     
//     // The time represented is not specified as either local time or Coordinated Universal Time (UTC).
//     UNSPECIFIED = 0;
//     // The time represented is UTC.
//     UTC = 1;
//     // The time represented is local time.
//     LOCAL = 2;
// }
// }

[ProtoContract]
[ProtoProxyFor<DateTime>]
public partial struct DateTimeProxy
{
    [ProtoMember(1,DataFormat = DataFormat.ZigZag)]
    internal long Ticks { get; set; }
    [ProtoMember(2)]
    internal TimeSpanScale Scale { get; set; }
    
    public static implicit operator DateTime(DateTimeProxy proxy)
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
                    return DateTime.MinValue;
                else if(proxy.Ticks==1)
                    return DateTime.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(nameof(proxy.Ticks), $"Invalid ticks for MINMAX scale: {proxy.Ticks}");
            default:
                throw new ArgumentOutOfRangeException(nameof(proxy.Scale), $"Unknown scale: {proxy.Scale}");
        }
        return new DateTime(ticks: proxy.Ticks+EpochOrigin.Ticks, kind: DateTimeKind.Unspecified);
    }

    internal static readonly DateTime EpochOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
    public static implicit operator DateTimeProxy(DateTime dt) => new DateTimeProxy
    {
        Ticks = dt.Ticks-EpochOrigin.Ticks,
        Scale = TimeSpanScale.TICKS
    };
    
}

internal enum TimeSpanScale
{
    DAYS = 0,
    HOURS = 1,
    MINUTES = 2,
    SECONDS = 3,
    MILLISECONDS = 4,
    TICKS = 5,
    MINMAX = 15
}
public sealed class DateTimeProtoParser : IProtoParser<DateTime>
{
    public static IProtoReader<DateTime> Reader { get; } = LightProto.Parser.DateTimeProxy.Reader;
    public static IProtoWriter<DateTime> Writer { get; } = LightProto.Parser.DateTimeProxy.Writer;
}