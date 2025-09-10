

namespace Dameng.Protobuf.Parser;

public class SInt32ProtoReader : IProtoReader<Int32>
{
    public Int32 ParseFrom(ref ReaderContext input)
    {
        return input.ReadSInt32();
    }
}
public class SInt32ProtoWriter : IProtoWriter<Int32>
{
    public int CalculateSize(Int32 value)
    {
        return CodedOutputStream.ComputeSInt32Size(value);
    }

    public void WriteTo(ref WriterContext output, Int32 value)
    {
        output.WriteSInt32(value);
    }
}
public class SInt32ProtoParser : IProtoParser<Int32>
{
    public static IProtoReader<Int32> Reader { get; } = new SInt32ProtoReader();
    public static IProtoWriter<Int32> Writer { get; } = new SInt32ProtoWriter();
}