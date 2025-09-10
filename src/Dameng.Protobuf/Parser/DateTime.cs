

namespace Dameng.Protobuf.Parser;

public class DateTimeProtoReader : IProtoReader<DateTime>
{
    public DateTime ParseFrom(ref ReaderContext input)
    {
        return new DateTime(input.ReadInt64());
    }
}
public class DateTimeProtoWriter : IProtoWriter<DateTime>
{
    public int CalculateSize(DateTime value)
    {
        return CodedOutputStream.ComputeInt64Size(value.Ticks);
    }


    public void WriteTo(ref WriterContext output, DateTime value)
    {
        output.WriteInt64(value.Ticks);
    }
}
public class DateTimeProtoParser : IProtoParser<DateTime>
{
    public static IProtoReader<DateTime> Reader { get; } = new DateTimeProtoReader();
    public static IProtoWriter<DateTime> Writer { get; } = new DateTimeProtoWriter();
}