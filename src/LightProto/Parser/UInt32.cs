

namespace LightProto.Parser;

public sealed class UInt32ProtoReader : IProtoReader<UInt32>
{
    public UInt32 ParseFrom(ref ReaderContext input)
    {
        return input.ReadUInt32();
    }
}
public sealed class UInt32ProtoWriter : IProtoWriter<UInt32>
{
    public int CalculateSize(UInt32 value)
    {
        return CodedOutputStream.ComputeUInt32Size(value);
    }

    public void WriteTo(ref WriterContext output, UInt32 value)
    {
        output.WriteUInt32(value);
    }
}
public sealed class UInt32ProtoParser : IProtoParser<UInt32>
{
    public static IProtoReader<UInt32> Reader { get; } = new UInt32ProtoReader();
    public static IProtoWriter<UInt32> Writer { get; } = new UInt32ProtoWriter();
}