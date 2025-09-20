namespace LightProto.Parser;

public sealed class DateOnlyProtoReader : IProtoReader<DateOnly>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Varint;

    public DateOnly ParseFrom(ref ReaderContext input)
    {
        return DateOnly.FromDayNumber(input.ReadInt32());
    }
}

public sealed class DateOnlyProtoWriter : IProtoWriter<DateOnly>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Varint;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int CalculateSize(DateOnly value)
    {
        return CodedOutputStream.ComputeInt32Size(value.DayNumber);
    }

    public void WriteTo(ref WriterContext output, DateOnly value)
    {
        output.WriteInt32(value.DayNumber);
    }
}

public sealed class DateOnlyProtoParser : IProtoParser<DateOnly>
{
    public static IProtoReader<DateOnly> ProtoReader { get; } = new DateOnlyProtoReader();
    public static IProtoWriter<DateOnly> ProtoWriter { get; } = new DateOnlyProtoWriter();
}
