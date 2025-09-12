

namespace Dameng.Protobuf.Parser;

public sealed class NullableProtoReader<T> : IProtoReader<Nullable<T>>
    where T : struct
{
    public IProtoReader<T> ValueReader { get; }

    public NullableProtoReader(IProtoReader<T> valueReader,uint tag,int fixedSize)
    {
        ValueReader = valueReader;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Nullable<T> ParseFrom(ref ReaderContext input)
    {
        return ValueReader.ParseFrom(ref input);
    }
}

public sealed class NullableProtoWriter<T> : IProtoWriter<Nullable<T>>
    where T : struct
{
    public IProtoWriter<T> ValueWriter { get; }

    public NullableProtoWriter(IProtoWriter<T> valueWriter,uint tag,int fixedSize)
    {
        ValueWriter = valueWriter;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Nullable<T> pair)
    {
        return pair.HasValue == false ? 0 : ValueWriter.CalculateSize(pair.Value);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Nullable<T> value)
    {
        if (value.HasValue)
        {
            ValueWriter.WriteTo(ref output, value.Value);
        }
    }
}