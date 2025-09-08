using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class SInt64ProtoReader : IProtoReader<Int64>
{
    public Int64 ParseFrom(ref ParseContext input)
    {
        return input.ReadSInt64();
    }
}
public class SInt64ProtoWriter : IProtoWriter<Int64>
{
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeSInt64Size(value);
    }

    public void WriteTo(ref WriteContext output, Int64 value)
    {
        output.WriteSInt64(value);
    }
}
public class SInt64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new SInt64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new SInt64ProtoWriter();
}