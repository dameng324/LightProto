namespace LightProto.Parser
{
    public sealed class UInt32ProtoParser : IProtoParser<UInt32>
    {
        public static IProtoReader<UInt32> ProtoReader { get; } = new UInt32ProtoReader();
        public static IProtoWriter<UInt32> ProtoWriter { get; } = new UInt32ProtoWriter();

        sealed class UInt32ProtoReader : IProtoReader, IProtoReader<UInt32>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public UInt32 ParseFrom(ref ReaderContext input)
            {
                return input.ReadUInt32();
            }
        }

        sealed class UInt32ProtoWriter : IProtoWriter, IProtoWriter<UInt32>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((UInt32)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (UInt32)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((UInt32)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(UInt32 value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(UInt32 value)
            {
                return CodedOutputStream.ComputeUInt32Size(value);
            }

            public void WriteTo(ref WriterContext output, UInt32 value)
            {
                output.WriteUInt32(value);
            }
        }
    }
}
