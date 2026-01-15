namespace LightProto.Parser
{
    public sealed class CharProtoParser : IProtoParser<Char>
    {
        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        CharProtoParser() { }

        public static IProtoReader<Char> ProtoReader { get; } = new CharProtoReader();
        public static IProtoWriter<Char> ProtoWriter { get; } = new CharProtoWriter();

        sealed class CharProtoReader : IProtoReader, IProtoReader<Char>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public Char ParseFrom(ref ReaderContext input)
            {
                return (Char)input.ReadUInt32();
            }
        }

        sealed class CharProtoWriter : IProtoWriter, IProtoWriter<Char>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Char)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (Char)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public int CalculateSize(Char value)
            {
                return CodedOutputStream.ComputeUInt32Size(value);
            }

            public void WriteTo(ref WriterContext output, Char value)
            {
                output.WriteUInt32(value);
            }
        }
    }
}
