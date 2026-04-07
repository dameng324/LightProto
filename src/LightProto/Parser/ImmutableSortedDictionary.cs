using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableSortedDictionaryProtoWriter<TKey, TValue>
        : IEnumerableKeyValuePairProtoWriter<ImmutableSortedDictionary<TKey, TValue>, TKey, TValue>
        where TKey : notnull
    {
        public ImmutableSortedDictionaryProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter, uint tag)
            : base(keyWriter, valueWriter, tag, (dic) => dic.Count) { }
    }

    public sealed class ImmutableSortedDictionaryProtoReader<TKey, TValue>
        : ICollectionReader<ImmutableSortedDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        private readonly DictionaryProtoReader<TKey, TValue> _dictionaryReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => _dictionaryReader.ParseFrom(ref input).ToImmutableSortedDictionary();

        ImmutableSortedDictionary<TKey, TValue> IProtoReader<ImmutableSortedDictionary<TKey, TValue>>.ParseFrom(ref ReaderContext input) =>
            _dictionaryReader.ParseFrom(ref input).ToImmutableSortedDictionary();

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;

        public ImmutableSortedDictionaryProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader, uint tag)
        {
            _dictionaryReader = new DictionaryProtoReader<TKey, TValue>(keyReader, valueReader, tag);
        }

        public ImmutableSortedDictionary<TKey, TValue> Empty => ImmutableSortedDictionary<TKey, TValue>.Empty;
        public IProtoReader<KeyValuePair<TKey, TValue>> ItemReader => _dictionaryReader.ItemReader;
    }
}
