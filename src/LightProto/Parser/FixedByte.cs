namespace LightProto.Parser
{
    public sealed class FixedByteProtoParser : IProtoParser<byte>
    {
        public static IProtoReader<byte> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<byte> ProtoWriter { get; } = new LightProtoWriter();

        sealed class LightProtoReader : IProtoReader, IProtoReader<byte>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public byte ParseFrom(ref ReaderContext input)
            {
                return (byte)input.ReadFixed32();
            }
        }

        sealed class LightProtoWriter : IProtoWriter, IProtoWriter<byte>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((byte)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (byte)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public int CalculateSize(byte value)
            {
                return CodedOutputStream.ComputeFixed32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public void WriteTo(ref WriterContext output, byte value)
            {
                output.WriteFixed32(value);
            }
        }
    }
}
