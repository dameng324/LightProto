using System.Runtime.CompilerServices;

namespace LightProto.Parser;

public sealed class EnumProtoParser<T> : IProtoParser<T>
    where T : Enum
{
    public static IProtoReader<T> ProtoReader { get; } = new EnumProtoReader();
    public static IProtoWriter<T> ProtoWriter { get; } = new EnumProtoWriter();

    sealed class EnumProtoReader : IProtoReader, IProtoReader<T>
    {
        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ParseFrom(ref ReaderContext input)
        {
            var value = input.ReadEnum();
            return Unsafe.As<int, T>(ref value);
        }
    }

    sealed class EnumProtoWriter : IProtoWriter, IProtoWriter<T>
    {
        int IProtoWriter.CalculateSize(object value) => CalculateSize((T)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
            WriteTo(ref output, (T)value);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculateSize(T value)
        {
            return CodedOutputStream.ComputeEnumSize(Unsafe.As<T, int>(ref value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ref WriterContext output, T value)
        {
            output.WriteEnum(Unsafe.As<T, int>(ref value));
        }
    }
}
