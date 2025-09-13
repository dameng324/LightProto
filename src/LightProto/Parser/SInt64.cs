

namespace LightProto.Parser;

public sealed class SInt64ProtoReader : IProtoReader<Int64>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Int64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadSInt64();
    }
}
public sealed class SInt64ProtoWriter : IProtoWriter<Int64>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Int64 pair)
    {
        return CodedOutputStream.ComputeSInt64Size(pair);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Int64 value)
    {
        output.WriteSInt64(value);
    }
}
public sealed class SInt64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new SInt64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new SInt64ProtoWriter();
}