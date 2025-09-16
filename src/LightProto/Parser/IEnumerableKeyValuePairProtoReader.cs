namespace LightProto.Parser;

public class IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue>
    : IEnumerableProtoReader<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public IEnumerableKeyValuePairProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        uint tag,
        uint keyTag,
        uint valueTag,
        Func<int, TDictionary> factory,
        Func<TDictionary, KeyValuePair<TKey, TValue>, TDictionary> addItem,
        uint tag2
    )
        : base(
            new KeyValuePairProtoReader<TKey, TValue>(keyReader, valueReader, keyTag, valueTag),
            tag,
            factory,
            addItem,
            itemFixedSize: 0,
            isPacked: false,
            tag2,
            completeAction: null
        ) { }
}
