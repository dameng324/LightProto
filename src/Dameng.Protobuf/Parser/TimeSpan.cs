

namespace Dameng.Protobuf.Parser;

public sealed class TimeSpanProtoReader : IProtoReader<TimeSpan>
{
    public TimeSpan ParseFrom(ref ReaderContext input)
    {
        return new TimeSpan(input.ReadInt64());
    }
}
public sealed class TimeSpanProtoWriter : IProtoWriter<TimeSpan>
{
    public int CalculateSize(TimeSpan value)
    {
        return CodedOutputStream.ComputeInt64Size(value.Ticks);
    }
    
    public void WriteTo(ref WriterContext output, TimeSpan value)
    {
        output.WriteInt64(value.Ticks);
    }
}
public sealed class TimeSpanProtoParser : IProtoParser<TimeSpan>
{
    public static IProtoReader<TimeSpan> Reader { get; } = new TimeSpanProtoReader();
    public static IProtoWriter<TimeSpan> Writer { get; } = new TimeSpanProtoWriter();
}