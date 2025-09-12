

namespace Dameng.Protobuf.Parser;

public sealed class BooleanProtoReader : IProtoReader<bool>
{
    public bool ParseFrom(ref ReaderContext input)
    {
        return input.ReadBool();
    }
}
public sealed class BooleanProtoWriter : IProtoWriter<bool>
{
    public int CalculateSize(bool pair)
    {
        return CodedOutputStream.ComputeBoolSize(pair);
    }

    public void WriteTo(ref WriterContext output, bool value)
    {
        output.WriteBool(value);
    }
}
public sealed class BooleanProtoParser : IProtoParser<bool>
{
    public static IProtoReader<bool> Reader { get; } = new BooleanProtoReader();
    public static IProtoWriter<bool> Writer { get; } = new BooleanProtoWriter();
}
