namespace LightProto.Parser;

public sealed class SFixed32ProtoReader : IProtoReader<int>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int ParseFrom(ref ReaderContext input)
    {
        return input.ReadSFixed32();
    }
}

public sealed class SFixed32ProtoWriter : IProtoWriter<int>
{
    public WireFormat.WireType WireType => WireFormat.WireType.Fixed32;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int CalculateSize(int value)
    {
        return CodedOutputStream.ComputeSFixed32Size(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
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
