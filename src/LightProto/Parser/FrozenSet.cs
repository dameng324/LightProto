#if NET8_0_OR_GREATER
using System.Collections.Frozen;

namespace LightProto.Parser
{
    public sealed class FrozenSetProtoWriter<T> : IEnumerableProtoWriter<FrozenSet<T>, T>
    {
        public FrozenSetProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
    }

    public sealed class FrozenSetProtoReader<TItem> : ICollectionReader<FrozenSet<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public FrozenSet<TItem> ParseFrom(ref ReaderContext input)
        {
            return _arrayReader.ParseFrom(ref input).ToFrozenSet();
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public FrozenSetProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public FrozenSetProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public FrozenSet<TItem> Empty => FrozenSet<TItem>.Empty;
    }
}
#endif
