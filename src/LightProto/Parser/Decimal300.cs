using System.Buffers.Binary;
using System.Globalization;

namespace LightProto.Parser;

public sealed class Decimal300ProtoReader : IProtoReader<Decimal>
{
    public Decimal ParseFrom(ref ReaderContext input)
    {
        var str= input.ReadString();
        return Decimal.Parse(str);
    }
}
public sealed class Decimal300ProtoWriter : IProtoWriter<Decimal>
{
    public int CalculateSize(Decimal value)
    {
        return CodedOutputStream.ComputeStringSize(value.ToString("G"));
    }

    public void WriteTo(ref WriterContext output, Decimal value)
    {
        output.WriteString(value.ToString("G"));
    }
}
public sealed class Decimal300ProtoParser : IProtoParser<Decimal>
{
    public static IProtoReader<Decimal> Reader { get; } = new Decimal300ProtoReader();
    public static IProtoWriter<Decimal> Writer { get; } = new Decimal300ProtoWriter();
}