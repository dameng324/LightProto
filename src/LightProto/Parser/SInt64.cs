namespace LightProto.Parser
{
    public sealed class SInt64ProtoParser : IProtoParser<Int64>
    {
        public static IProtoReader<Int64> ProtoReader { get; } = new SInt64ProtoReader();
        public static IProtoWriter<Int64> ProtoWriter { get; } = new SInt64ProtoWriter();

        sealed class SInt64ProtoReader : IProtoReader, IProtoReader<Int64>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public Int64 ParseFrom(ref ReaderContext input)
            {
                return input.ReadSInt64();
            }
        }

        sealed class SInt64ProtoWriter : IProtoWriter, IProtoWriter<Int64>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Int64)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (Int64)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public int CalculateSize(Int64 value)
            {
                return CodedOutputStream.ComputeSInt64Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public void WriteTo(ref WriterContext output, Int64 value)
            {
                output.WriteSInt64(value);
            }
        }
    }
}
