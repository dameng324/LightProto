namespace Dameng.Protobuf.Parser;

public class IEnumerableKeyValuePairProtoWriter<TDictionary, TKey, TValue>
    : IEnumerableProtoWriter<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public IEnumerableKeyValuePairProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag,
        uint keyTag,
        uint valueTag,
        Func<TDictionary, int> getCount
    ) : base(
        itemWriter: new KeyValuePairProtoWriter<TKey, TValue>(keyWriter, valueWriter, keyTag, valueTag),
        tag:tag, getCount,itemFixedSize:0, isPacked: false)
    {
    }
}