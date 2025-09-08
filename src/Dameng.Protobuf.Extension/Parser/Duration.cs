using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension;

public class DurationProtoReader : IProtoReader<Duration>
{
    public Duration ParseFrom(ref ParseContext input)
    {
        var Duration = new Duration();
        input.ReadMessage(Duration);
        return Duration;
    }
}
public class DurationProtoWriter : IProtoMessageWriter<Duration>
{
    public int CalculateSize(Duration value)
    {
        return value.CalculateSize();
    }
    
    public void WriteTo(ref WriteContext output, Duration value)
    {
        WritingPrimitivesMessages.WriteRawMessage(ref output, value);
    }
    public int CalculateMessageSize(Duration value)
    {
        return CodedOutputStream.ComputeMessageSize(value);
    }

    public void WriteMessageTo(ref WriteContext output, Duration value)
    {
        output.WriteMessage(value);
    }
}
public class DurationProtoParser : IProtoParser<Duration>
{
    public static IProtoReader<Duration> Reader { get; } = new DurationProtoReader();
    public static IProtoWriter<Duration> Writer { get; } = new DurationProtoWriter();
}