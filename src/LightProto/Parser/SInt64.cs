namespace LightProto.Parser;

public sealed class SInt64ProtoReader : IProtoReader<Int64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Varint;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public Int64 ParseFrom(ref ReaderContext input)
    {
        return input.ReadSInt64();
    }
}

public sealed class SInt64ProtoWriter : IProtoWriter<Int64>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Varint;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int CalculateSize(Int64 value)
    {
        return CodedOutputStream.ComputeSInt64Size(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public void WriteTo(ref WriterContext output, Int64 value)
    {
        output.WriteSInt64(value);
    }
}

public sealed class SInt64ProtoParser : IProtoParser<Int64>
{
    public static IProtoReader<Int64> ProtoReader { get; } = new SInt64ProtoReader();
    public static IProtoWriter<Int64> ProtoWriter { get; } = new SInt64ProtoWriter();
}
