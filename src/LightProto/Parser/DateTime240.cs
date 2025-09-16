namespace LightProto.Parser;

[ProtoContract]
[ProtoProxyFor<DateTime>]
public sealed partial class DateTime240Proxy
{
    [ProtoMember(1)]
    private long Seconds { get; set; }

    [ProtoMember(2)]
    private int Nanos { get; set; }

    private static readonly DateTime UnixEpoch = new DateTime(
        1970,
        1,
        1,
        0,
        0,
        0,
        DateTimeKind.Utc
    );

    // Constants determined programmatically, but then hard-coded so they can be constant expressions.
    private const long BclSecondsAtUnixEpoch = 62135596800;
    internal const long UnixSecondsAtBclMaxValue = 253402300799;
    internal const long UnixSecondsAtBclMinValue = -BclSecondsAtUnixEpoch;

    /// <summary>
    /// The number of nanoseconds in a second.
    /// </summary>
    public const int NanosecondsPerSecond = 1000000000;

    /// <summary>
    /// The number of nanoseconds in a BCL tick (as used by <see cref="TimeSpan"/> and <see cref="DateTime"/>).
    /// </summary>
    public const int NanosecondsPerTick = 100;
    internal const int MaxNanos = NanosecondsPerSecond - 1;

    public static implicit operator DateTime(DateTime240Proxy proxy)
    {
        if (!IsNormalized(proxy.Seconds, proxy.Nanos))
        {
            throw new InvalidOperationException(
                @"Timestamp contains invalid values: Seconds={Seconds}; Nanos={Nanos}"
            );
        }
        return UnixEpoch.AddSeconds(proxy.Seconds).AddTicks(proxy.Nanos / NanosecondsPerTick);
    }

    public static implicit operator DateTime240Proxy(DateTime dateTime)
    {
        // Do the arithmetic using DateTime.Ticks, which is always non-negative, making things simpler.
        long secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
        int nanoseconds = (int)(dateTime.Ticks % TimeSpan.TicksPerSecond) * NanosecondsPerTick;
        return new DateTime240Proxy
        {
            Seconds = secondsSinceBclEpoch - BclSecondsAtUnixEpoch,
            Nanos = nanoseconds,
        };
    }

    private static bool IsNormalized(long seconds, int nanoseconds) =>
        nanoseconds >= 0
        && nanoseconds <= MaxNanos
        && seconds >= UnixSecondsAtBclMinValue
        && seconds <= UnixSecondsAtBclMaxValue;
}

public sealed class DateTime240ProtoParser : IProtoParser<DateTime>
{
    public static IProtoReader<DateTime> Reader { get; } =
        LightProto.Parser.DateTime240Proxy.Reader;
    public static IProtoWriter<DateTime> Writer { get; } =
        LightProto.Parser.DateTime240Proxy.Writer;
}
