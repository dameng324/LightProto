using System.Collections.Concurrent;

namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class ConcurrentDictionaryProtoReader<TKey, TValue>
    : IEnumerableKeyValuePairProtoReader<ConcurrentDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public ConcurrentDictionaryProtoReader(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        : base(keyReader, valueReader, static items => new(items), new()) { }
}

public sealed class ConcurrentDictionaryProtoWriter<TKey, TValue>
    : IEnumerableKeyValuePairProtoWriter<ConcurrentDictionary<TKey, TValue>, TKey, TValue>
    where TKey : notnull
{
    public ConcurrentDictionaryProtoWriter(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter,
        uint tag
    )
        : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
}
