namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class SortedListProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<SortedList<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedListProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        : base(
            keyReader,
            valueReader,
            static items =>
            {
                var dic = new SortedList<TKey, TValue>(items.Count);
                foreach (var item in items)
                    dic.Add(item.Key, item.Value);
                return dic;
            },
            new()
        ) { }
}

public sealed class SortedListProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<SortedList<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedListProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag
    )
        : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
}
