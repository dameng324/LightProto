
using System.Runtime.InteropServices;

namespace Dameng.Protobuf.Parser;

public sealed class ByteListProtoReader : IProtoReader<List<byte>>
{
    public List<byte> ParseFrom(ref ReaderContext input)
    {
        var length = input.ReadLength();
        return [..ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length)];
    }
}
public sealed class ByteListProtoWriter : IProtoWriter<List<byte>>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(List<byte> pair)
    {
        return CodedOutputStream.ComputeLengthSize(pair.Count)+pair.Count;
    }

    public void WriteTo(ref WriterContext output, List<byte> value)
    {
        output.WriteLength(value.Count);
        WritingPrimitives.WriteRawBytes(ref output.buffer,ref output.state, CollectionsMarshal.AsSpan(value));
    }
}
public sealed class ByteListProtoParser : IProtoParser<List<byte>>
{
    public static IProtoReader<List<byte>> Reader { get; } = new ByteListProtoReader();
    public static IProtoWriter<List<byte>> Writer { get; } = new ByteListProtoWriter();
}