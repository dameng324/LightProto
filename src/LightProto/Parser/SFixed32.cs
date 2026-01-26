namespace LightProto.Parser
{
    public sealed class SFixed32ProtoParser : IProtoParser<int>
    {
        public static IProtoReader<int> ProtoReader { get; } = new SFixed32ProtoReader();
        public static IProtoWriter<int> ProtoWriter { get; } = new SFixed32ProtoWriter();

        sealed class SFixed32ProtoReader : IProtoReader, IProtoReader<int>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int ParseFrom(ref ReaderContext input)
            {
                return input.ReadSFixed32();
            }
        }

        sealed class SFixed32ProtoWriter : IProtoWriter, IProtoWriter<int>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((int)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (int)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((int)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(int value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(int value)
            {
                return CodedOutputStream.ComputeSFixed32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, int value)
            {
                output.WriteSFixed32(value);
            }
        }
    }
}
