using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableHashSetProtoWriter<T>
        : IEnumerableProtoWriter<ImmutableHashSet<T>, T>
    {
        public ImmutableHashSetProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
    }

    public sealed class ImmutableHashSetProtoReader<TItem>
        : ICollectionReader<ImmutableHashSet<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public ImmutableHashSet<TItem> ParseFrom(ref ReaderContext input)
        {
            return ImmutableHashSet.Create(_arrayReader.ParseFrom(ref input));
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ImmutableHashSetProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ImmutableHashSetProtoReader(
            IProtoReader<TItem> itemReader,
            uint tag,
            int itemFixedSize
        )
            : this(itemReader, itemFixedSize) { }

        public ImmutableHashSet<TItem> Empty => ImmutableHashSet<TItem>.Empty;
    }
}
