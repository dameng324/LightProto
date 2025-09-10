

namespace Dameng.Protobuf.Parser;

public class BooleanProtoReader : IProtoReader<bool>
{
    public bool ParseFrom(ref ReaderContext input)
    {
        return input.ReadBool();
    }
}
public class BooleanProtoWriter : IProtoWriter<bool>
{
    public int CalculateSize(bool value)
    {
        return CodedOutputStream.ComputeBoolSize(value);
    }

    public void WriteTo(ref WriterContext output, bool value)
    {
        output.WriteBool(value);
    }
}
public class BooleanProtoParser : IProtoParser<bool>
{
    public static IProtoReader<bool> Reader { get; } = new BooleanProtoReader();
    public static IProtoWriter<bool> Writer { get; } = new BooleanProtoWriter();
}
