

namespace Dameng.Protobuf.Parser;

public class StringProtoReader : IProtoReader<string>
{
    public string ParseFrom(ref ReaderContext input)
    {
        return input.ReadString();
    }
}
public class StringProtoWriter : IProtoWriter<string>
{
    public int CalculateSize(string value)
    {
        return CodedOutputStream.ComputeStringSize(value);
    }

    public void WriteTo(ref WriterContext output, string value)
    {
        output.WriteString(value);
    }
}
public class StringProtoParser : IProtoParser<string>
{
    public static IProtoReader<string> Reader { get; } = new StringProtoReader();
    public static IProtoWriter<string> Writer { get; } = new StringProtoWriter();
}