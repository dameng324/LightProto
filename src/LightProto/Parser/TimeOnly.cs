

namespace LightProto.Parser;

public sealed class TimeOnlyProtoReader : IProtoReader<TimeOnly>
{
    public TimeOnly ParseFrom(ref ReaderContext input)
    {
        return new TimeOnly(input.ReadInt64());
    }
}
public sealed class TimeOnlyProtoWriter : IProtoWriter<TimeOnly>
{
    public int CalculateSize(TimeOnly value)
    {
        return CodedOutputStream.ComputeInt64Size(value.Ticks);
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