using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension;

public class DateTimeProtoReader : IProtoReader<DateTime>
{
    public DateTime ParseFrom(ref ParseContext input)
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


    public void WriteTo(ref WriteContext output, DateTime value)
    {
        output.WriteInt64(value.Ticks);
    }
}
public class DateTimeProtoParser : IProtoParser<DateTime>
{
    public static IProtoReader<DateTime> Reader { get; } = new DateTimeProtoReader();
    public static IProtoWriter<DateTime> Writer { get; } = new DateTimeProtoWriter();
}