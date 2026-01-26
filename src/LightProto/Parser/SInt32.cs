namespace LightProto.Parser
{
    public sealed class SInt32ProtoParser : IProtoParser<Int32>
    {
        public static IProtoReader<Int32> ProtoReader { get; } = new SInt32ProtoReader();
        public static IProtoWriter<Int32> ProtoWriter { get; } = new SInt32ProtoWriter();

        sealed class SInt32ProtoReader : IProtoReader, IProtoReader<Int32>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public Int32 ParseFrom(ref ReaderContext input)
            {
                return input.ReadSInt32();
            }
        }

        sealed class SInt32ProtoWriter : IProtoWriter, IProtoWriter<Int32>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Int32)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Int32)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((Int32)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(Int32 value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(Int32 value)
            {
                return CodedOutputStream.ComputeSInt32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, Int32 value)
            {
                output.WriteSInt32(value);
            }
        }
    }
}
