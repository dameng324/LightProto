using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class SFixed64ProtoReader : IProtoReader<Int64>
{
    public Int64 ParseFrom(ref ParseContext input)
    {
        return input.ReadSFixed64();
    }
}
public class SFixed64ProtoWriter : IProtoWriter<Int64>
{
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeSFixed64Size(value);
    }

    public void WriteTo(ref WriteContext output, Int64 value)
    {
        output.WriteSFixed64(value);
    }
}
public class SFixed64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new SFixed64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new SFixed64ProtoWriter();
}