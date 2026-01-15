namespace LightProto.Parser
{
    public sealed class SInt16ProtoParser : IProtoParser<Int16>
    {
        public static IProtoReader<Int16> ProtoReader { get; } = new SInt16ProtoReader();
        public static IProtoWriter<Int16> ProtoWriter { get; } = new SInt16ProtoWriter();

        sealed class SInt16ProtoReader : IProtoReader, IProtoReader<Int16>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public Int16 ParseFrom(ref ReaderContext input)
            {
                return (Int16)input.ReadSInt32();
            }
        }

        sealed class SInt16ProtoWriter : IProtoWriter, IProtoWriter<Int16>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Int16)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (Int16)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public int CalculateSize(Int16 value)
            {
                return CodedOutputStream.ComputeSInt32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public void WriteTo(ref WriterContext output, Int16 value)
            {
                output.WriteSInt32(value);
            }
        }
    }
}
