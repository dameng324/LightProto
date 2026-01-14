namespace LightProto.Parser
{
    public sealed class SByteProtoParser : IProtoParser<sbyte>
    {
        public static IProtoReader<sbyte> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<sbyte> ProtoWriter { get; } = new LightProtoWriter();

        sealed class LightProtoReader : IProtoReader, IProtoReader<sbyte>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public sbyte ParseFrom(ref ReaderContext input)
            {
                return (sbyte)input.ReadInt32();
            }
        }

        sealed class LightProtoWriter : IProtoWriter, IProtoWriter<sbyte>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((sbyte)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (sbyte)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public int CalculateSize(sbyte value)
            {
                return CodedOutputStream.ComputeInt32Size(value);
            }

            public void WriteTo(ref WriterContext output, sbyte value)
            {
                output.WriteInt32(value);
            }
        }
    }
}
