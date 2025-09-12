namespace Dameng.Protobuf.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class SortedDictionaryProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<SortedDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedDictionaryProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader,
        uint tag,
        uint keyTag,
        uint valueTag
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
            }
        ) { }
}

public sealed class SortedDictionaryProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<SortedDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public SortedDictionaryProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag,
        uint keyTag,
        uint valueTag
    )
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count) { }
}
