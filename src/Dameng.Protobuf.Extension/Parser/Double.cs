using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class DoubleProtoReader : IProtoReader<Double>
{
    public Double ParseFrom(ref ParseContext input)
    {
        return input.ReadDouble();
    }
}
public class DoubleProtoWriter : IProtoWriter<Double>
{
    public int CalculateSize(Double value)
    {
        return CodedOutputStream.ComputeDoubleSize(value);
    }

    public void WriteTo(ref WriteContext output, Double value)
    {
        output.WriteDouble(value);
    }
}
public class DoubleProtoParser : IProtoParser<Double>
{
    public static IProtoReader<Double> Reader { get; } = new DoubleProtoReader();
    public static IProtoWriter<Double> Writer { get; } = new DoubleProtoWriter();
}