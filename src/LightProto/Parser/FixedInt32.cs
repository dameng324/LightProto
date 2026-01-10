namespace LightProto.Parser
{
    public sealed class Fixed32ProtoParser : IProtoParser<UInt32>
    {
        public static IProtoReader<UInt32> ProtoReader { get; } = new Fixed32ProtoReader();
        public static IProtoWriter<UInt32> ProtoWriter { get; } = new Fixed32ProtoWriter();

        sealed class Fixed32ProtoReader : IProtoReader, IProtoReader<UInt32>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public UInt32 ParseFrom(ref ReaderContext input)
            {
                return input.ReadFixed32();
            }
        }

        sealed class Fixed32ProtoWriter : IProtoWriter, IProtoWriter<UInt32>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((UInt32)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (UInt32)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public int CalculateSize(UInt32 value)
            {
                return CodedOutputStream.ComputeFixed32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public void WriteTo(ref WriterContext output, UInt32 value)
            {
                output.WriteFixed32(value);
            }
        }
    }
}
