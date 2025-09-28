namespace LightProto.Parser;

public class KeyValuePairProtoReader<TKey, TValue> : IProtoReader<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public bool IsMessage => true;
    

    private readonly IProtoReader<TKey> _keyReader;
    private readonly IProtoReader<TValue> _valueReader;
    private readonly uint _keyTag;
    private readonly uint _valueTag;

    public KeyValuePairProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        uint keyTag,
        uint valueTag
    )
    {
        _keyReader = keyReader;
        _valueReader = valueReader;
        _keyTag = keyTag;
        _valueTag = valueTag;
    }

    public KeyValuePair<TKey, TValue> ParseFrom(ref ReaderContext ctx)
    {
        TKey key = default!;
        TValue value = default!;
        uint tag;
        while ((tag = ctx.ReadTag()) != 0)
        {
            if ((tag & 7) == 4)
            {
                break;
            }

            if (tag == _keyTag)
            {
                if (_keyReader is ICollectionReader)
                {
                    key = _keyReader.ParseFrom(ref ctx);
                }
                else
                {
                    key = _keyReader.ParseMessageFrom(ref ctx);
                }
            }
            else if (tag == _valueTag)
            {
                if (_valueReader is ICollectionReader)
                {
                    value = _valueReader.ParseFrom(ref ctx);
                }
                else
                {
                    value = _valueReader.ParseMessageFrom(ref ctx);
                }
            }
            else
            {
                continue;
            }
        }

        return new KeyValuePair<TKey, TValue>(key, value);
    }
}

public class KeyValuePairProtoWriter<TKey, TValue> : IProtoWriter<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public bool IsMessage => true;
    

    public int CalculateSize(KeyValuePair<TKey, TValue> value)
    {
        int size = 0;
        if (_keyWriter is ICollectionWriter)
        {
            size += _keyWriter.CalculateMessageSize(value.Key);
        }
        else
        {
            size += CodedOutputStream.ComputeRawVarint32Size(_keyTag);
            size += _keyWriter.CalculateMessageSize(value.Key);
        }
        if (_valueWriter is ICollectionWriter)
        {
            size += _valueWriter.CalculateSize(value.Value);
        }
        else
        {
            size += CodedOutputStream.ComputeRawVarint32Size(_valueTag);
            size += _valueWriter.CalculateMessageSize(value.Value);
        }

        return size;
    }

    public void WriteTo(ref WriterContext output, KeyValuePair<TKey, TValue> pair)
    {
        if (_keyWriter is ICollectionWriter)
        {
            _keyWriter.WriteMessageTo(ref output, pair.Key);
        }
        else
        {
            output.WriteTag(_keyTag);
            _keyWriter.WriteMessageTo(ref output, pair.Key);
        }

        if (_valueWriter is ICollectionWriter)
        {
            _valueWriter.WriteMessageTo(ref output, pair.Value);
        }
        else
        {
            output.WriteTag(_valueTag);
            _valueWriter.WriteMessageTo(ref output, pair.Value);
        }
    }

    private readonly IProtoWriter<TKey> _keyWriter;
    private readonly IProtoWriter<TValue> _valueWriter;
    private readonly uint _keyTag;
    private readonly uint _valueTag;

    public KeyValuePairProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint keyTag,
        uint valueTag
    )
    {
        _keyWriter = keyWriter;
        _valueWriter = valueWriter;
        _keyTag = keyTag;
        _valueTag = valueTag;
    }
}
