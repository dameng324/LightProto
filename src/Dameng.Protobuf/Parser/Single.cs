

namespace Dameng.Protobuf.Parser;

public sealed class SingleProtoReader : IProtoReader<Single>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Single ParseFrom(ref ReaderContext input)
    {
        return input.ReadFloat();
    }
}
public sealed class SingleProtoWriter : IProtoWriter<Single>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Single pair)
    {
        return CodedOutputStream.ComputeFloatSize(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Single value)
    {
        output.WriteFloat(value);
    }
}
public sealed class SingleProtoParser : IProtoParser<Single>
{
    public static IProtoReader<Single> Reader { get; } = new SingleProtoReader();
    public static IProtoWriter<Single> Writer { get; } = new SingleProtoWriter();
}