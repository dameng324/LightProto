

namespace Dameng.Protobuf.Parser;

public sealed class UInt64ProtoReader : IProtoReader<UInt64>
{
    public UInt64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadUInt64();
    }
}
public sealed class UInt64ProtoWriter : IProtoWriter<UInt64>
{
    public int CalculateSize(UInt64 value)
    {
        return CodedOutputStream.ComputeUInt64Size(value);
    }

    public void WriteTo(ref WriterContext output, UInt64 value)
    {
        output.WriteUInt64(value);
    }
}
public sealed class UInt64ProtoParser : IProtoParser<UInt64>
{
    public static IProtoReader<UInt64> Reader { get; } = new UInt64ProtoReader();
    public static IProtoWriter<UInt64> Writer { get; } = new UInt64ProtoWriter();
}