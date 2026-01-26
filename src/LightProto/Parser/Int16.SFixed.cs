namespace LightProto.Parser
{
    public sealed class SFixed16ProtoParser : IProtoParser<short>
    {
        public static IProtoReader<short> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<short> ProtoWriter { get; } = new LightProtoWriter();

        sealed class LightProtoReader : IProtoReader, IProtoReader<short>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public short ParseFrom(ref ReaderContext input)
            {
                return (short)input.ReadSFixed32();
            }
        }

        sealed class LightProtoWriter : IProtoWriter, IProtoWriter<short>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((short)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (short)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((short)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(short value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(short value)
            {
                return CodedOutputStream.ComputeSFixed32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, short value)
            {
                output.WriteSFixed32(value);
            }
        }
    }
}
