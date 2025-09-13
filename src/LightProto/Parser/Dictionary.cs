namespace LightProto.Parser;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public sealed class DictionaryProtoReader<TKey, TValue> : IEnumerableKeyValuePairProtoReader<Dictionary<TKey, TValue>,TKey, TValue>where TKey:notnull
{
    public DictionaryProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader, uint tag, uint keyTag, uint valueTag) : 
        base(keyReader, valueReader, tag, keyTag, valueTag, static capacity=>new(capacity), static (dic,pair)=>
        {
             dic[pair.Key] = pair.Value;
             return dic;
        }) { }
}
public sealed class DictionaryProtoWriter<TKey, TValue> : IEnumerableKeyValuePairProtoWriter<Dictionary<TKey, TValue>,TKey,TValue>where TKey:notnull
{
    public DictionaryProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter, uint tag, uint keyTag,
        uint valueTag)
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count) { }
}