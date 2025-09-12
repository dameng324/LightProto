

namespace Dameng.Protobuf.Parser;

public sealed class SFixed32ProtoReader : IProtoReader<int>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int ParseFrom(ref ReaderContext input)
    {
        return input.ReadSFixed32();
    }
}
public sealed class SFixed32ProtoWriter : IProtoWriter<int>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(int pair)
    {
        return CodedOutputStream.ComputeSFixed32Size(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, int value)
    {
        output.WriteSFixed32(value);
    }
}
public sealed class SFixed32ProtoParser : IProtoParser<int>
{
    public static IProtoReader<int> Reader { get; } = new SFixed32ProtoReader();
    public static IProtoWriter<int> Writer { get; } = new SFixed32ProtoWriter();
}