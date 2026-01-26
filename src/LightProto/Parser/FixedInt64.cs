namespace LightProto.Parser
{
    public sealed class Fixed64ProtoParser : IProtoParser<UInt64>
    {
        public static IProtoReader<UInt64> ProtoReader { get; } = new Fixed64ProtoReader();
        public static IProtoWriter<UInt64> ProtoWriter { get; } = new Fixed64ProtoWriter();

        sealed class Fixed64ProtoReader : IProtoReader, IProtoReader<UInt64>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public UInt64 ParseFrom(ref ReaderContext input)
            {
                return input.ReadFixed64();
            }
        }

        sealed class Fixed64ProtoWriter : IProtoWriter, IProtoWriter<UInt64>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((UInt64)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (UInt64)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((UInt64)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(UInt64 value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(UInt64 value)
            {
                return CodedOutputStream.ComputeFixed64Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, UInt64 value)
            {
                output.WriteFixed64(value);
            }
        }
    }
}
