

namespace Dameng.Protobuf.Parser;

public class SingleProtoReader : IProtoReader<Single>
{
    public Single ParseFrom(ref ReaderContext input)
    {
        return input.ReadFloat();
    }
}
public class SingleProtoWriter : IProtoWriter<Single>
{
    public int CalculateSize(Single value)
    {
        return CodedOutputStream.ComputeFloatSize(value);
    }

    public void WriteTo(ref WriterContext output, Single value)
    {
        output.WriteFloat(value);
    }
}
public class SingleProtoParser : IProtoParser<Single>
{
    public static IProtoReader<Single> Reader { get; } = new SingleProtoReader();
    public static IProtoWriter<Single> Writer { get; } = new SingleProtoWriter();
}