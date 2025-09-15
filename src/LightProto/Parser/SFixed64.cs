

namespace LightProto.Parser;

public sealed class SFixed64ProtoReader : IProtoReader<Int64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Int64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadSFixed64();
    }
}
public sealed class SFixed64ProtoWriter : IProtoWriter<Int64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed64;
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeSFixed64Size(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Int64 value)
    {
        output.WriteSFixed64(value);
    }
}
public sealed class SFixed64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> Reader { get; } = new SFixed64ProtoReader();
    public static IProtoWriter<Int64> Writer { get; } = new SFixed64ProtoWriter();
}