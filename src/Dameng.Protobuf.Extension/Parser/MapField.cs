using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Dameng.Protobuf.Extension;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public class MapFieldProtoReader<TKey, TValue> : IEnumerableKeyValuePairProtoReader<MapField<TKey, TValue>,TKey, TValue>
{
    public MapFieldProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader, uint tag, uint keyTag, uint valueTag) : 
        base(keyReader, valueReader, tag, keyTag, valueTag, static ()=>new(), static (dic,key,value)=>dic[key]=value) { }
}
public class MapFieldProtoWriter<TKey, TValue> : IEnumerableKeyValuePairProtoWriter<MapField<TKey, TValue>,TKey,TValue>
{
    public MapFieldProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter, uint tag, uint keyTag,
        uint valueTag)
        : base(keyWriter, valueWriter, tag, keyTag, valueTag, (dic) => dic.Count) { }
}