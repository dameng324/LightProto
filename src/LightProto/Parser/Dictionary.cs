namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class DictionaryProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<Dictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public DictionaryProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        : base(keyReader, valueReader,
#if NET7_0_OR_GREATER
            static items => new(items),
#else
            static items =>
            {
                var dic = new Dictionary<TKey, TValue>(items.Count);
                foreach (var item in items)
                {
                    dic.Add(item.Key, item.Value);
                }
                return dic;
            },
#endif
            new()) { }
}

public sealed class DictionaryProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<Dictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public DictionaryProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag
    )
        : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
}
