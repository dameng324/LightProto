using System.Collections;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class IEnumerableKeyValuePairProtoWriter<TDictionary, TKey, TValue>
    : IProtoWriter<TDictionary>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
{
    private readonly IProtoWriter<TKey> _keyWriter;
    private readonly IProtoWriter<TValue> _valueWriter;
    private readonly uint _tag;
    private readonly uint _keyTag;
    private readonly uint _valueTag;
    private readonly Func<TDictionary, int> _getCount;

    public IEnumerableKeyValuePairProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag,
        uint keyTag,
        uint valueTag,
        Func<TDictionary, int> getCount
    )
    {
        _keyWriter = keyWriter;
        _valueWriter = valueWriter;
        _tag = tag;
        _keyTag = keyTag;
        _valueTag = valueTag;
        _getCount = getCount;
    }

    public int CalculateSize(KeyValuePair<TKey, TValue> value)
    {
        var size = 0;
        size += CodedOutputStream.ComputeRawVarint32Size(_keyTag);
        size += _keyWriter is IProtoMessageWriter<TKey> keyMessageWriter
            ? keyMessageWriter.CalculateMessageSize(value.Key)
            : _keyWriter.CalculateSize(value.Key);
        size += CodedOutputStream.ComputeRawVarint32Size(_valueTag);
        size += _valueWriter is IProtoMessageWriter<TValue> valueMessageWriter
            ? valueMessageWriter.CalculateMessageSize(value.Value)
            : _valueWriter.CalculateSize(value.Value);
        return size;
    }

    public int CalculateSize(TDictionary value)
    {
        if (_getCount(value) == 0)
        {
            return 0;
        }

        var size = 0;
        foreach (var pair in value)
        {
            size += CodedOutputStream.ComputeRawVarint32Size(_tag);

            var entrySize = CalculateSize(pair);
            size += CodedOutputStream.ComputeLengthSize(entrySize);
            size += entrySize;
        }

        return size;
    }

    public void WriteTo(ref WriteContext output, TDictionary value)
    {
        foreach (var pair in value)
        {
            output.WriteTag(_tag);
            var size = CalculateSize(pair);
            output.WriteLength(size);

            output.WriteTag(_keyTag);
            if (_keyWriter is IProtoMessageWriter<TKey> messageWriter)
            {
                messageWriter.WriteMessageTo(ref output, pair.Key);
            }
            else
            {
                _keyWriter.WriteTo(ref output, pair.Key);
            }

            output.WriteTag(_valueTag);
            if (_valueWriter is IProtoMessageWriter<TValue> valueMessageWriter)
            {
                valueMessageWriter.WriteMessageTo(ref output, pair.Value);
            }
            else
            {
                _valueWriter.WriteTo(ref output, pair.Value);
            }
        }
    }
}
