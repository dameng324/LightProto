using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class StringProtoReader : IProtoReader<string>
{
    public string ParseFrom(ref ParseContext input)
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

    public void WriteTo(ref WriteContext output, string value)
    {
        output.WriteString(value);
    }
}
public class StringProtoParser : IProtoParser<string>
{
    public static IProtoReader<string> Reader { get; } = new StringProtoReader();
    public static IProtoWriter<string> Writer { get; } = new StringProtoWriter();
}