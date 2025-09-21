using System.Buffers.Binary;

namespace LightProto.Parser;

public sealed class Guid300ProtoReader : IProtoReader<Guid>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

    public Guid ParseFrom(ref ReaderContext input)
    {
        var str = input.ReadString();
        return Guid.Parse(str);
    }
}

public sealed class Guid300ProtoWriter : IProtoWriter<Guid>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

    public int CalculateSize(Guid value)
    {
        return CodedOutputStream.ComputeStringSize(value.ToString());
    }

    public void WriteTo(ref WriterContext output, Guid value)
    {
        output.WriteString(value.ToString());
    }
}

public sealed class Guid300ProtoParser : IProtoParser<Guid>
{
    public static IProtoReader<Guid> ProtoReader { get; } = new Guid300ProtoReader();
    public static IProtoWriter<Guid> ProtoWriter { get; } = new Guid300ProtoWriter();
}
