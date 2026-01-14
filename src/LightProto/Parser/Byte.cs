namespace LightProto.Parser
{
    public sealed class ByteProtoParser : IProtoParser<byte>
    {
        public static IProtoReader<byte> ProtoReader { get; } = new ByteProtoReader();
        public static IProtoWriter<byte> ProtoWriter { get; } = new ByteProtoWriter();

        sealed class ByteProtoReader : IProtoReader, IProtoReader<byte>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public byte ParseFrom(ref ReaderContext input)
            {
                return input.ReadByte();
            }
        }

        sealed class ByteProtoWriter : IProtoWriter, IProtoWriter<byte>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((byte)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (byte)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public int CalculateSize(byte value)
            {
                return CodedOutputStream.ComputeByteSize(value);
            }

            public void WriteTo(ref WriterContext output, byte value)
            {
                output.WriteByte(value);
            }
        }
    }
}
