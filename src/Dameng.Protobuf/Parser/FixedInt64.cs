

namespace Dameng.Protobuf.Parser;

public class Fixed64ProtoReader : IProtoReader<UInt64>
{
    public UInt64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadFixed64();
    }
}
public class Fixed64ProtoWriter : IProtoWriter<UInt64>
{
    public int CalculateSize(UInt64 value)
    {
        return CodedOutputStream.ComputeFixed64Size(value);
    }

    public void WriteTo(ref WriterContext output, UInt64 value)
    {
        output.WriteFixed64(value);
    }
}
public class Fixed64ProtoParser : IProtoParser<UInt64>
{
    public static IProtoReader<UInt64> Reader { get; } = new Fixed64ProtoReader();
    public static IProtoWriter<UInt64> Writer { get; } = new Fixed64ProtoWriter();
}