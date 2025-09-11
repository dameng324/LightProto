

namespace Dameng.Protobuf.Parser;

public sealed class DateOnlyProtoReader : IProtoReader<DateOnly>
{
    public DateOnly ParseFrom(ref ReaderContext input)
    {
        return DateOnly.FromDayNumber(input.ReadInt32());
    }
}
public sealed class DateOnlyProtoWriter : IProtoWriter<DateOnly>
{
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
public sealed class DateOnlyProtoParser : IProtoParser<DateOnly>
{
    public static IProtoReader<DateOnly> Reader { get; } = new DateOnlyProtoReader();
    public static IProtoWriter<DateOnly> Writer { get; } = new DateOnlyProtoWriter();
}