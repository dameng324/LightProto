namespace LightProto.Parser;

#if NET6_0_OR_GREATER
public sealed class DateOnlyProtoParser : IProtoParser<DateOnly>
{
    public static IProtoReader<DateOnly> ProtoReader { get; } = new DateOnlyProtoReader();
    public static IProtoWriter<DateOnly> ProtoWriter { get; } = new DateOnlyProtoWriter();

    sealed class DateOnlyProtoReader : IProtoReader<DateOnly>
    {
        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        public DateOnly ParseFrom(ref ReaderContext input)
        {
            return DateOnly.FromDayNumber(input.ReadInt32());
        }
    }

    sealed class DateOnlyProtoWriter : IProtoWriter<DateOnly>
    {
        public WireFormat.WireType WireType => WireFormat.WireType.Varint;
        public bool IsMessage => false;

        public void WriteTo(ref WriterContext output, DateOnly value)
        {
            output.WriteInt32(value.DayNumber);
        }
    }
}
#endif
