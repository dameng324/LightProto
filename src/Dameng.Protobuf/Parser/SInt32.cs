

namespace Dameng.Protobuf.Parser;

public sealed class SInt32ProtoReader : IProtoReader<Int32>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Int32 ParseFrom(ref ReaderContext input)
    {
        return input.ReadSInt32();
    }
}
public sealed class SInt32ProtoWriter : IProtoWriter<Int32>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Int32 pair)
    {
        return CodedOutputStream.ComputeSInt32Size(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Int32 value)
    {
        output.WriteSInt32(value);
    }
}
public sealed class SInt32ProtoParser : IProtoParser<Int32>
{
    public static IProtoReader<Int32> Reader { get; } = new SInt32ProtoReader();
    public static IProtoWriter<Int32> Writer { get; } = new SInt32ProtoWriter();
}