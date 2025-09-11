

namespace Dameng.Protobuf.Parser;

public sealed class Int64ProtoReader : IProtoReader<Int64>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Int64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadInt64();
    }
}
public sealed class Int64ProtoWriter : IProtoWriter<Int64>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeInt64Size(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Int64 value)
    {
        output.WriteInt64(value);
    }
}
public sealed class Int64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new Int64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new Int64ProtoWriter();
}