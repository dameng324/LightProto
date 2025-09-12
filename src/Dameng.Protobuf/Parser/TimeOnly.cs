

namespace Dameng.Protobuf.Parser;

public sealed class TimeOnlyProtoReader : IProtoReader<TimeOnly>
{
    public TimeOnly ParseFrom(ref ReaderContext input)
    {
        return new TimeOnly(input.ReadInt64());
    }
}
public sealed class TimeOnlyProtoWriter : IProtoWriter<TimeOnly>
{
    public int CalculateSize(TimeOnly pair)
    {
        return CodedOutputStream.ComputeInt64Size(pair.Ticks);
    }
    
    public void WriteTo(ref WriterContext output, TimeOnly value)
    {
        output.WriteInt64(value.Ticks);
    }
}
public sealed class TimeOnlyProtoParser : IProtoParser<TimeOnly>
{
    public static IProtoReader<TimeOnly> Reader { get; } = new TimeOnlyProtoReader();
    public static IProtoWriter<TimeOnly> Writer { get; } = new TimeOnlyProtoWriter();
}