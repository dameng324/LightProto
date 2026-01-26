#if NET5_0_OR_GREATER
namespace LightProto.Parser;

public sealed class RuneProtoParser : IProtoParser<System.Text.Rune>
{
    public static IProtoReader<System.Text.Rune> ProtoReader { get; } = new RuneProtoReader();
    public static IProtoWriter<System.Text.Rune> ProtoWriter { get; } = new RuneProtoWriter();

    sealed class RuneProtoReader : IProtoReader, IProtoReader<System.Text.Rune>
    {
        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        public System.Text.Rune ParseFrom(ref ReaderContext input)
        {
            return new System.Text.Rune(input.ReadUInt32());
        }
    }

    sealed class RuneProtoWriter : IProtoWriter, IProtoWriter<System.Text.Rune>
    {
        int IProtoWriter.CalculateSize(object value) => CalculateSize((System.Text.Rune)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (System.Text.Rune)value);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((System.Text.Rune)value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CalculateSize(System.Text.Rune value) => (int)CalculateLongSize(value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long CalculateLongSize(System.Text.Rune value)
        {
            return CodedOutputStream.ComputeUInt32Size((uint)value.Value);
        }

        public void WriteTo(ref WriterContext output, System.Text.Rune value)
        {
            output.WriteUInt32((uint)value.Value);
        }
    }
}
#endif
