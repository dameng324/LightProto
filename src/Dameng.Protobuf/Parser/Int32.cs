

namespace Dameng.Protobuf.Parser;

public sealed class Int32ProtoReader : IProtoReader<int>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int ParseFrom(ref ReaderContext input)
    {
        return input.ReadInt32();
    }
}
public sealed class Int32ProtoWriter : IProtoWriter<int>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(int pair)
    {
        return CodedOutputStream.ComputeInt32Size(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, int value)
    {
        output.WriteInt32(value);
    }
}
public sealed class Int32ProtoParser : IProtoParser<int>
{
    public static IProtoReader<int> Reader { get; } = new Int32ProtoReader();
    public static IProtoWriter<int> Writer { get; } = new Int32ProtoWriter();
}