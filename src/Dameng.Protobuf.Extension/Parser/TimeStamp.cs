using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension;

public class TimestampProtoReader : IProtoReader<Timestamp>
{
    public Timestamp ParseFrom(ref ParseContext input)
    {
        var timestamp = new Timestamp();
        input.ReadMessage(timestamp);
        return timestamp;
    }
}
public class TimestampProtoWriter : IProtoMessageWriter<Timestamp>
{
    public int CalculateSize(Timestamp value)
    {
        return value.CalculateSize();
    }


    public void WriteTo(ref WriteContext output, Timestamp value)
    {
        WritingPrimitivesMessages.WriteRawMessage(ref output, value);
    }
    public int CalculateMessageSize(Timestamp value)
    {
        return CodedOutputStream.ComputeMessageSize(value);
    }

    public void WriteMessageTo(ref WriteContext output, Timestamp value)
    {
        output.WriteMessage(value);
    }
}
public class TimestampProtoParser : IProtoParser<Timestamp>
{
    public static IProtoReader<Timestamp> Reader { get; } = new TimestampProtoReader();
    public static IProtoWriter<Timestamp> Writer { get; } = new TimestampProtoWriter();
}