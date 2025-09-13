

namespace LightProto.Parser;

public sealed class DoubleProtoReader : IProtoReader<Double>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Double ParseFrom(ref ReaderContext input)
    {
        return input.ReadDouble();
    }
}
public sealed class DoubleProtoWriter : IProtoWriter<Double>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Double pair)
    {
        return CodedOutputStream.ComputeDoubleSize(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Double value)
    {
        output.WriteDouble(value);
    }
}
public sealed class DoubleProtoParser : IProtoParser<Double>
{
    public static IProtoReader<Double> Reader { get; } = new DoubleProtoReader();
    public static IProtoWriter<Double> Writer { get; } = new DoubleProtoWriter();
}