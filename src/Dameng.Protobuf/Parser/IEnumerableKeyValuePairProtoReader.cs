namespace Dameng.Protobuf.Parser;

public class IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue>
    : IProtoReader<TDictionary>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private readonly IProtoReader<TKey> _keyReader;
    private readonly IProtoReader<TValue> _valueReader;
    private readonly uint _tag;
    private readonly uint _keyTag;
    private readonly uint _valueTag;
    private readonly Action<TDictionary, TKey, TValue> _addItem;
    private readonly Func<TDictionary> _factory;

    public IEnumerableKeyValuePairProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        uint tag,
        uint keyTag,
        uint valueTag,
        Func<TDictionary> factory,
        Action<TDictionary, TKey, TValue> addItem
    )
    {
        _keyReader = keyReader;
        _valueReader = valueReader;
        _tag = tag;
        _keyTag = keyTag;
        _valueTag = valueTag;
        _factory = factory;
        _addItem = addItem;
    }

    public TDictionary ParseFrom(ref ReaderContext input)
    {
        var dictionary = _factory();
        do
        {
            KeyValuePair<TKey, TValue> entry = ReadMapEntry(ref input);
            _addItem(dictionary, entry.Key, entry.Value);
        } while (ParsingPrimitives.MaybeConsumeTag(ref input.buffer, ref input.state, _tag));

        return dictionary;
    }

    private T Read<T>(ref ReaderContext ctx, IProtoReader<T> reader)
    {
        if (reader is IProtoMessageReader<T> messageReader)
            return messageReader.ParseMessageFrom(ref ctx);
        else
            return reader.ParseFrom(ref ctx);
    }

    public KeyValuePair<TKey, TValue> ReadMapEntry(ref ReaderContext ctx)
    {
        int length = ParsingPrimitives.ParseLength(ref ctx.buffer, ref ctx.state);
        if (ctx.state.recursionDepth >= ctx.state.recursionLimit)
        {
            throw InvalidProtocolBufferException.RecursionLimitExceeded();
        }
        int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);
        ++ctx.state.recursionDepth;

        TKey key = default!;
        TValue value = default!;
        uint tag;
        while ((tag = ctx.ReadTag()) != 0)
        {
            if (tag == _keyTag)
            {
                key = Read<TKey>(ref ctx, _keyReader);
            }
            else if (tag == _valueTag)
            {
                value = Read<TValue>(ref ctx, _valueReader);
            }
            else
            {
                ParsingPrimitivesMessages.SkipLastField(ref ctx.buffer, ref ctx.state);
            }
        }

        if (key is null)
        {
            throw new Exception($"Map entry is missing required key field (tag {_keyTag}).");
        }

        ParsingPrimitivesMessages.CheckReadEndOfStreamTag(ref ctx.state);
        // Check that we've read exactly as much data as expected.
        if (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
        {
            throw InvalidProtocolBufferException.TruncatedMessage();
        }
        --ctx.state.recursionDepth;
        SegmentedBufferHelper.PopLimit(ref ctx.state, oldLimit);

        return new KeyValuePair<TKey, TValue>(key, value);
    }
}
