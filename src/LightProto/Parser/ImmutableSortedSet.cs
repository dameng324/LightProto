using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableSortedSetProtoWriter<T> : IEnumerableProtoWriter<ImmutableSortedSet<T>, T>
    {
        public ImmutableSortedSetProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
    }

    public sealed class ImmutableSortedSetProtoReader<TItem> : ICollectionReader<ImmutableSortedSet<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public ImmutableSortedSet<TItem> ParseFrom(ref ReaderContext input)
        {
            return ImmutableSortedSet.Create(_arrayReader.ParseFrom(ref input));
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ImmutableSortedSetProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ImmutableSortedSetProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public ImmutableSortedSet<TItem> Empty => ImmutableSortedSet<TItem>.Empty;
    }
}
