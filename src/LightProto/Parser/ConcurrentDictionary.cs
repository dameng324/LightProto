using System.Collections.Concurrent;

namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class ConcurrentDictionaryProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<ConcurrentDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public ConcurrentDictionaryProtoReader(
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

public sealed class ConcurrentDictionaryProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<ConcurrentDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public ConcurrentDictionaryProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag,
        uint keyTag,
        uint valueTag,
        uint tag2
    )
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count, tag2) { }
}
