namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class SortedListProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<SortedList<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedListProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        uint tag,
        uint keyTag,
        uint valueTag,
        uint tag2
    )
        : base(
            keyReader,
            valueReader,
            tag,
            keyTag,
            valueTag,
            static (_) => new(),
            static (dic, pair) =>
            {
                dic[pair.Key] = pair.Value;
                return dic;
            },
            tag2
        ) { }
}

public sealed class SortedListProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<SortedList<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedListProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag,
        uint keyTag,
        uint valueTag,
        uint tag2
    )
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count, tag2) { }
}
