using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class ByteStringProtoReader : IProtoReader<ByteString>
{
    public ByteString ParseFrom(ref ParseContext input)
    {
        return input.ReadBytes();
    }
}
public class ByteStringProtoWriter : IProtoWriter<ByteString>
{
    public int CalculateSize(ByteString value)
    {
        return CodedOutputStream.ComputeBytesSize(value);
    }

    public void WriteTo(ref WriteContext output, ByteString value)
    {
        output.WriteBytes(value);
    }
}
public class ByteStringProtoParser : IProtoParser<ByteString>
{
    public static IProtoReader<ByteString> Reader { get; } = new ByteStringProtoReader();
    public static IProtoWriter<ByteString> Writer { get; } = new ByteStringProtoWriter();
}