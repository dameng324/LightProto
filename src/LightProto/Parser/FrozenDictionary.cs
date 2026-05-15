#if NET8_0_OR_GREATER
using System.Collections.Frozen;

namespace LightProto.Parser
{
    public sealed class FrozenDictionaryProtoWriter<TKey, TValue>
        : IEnumerableKeyValuePairProtoWriter<FrozenDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public FrozenDictionaryProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter, uint tag)
            : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
    }

    public sealed class FrozenDictionaryProtoReader<TKey, TValue>
        : ICollectionReader<FrozenDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        private readonly DictionaryProtoReader<TKey, TValue> _dictionaryReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => _dictionaryReader.ParseFrom(ref input).ToFrozenDictionary();

        FrozenDictionary<TKey, TValue> IProtoReader<FrozenDictionary<TKey, TValue>>.ParseFrom(ref ReaderContext input) =>
            _dictionaryReader.ParseFrom(ref input).ToFrozenDictionary();

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;

        public FrozenDictionaryProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader, uint tag)
        {
            _dictionaryReader = new DictionaryProtoReader<TKey, TValue>(keyReader, valueReader, tag);
        }

        public FrozenDictionary<TKey, TValue> Empty => FrozenDictionary<TKey, TValue>.Empty;
        public IProtoReader<KeyValuePair<TKey, TValue>> ItemReader => _dictionaryReader.ItemReader;
    }
}
#endif
