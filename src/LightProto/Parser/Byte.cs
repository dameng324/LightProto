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
                return (byte)input.ReadUInt32();
            }
        }

        sealed class ByteProtoWriter : IProtoWriter, IProtoWriter<byte>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((byte)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (byte)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((byte)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(byte value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(byte value)
            {
                return CodedOutputStream.ComputeUInt32Size(value);
            }

            public void WriteTo(ref WriterContext output, byte value)
            {
                output.WriteUInt32(value);
            }
        }
    }
}
