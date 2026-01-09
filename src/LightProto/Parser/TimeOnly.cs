namespace LightProto.Parser;

#if NET6_0_OR_GREATER
public sealed class TimeOnlyProtoParser : IProtoParser<TimeOnly>
{
    public static IProtoReader<TimeOnly> ProtoReader { get; } = new TimeOnlyProtoReader();
    public static IProtoWriter<TimeOnly> ProtoWriter { get; } = new TimeOnlyProtoWriter();

    sealed class TimeOnlyProtoReader : IProtoReader, IProtoReader<TimeOnly>
    {
        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        public TimeOnly ParseFrom(ref ReaderContext input)
        {
            return new TimeOnly(input.ReadInt64());
        }
    }

    sealed class TimeOnlyProtoWriter : IProtoWriter, IProtoWriter<TimeOnly>
    {
        int IProtoWriter.CalculateSize(object value) => CalculateSize((TimeOnly)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
            WriteTo(ref output, (TimeOnly)value);

        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        public int CalculateSize(TimeOnly value)
        {
            return CodedOutputStream.ComputeInt64Size(value.Ticks);
        }

        public void WriteTo(ref WriterContext output, TimeOnly value)
        {
            output.WriteInt64(value.Ticks);
        }
    }
}
#endif
