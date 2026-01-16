namespace LightProto.Parser
{
#if NET6_0_OR_GREATER
    public sealed class DateOnlyProtoParser : IProtoParser<DateOnly>
    {
        public static IProtoReader<DateOnly> ProtoReader { get; } = new DateOnlyProtoReader();
        public static IProtoWriter<DateOnly> ProtoWriter { get; } = new DateOnlyProtoWriter();

        sealed class DateOnlyProtoReader : IProtoReader, IProtoReader<DateOnly>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            public DateOnly ParseFrom(ref ReaderContext input)
            {
                return DateOnly.FromDayNumber(input.ReadInt32());
            }
        }

        sealed class DateOnlyProtoWriter : IProtoWriter, IProtoWriter<DateOnly>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((DateOnly)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (DateOnly)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(DateOnly value)
            {
                return CodedOutputStream.ComputeInt32Size(value.DayNumber);
            }

            public void WriteTo(ref WriterContext output, DateOnly value)
            {
                output.WriteInt32(value.DayNumber);
            }
        }
    }
#endif
}
