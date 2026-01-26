namespace LightProto.Parser
{
    public sealed class Int32ProtoParser : IProtoParser<int>
    {
        public static IProtoReader<int> ProtoReader { get; } = new Int32ProtoReader();
        public static IProtoWriter<int> ProtoWriter { get; } = new Int32ProtoWriter();

        sealed class Int32ProtoReader : IProtoReader, IProtoReader<int>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int ParseFrom(ref ReaderContext input)
            {
                return input.ReadInt32();
            }
        }

        sealed class Int32ProtoWriter : IProtoWriter, IProtoWriter<int>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((int)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (int)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((int)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(int value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(int value)
            {
                return CodedOutputStream.ComputeInt32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, int value)
            {
                output.WriteInt32(value);
            }
        }
    }
}
