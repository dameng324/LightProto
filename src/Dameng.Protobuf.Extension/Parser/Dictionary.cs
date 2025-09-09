using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public class DictionaryProtoReader<TKey, TValue> : IEnumerableKeyValuePairProtoReader<Dictionary<TKey, TValue>,TKey, TValue>
{
    public DictionaryProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader, uint tag, uint keyTag, uint valueTag) : 
        base(keyReader, valueReader, tag, keyTag, valueTag, static ()=>new(), static (dic,key,value)=>dic[key]=value) { }
}
public class DictionaryProtoWriter<TKey, TValue> : IEnumerableKeyValuePairProtoWriter<Dictionary<TKey, TValue>,TKey,TValue>
{
    public DictionaryProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter, uint tag, uint keyTag,
        uint valueTag)
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count) { }
}