namespace LightProto.Parser;

public class IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue>
    : IEnumerableProtoReader<TDictionary, KeyValuePair<TKey, TValue>>
    where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    public IEnumerableKeyValuePairProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        Func<List<KeyValuePair<TKey, TValue>>, TDictionary> pairListToDictionaryFunc,
        TDictionary empty
    )
        : base(
            new KeyValuePairProtoReader<TKey, TValue>(keyReader, valueReader),
            itemFixedSize: 0,
            pairListToDictionaryFunc,
            empty
        ) { }
}
