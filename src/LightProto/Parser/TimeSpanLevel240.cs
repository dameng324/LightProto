namespace LightProto.Parser;

[ProtoContract]
[ProtoProxyFor<TimeSpan>]
public sealed partial class TimeSpan240Proxy
{
    [ProtoMember(1)]
    public long Seconds { get; set; }

    [ProtoMember(2)]
    public int Nanos { get; set; }

    /// <summary>
    /// The number of nanoseconds in a second.
    /// </summary>
    public const int NanosecondsPerSecond = 1000000000;

    /// <summary>
    /// The number of nanoseconds in a BCL tick (as used by <see cref="TimeSpan"/> and <see cref="TimeSpan"/>).
    /// </summary>
    public const int NanosecondsPerTick = 100;

    /// <summary>
    /// The maximum permitted number of seconds.
    /// </summary>
    public const long MaxSeconds = 315576000000L;

    /// <summary>
    /// The minimum permitted number of seconds.
    /// </summary>
    public const long MinSeconds = -315576000000L;

    internal const int MaxNanoseconds = NanosecondsPerSecond - 1;
    internal const int MinNanoseconds = -NanosecondsPerSecond + 1;

    internal static bool IsNormalized(long seconds, int nanoseconds)
    {
        // Simple boundaries
        if (
            seconds < MinSeconds
            || seconds > MaxSeconds
            || nanoseconds < MinNanoseconds
            || nanoseconds > MaxNanoseconds
        )
        {
            return false;
        }
        // We only have a problem is one is strictly negative and the other is
        // strictly positive.
        return Math.Sign(seconds) * Math.Sign(nanoseconds) != -1;
    }

    public static implicit operator TimeSpan(TimeSpan240Proxy proxy)
    {
        checked
        {
            long ticks = proxy.Seconds * TimeSpan.TicksPerSecond + proxy.Nanos / NanosecondsPerTick;
            return TimeSpan.FromTicks(ticks);
        }
    }

    public static implicit operator TimeSpan240Proxy(TimeSpan timeSpan)
    {
        checked
        {
            long ticks = timeSpan.Ticks;
            long seconds = ticks / TimeSpan.TicksPerSecond;
            int nanos = (int)(ticks % TimeSpan.TicksPerSecond) * NanosecondsPerTick;
            return new TimeSpan240Proxy { Seconds = seconds, Nanos = nanos };
        }
    }
}

public sealed class TimeSpan240ProtoParser : IProtoParser<TimeSpan>
{
    public static IProtoReader<TimeSpan> Reader { get; } = LightProto.Parser.TimeSpan240Proxy.Reader;
    public static IProtoWriter<TimeSpan> Writer { get; } = LightProto.Parser.TimeSpan240Proxy.Writer;
}