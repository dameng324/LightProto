namespace LightProto.Parser
{
    public sealed class NullableProtoReader<T> : IProtoReader, IProtoReader<T?>
        where T : struct
    {
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input)!;

        public IProtoReader<T> ValueReader { get; }
        public WireFormat.WireType WireType => ValueReader.WireType;

        public NullableProtoReader(IProtoReader<T> valueReader)
        {
            ValueReader = valueReader;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
        )]
        public T? ParseFrom(ref ReaderContext input)
        {
            return ValueReader.ParseMessageFrom(ref input);
        }
    }

    public sealed class NullableProtoWriter<T> : IProtoWriter, IProtoWriter<T?>
        where T : struct
    {
        public IProtoWriter<T> ValueWriter { get; }
        public WireFormat.WireType WireType => ValueWriter.WireType;
        public bool IsMessage => false;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((T?)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
            WriteTo(ref output, (T?)value);

        public NullableProtoWriter(IProtoWriter<T> valueWriter)
        {
            ValueWriter = valueWriter;
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
        )]
        public int CalculateSize(T? value)
        {
            return !value.HasValue ? 0 : ValueWriter.CalculateMessageSize(value.Value);
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
        )]
        public void WriteTo(ref WriterContext output, T? value)
        {
            if (value.HasValue)
            {
                ValueWriter.WriteMessageTo(ref output, value.Value);
            }
        }
    }
}
