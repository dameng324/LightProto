

namespace Dameng.Protobuf.Parser;

public class SFixed32ProtoReader : IProtoReader<int>
{
    public int ParseFrom(ref ReaderContext input)
    {
        return input.ReadSFixed32();
    }
}
public class SFixed32ProtoWriter : IProtoWriter<int>
{
    public int CalculateSize(int value)
    {
        return CodedOutputStream.ComputeSFixed32Size(value);
    }

    public void WriteTo(ref WriterContext output, int value)
    {
        output.WriteSFixed32(value);
    }
}
public class SFixed32ProtoParser : IProtoParser<int>
{
    public static IProtoReader<int> Reader { get; } = new SFixed32ProtoReader();
    public static IProtoWriter<int> Writer { get; } = new SFixed32ProtoWriter();
}