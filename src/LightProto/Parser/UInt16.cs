namespace LightProto.Parser
{
    public sealed class UInt16ProtoParser : IProtoParser<UInt16>
    {
        public static IProtoReader<UInt16> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<UInt16> ProtoWriter { get; } = new LightProtoWriter();

        sealed class LightProtoReader : IProtoReader, IProtoReader<UInt16>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public UInt16 ParseFrom(ref ReaderContext input)
            {
                return (UInt16)input.ReadUInt32();
            }
        }

        sealed class LightProtoWriter : IProtoWriter, IProtoWriter<UInt16>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((UInt16)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (UInt16)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((UInt16)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(UInt16 value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(UInt16 value)
            {
                return CodedOutputStream.ComputeUInt32Size(value);
            }

            public void WriteTo(ref WriterContext output, UInt16 value)
            {
                output.WriteUInt32(value);
            }
        }
    }
}
