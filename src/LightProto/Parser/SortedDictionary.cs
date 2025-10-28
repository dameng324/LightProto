namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class SortedDictionaryProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<SortedDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedDictionaryProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        : base(
            keyReader,
            valueReader,
            static items =>
            {
                var dic = new SortedDictionary<TKey, TValue>();
                foreach (var item in items)
                    dic.Add(item.Key, item.Value);
                return dic;
            },
            new()
        ) { }
}

public sealed class SortedDictionaryProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<SortedDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedDictionaryProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag
    )
        : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
}
