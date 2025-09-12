

namespace Dameng.Protobuf.Parser;

public sealed class Fixed32ProtoReader : IProtoReader<UInt32>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public UInt32 ParseFrom(ref ReaderContext input)
    {
        return input.ReadFixed32();
    }
}
public sealed class Fixed32ProtoWriter : IProtoWriter<UInt32>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(UInt32 pair)
    {
        return CodedOutputStream.ComputeFixed32Size(pair);
    }
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]

    public void WriteTo(ref WriterContext output, UInt32 value)
    {
        output.WriteFixed32(value);
    }
}
public sealed class Fixed32ProtoParser : IProtoParser<UInt32>
{
    public static IProtoReader<UInt32> Reader { get; } = new Fixed32ProtoReader();
    public static IProtoWriter<UInt32> Writer { get; } = new Fixed32ProtoWriter();
}