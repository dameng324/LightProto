namespace LightProto.Parser
{
    public sealed class SFixed64ProtoParser : IProtoParser<Int64>
    {
        public static IProtoReader<Int64> ProtoReader { get; } = new SFixed64ProtoReader();
        public static IProtoWriter<Int64> ProtoWriter { get; } = new SFixed64ProtoWriter();

        sealed class SFixed64ProtoReader : IProtoReader, IProtoReader<Int64>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public Int64 ParseFrom(ref ReaderContext input)
            {
                return input.ReadSFixed64();
            }
        }

        sealed class SFixed64ProtoWriter : IProtoWriter, IProtoWriter<Int64>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Int64)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Int64)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((Int64)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(Int64 value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(Int64 value)
            {
                return CodedOutputStream.ComputeSFixed64Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, Int64 value)
            {
                output.WriteSFixed64(value);
            }
        }
    }
}
