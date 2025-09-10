

namespace Dameng.Protobuf.Parser;

public class Fixed32ProtoReader : IProtoReader<UInt32>
{
    public UInt32 ParseFrom(ref ReaderContext input)
    {
        return input.ReadFixed32();
    }
}
public class Fixed32ProtoWriter : IProtoWriter<UInt32>
{
    public int CalculateSize(UInt32 value)
    {
        return CodedOutputStream.ComputeFixed32Size(value);
    }

    public void WriteTo(ref WriterContext output, UInt32 value)
    {
        output.WriteFixed32(value);
    }
}
public class Fixed32ProtoParser : IProtoParser<UInt32>
{
    public static IProtoReader<UInt32> Reader { get; } = new Fixed32ProtoReader();
    public static IProtoWriter<UInt32> Writer { get; } = new Fixed32ProtoWriter();
}