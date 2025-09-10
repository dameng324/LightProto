

namespace Dameng.Protobuf.Parser;

public class NullableProtoReader<T> : IProtoReader<Nullable<T>>
    where T : struct
{
    public IProtoReader<T> ValueReader { get; }

    public NullableProtoReader(IProtoReader<T> valueReader,uint tag,int fixedSize)
    {
        ValueReader = valueReader;
    }

    public Nullable<T> ParseFrom(ref ReaderContext input)
    {
        return ValueReader.ParseFrom(ref input);
    }
}

public class NullableProtoWriter<T> : IProtoWriter<Nullable<T>>
    where T : struct
{
    public IProtoWriter<T> ValueWriter { get; }

    public NullableProtoWriter(IProtoWriter<T> valueWriter,uint tag,int fixedSize)
    {
        ValueWriter = valueWriter;
    }

    public int CalculateSize(Nullable<T> value)
    {
        return value.HasValue == false ? 0 : ValueWriter.CalculateSize(value.Value);
    }

    public void WriteTo(ref WriterContext output, Nullable<T> value)
    {
        if (value.HasValue)
        {
            ValueWriter.WriteTo(ref output, value.Value);
        }
    }
}