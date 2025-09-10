

namespace Dameng.Protobuf.Parser;

public class Int32ProtoReader : IProtoReader<int>
{
    public int ParseFrom(ref ReaderContext input)
    {
        return input.ReadInt32();
    }
}
public class Int32ProtoWriter : IProtoWriter<int>
{
    public int CalculateSize(int value)
    {
        return CodedOutputStream.ComputeInt32Size(value);
    }

    public void WriteTo(ref WriterContext output, int value)
    {
        output.WriteInt32(value);
    }
}
public class Int32ProtoParser : IProtoParser<int>
{
    public static IProtoReader<int> Reader { get; } = new Int32ProtoReader();
    public static IProtoWriter<int> Writer { get; } = new Int32ProtoWriter();
}