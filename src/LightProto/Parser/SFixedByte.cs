namespace LightProto.Parser
{
    public sealed class SFixedByteProtoParser : IProtoParser<sbyte>
    {
        public static IProtoReader<sbyte> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<sbyte> ProtoWriter { get; } = new LightProtoWriter();

        sealed class LightProtoReader : IProtoReader, IProtoReader<sbyte>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public sbyte ParseFrom(ref ReaderContext input)
            {
                return (sbyte)input.ReadSFixed32();
            }
        }

        sealed class LightProtoWriter : IProtoWriter, IProtoWriter<sbyte>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((sbyte)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (sbyte)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(sbyte value)
            {
                return CodedOutputStream.ComputeSFixed32Size(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, sbyte value)
            {
                output.WriteSFixed32(value);
            }
        }
    }
}
