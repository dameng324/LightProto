namespace LightProto.Parser
{
    public sealed class UInt64ProtoParser : IProtoParser<UInt64>
    {
        public static IProtoReader<UInt64> ProtoReader { get; } = new UInt64ProtoReader();
        public static IProtoWriter<UInt64> ProtoWriter { get; } = new UInt64ProtoWriter();

        sealed class UInt64ProtoReader : IProtoReader, IProtoReader<UInt64>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public UInt64 ParseFrom(ref ReaderContext input)
            {
                return input.ReadUInt64();
            }
        }

        sealed class UInt64ProtoWriter : IProtoWriter, IProtoWriter<UInt64>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((UInt64)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (UInt64)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public int CalculateSize(UInt64 value)
            {
                return CodedOutputStream.ComputeUInt64Size(value);
            }

            public void WriteTo(ref WriterContext output, UInt64 value)
            {
                output.WriteUInt64(value);
            }
        }
    }
}
