namespace LightProto.Parser;

public sealed class NullableProtoReader<T> : IProtoReader<Nullable<T>>
    where T : struct
{
    public IProtoReader<T> ValueReader { get; }
    public WireFormat.WireType WireType => ValueReader.WireType;

    public NullableProtoReader(IProtoReader<T> valueReader, uint tag, int fixedSize)
    {
        ValueReader = valueReader;
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public Nullable<T> ParseFrom(ref ReaderContext input)
    {
        return ValueReader.ParseMessageFrom(ref input);
    }
}

public sealed class NullableProtoWriter<T> : IProtoWriter<Nullable<T>>
    where T : struct
{
    public IProtoWriter<T> ValueWriter { get; }
    public WireFormat.WireType WireType => ValueWriter.WireType;

    public NullableProtoWriter(IProtoWriter<T> valueWriter, uint tag, int fixedSize)
    {
        ValueWriter = valueWriter;
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int CalculateSize(Nullable<T> value)
    {
        return value.HasValue == false ? 0 : ValueWriter.CalculateMessageSize(value.Value);
    }

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public void WriteTo(ref WriterContext output, Nullable<T> value)
    {
        if (value.HasValue)
        {
            ValueWriter.WriteMessageTo(ref output, value.Value);
        }
    }
}
