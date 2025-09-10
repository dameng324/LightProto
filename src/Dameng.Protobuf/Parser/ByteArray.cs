
namespace Dameng.Protobuf.Parser;

public class ByteArrayProtoReader : IProtoReader<byte[]>
{
    public byte[] ParseFrom(ref ReaderContext input)
    {
        var length = input.ReadLength();
        return ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state,length);
    }
}
public class ByteArrayProtoWriter : IProtoWriter<byte[]>
{
    public int CalculateSize(byte[] value)
    {
        return CodedOutputStream.ComputeLengthSize(value.Length)+value.Length;
    }

    public void WriteTo(ref WriterContext output, byte[] value)
    {
        output.WriteLength(value.Length);
        WritingPrimitives.WriteRawBytes(ref output.buffer,ref output.state, value);
    }
}
public class ByteArrayProtoParser : IProtoParser<byte[]>
{
    public static IProtoReader<byte[]> Reader { get; } = new ByteArrayProtoReader();
    public static IProtoWriter<byte[]> Writer { get; } = new ByteArrayProtoWriter();
}