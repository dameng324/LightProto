

namespace Dameng.Protobuf.Parser;

public sealed class DateTimeProtoReader : IProtoReader<DateTime>
{
    public DateTime ParseFrom(ref ReaderContext input)
    {
        return new DateTime(input.ReadInt64());
    }
}
public sealed class DateTimeProtoWriter : IProtoWriter<DateTime>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(DateTime value)
    {
        return CodedOutputStream.ComputeInt64Size(value.Ticks);
    }


    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, DateTime value)
    {
        output.WriteInt64(value.Ticks);
    }
}
public sealed class DateTimeProtoParser : IProtoParser<DateTime>
{
    public static IProtoReader<DateTime> Reader { get; } = new DateTimeProtoReader();
    public static IProtoWriter<DateTime> Writer { get; } = new DateTimeProtoWriter();
}