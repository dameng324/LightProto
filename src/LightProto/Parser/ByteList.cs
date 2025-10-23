using System.Runtime.InteropServices;

namespace LightProto.Parser;

public sealed class ByteListProtoParser : IProtoParser<List<byte>>
{
    public static IProtoReader<List<byte>> ProtoReader { get; } = new ByteListProtoReader();
    public static IProtoWriter<List<byte>> ProtoWriter { get; } = new ByteListProtoWriter();

    sealed class ByteListProtoReader : IProtoReader<List<byte>>
    {
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        public List<byte> ParseFrom(ref ReaderContext input)
        {
            var length = input.ReadLength();
            return [.. ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length)];
        }
    }

    sealed class ByteListProtoWriter : IProtoWriter<List<byte>>
    {
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        public void WriteTo(ref WriterContext output, List<byte> value)
        {
            output.WriteBytes(
#if NET5_0_OR_GREATER
                CollectionsMarshal.AsSpan(value)
#else
                value.ToArray()
#endif
            );
        }
    }
}
