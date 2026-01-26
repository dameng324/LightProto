namespace LightProto.Parser
{
    public sealed class BooleanProtoParser : IProtoParser<bool>
    {
        public static IProtoReader<bool> ProtoReader { get; } = new BooleanProtoReader();
        public static IProtoWriter<bool> ProtoWriter { get; } = new BooleanProtoWriter();

        sealed class BooleanProtoReader : IProtoReader, IProtoReader<bool>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public bool ParseFrom(ref ReaderContext input)
            {
                return input.ReadBool();
            }
        }

        sealed class BooleanProtoWriter : IProtoWriter, IProtoWriter<bool>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((bool)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (bool)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((bool)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(bool value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(bool value)
            {
                return CodedOutputStream.ComputeBoolSize(value);
            }

            public void WriteTo(ref WriterContext output, bool value)
            {
                output.WriteBool(value);
            }
        }
    }
}
