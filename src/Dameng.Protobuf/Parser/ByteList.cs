
using System.Runtime.InteropServices;

namespace Dameng.Protobuf.Parser;

public class ByteListProtoReader : IProtoReader<List<byte>>
{
    public List<byte> ParseFrom(ref ReaderContext input)
    {
        var length = input.ReadLength();
        return [..ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length)];
    }
}
public class ByteListProtoWriter : IProtoWriter<List<byte>>
{
    public int CalculateSize(List<byte> value)
    {
        return CodedOutputStream.ComputeLengthSize(value.Count)+value.Count;
    }

    public void WriteTo(ref WriterContext output, List<byte> value)
    {
        output.WriteLength(value.Count);
        WritingPrimitives.WriteRawBytes(ref output.buffer,ref output.state, CollectionsMarshal.AsSpan(value));
    }
}
public class ByteListProtoParser : IProtoParser<List<byte>>
{
    public static IProtoReader<List<byte>> Reader { get; } = new ByteListProtoReader();
    public static IProtoWriter<List<byte>> Writer { get; } = new ByteListProtoWriter();
}