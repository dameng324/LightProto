using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class Int64ProtoReader : IProtoReader<Int64>
{
    public Int64 ParseFrom(ref ParseContext input)
    {
        return input.ReadInt64();
    }
}
public class Int64ProtoWriter : IProtoWriter<Int64>
{
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeInt64Size(value);
    }

    public void WriteTo(ref WriteContext output, Int64 value)
    {
        output.WriteInt64(value);
    }
}
public class Int64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new Int64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new Int64ProtoWriter();
}