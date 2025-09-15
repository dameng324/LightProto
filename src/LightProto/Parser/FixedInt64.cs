

namespace LightProto.Parser;

public sealed class Fixed64ProtoReader : IProtoReader<UInt64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public UInt64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadFixed64();
    }
}
public sealed class Fixed64ProtoWriter : IProtoWriter<UInt64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(UInt64 value)
    {
        return CodedOutputStream.ComputeFixed64Size(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, UInt64 value)
    {
        output.WriteFixed64(value);
    }
}
public sealed class Fixed64ProtoParser : IProtoParser<UInt64>
{
    public static IProtoReader<UInt64> Reader { get; } = new Fixed64ProtoReader();
    public static IProtoWriter<UInt64> Writer { get; } = new Fixed64ProtoWriter();
}