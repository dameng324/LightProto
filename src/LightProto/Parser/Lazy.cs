using System.Diagnostics.CodeAnalysis;

namespace LightProto.Parser
{
    public sealed class LazyProtoReader<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
        T> : IProtoReader, IProtoReader<Lazy<T>>
    {
        public bool IsMessage => ValueReader.IsMessage;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public IProtoReader<T> ValueReader { get; }
        public WireFormat.WireType WireType => ValueReader.WireType;

        public LazyProtoReader(IProtoReader<T> valueReader)
        {
            ValueReader = valueReader;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public Lazy<T> ParseFrom(ref ReaderContext input)
        {
            var t = ValueReader.ParseMessageFrom(ref input);
            return new Lazy<T>(() => t);
        }
    }

    public sealed class LazyProtoWriter<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
        T> : IProtoWriter, IProtoWriter<Lazy<T>>
    {
        int IProtoWriter.CalculateSize(object value) => CalculateSize((Lazy<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Lazy<T>)value);

        public IProtoWriter<T> ValueWriter { get; }
        public WireFormat.WireType WireType => ValueWriter.WireType;
        public bool IsMessage => ValueWriter.IsMessage;

        public LazyProtoWriter(IProtoWriter<T> valueWriter)
        {
            ValueWriter = valueWriter;
        }

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((Lazy<T>)value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CalculateSize(Lazy<T> value)
        {
            var longSize = CalculateLongSize(value);
            if (longSize > int.MaxValue)
            {
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            }
            return (int)longSize;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long CalculateLongSize(Lazy<T> value)
        {
            return ValueWriter.CalculateLongMessageSize(value.Value);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ref WriterContext output, Lazy<T> value)
        {
            ValueWriter.WriteMessageTo(ref output, value.Value);
        }
    }
}
