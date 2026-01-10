using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableArrayProtoWriter<T> : IEnumerableProtoWriter<ImmutableArray<T>, T>
    {
        public ImmutableArrayProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Length, itemFixedSize) { }
    }

    public sealed class ImmutableArrayProtoReader<TItem>
        : ICollectionReader<ImmutableArray<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public ImmutableArray<TItem> ParseFrom(ref ReaderContext input)
        {
            return ImmutableArray.Create(_arrayReader.ParseFrom(ref input));
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;

        public IProtoReader<TItem> ItemReader { get; }

        public ImmutableArrayProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ImmutableArrayProtoReader(
            IProtoReader<TItem> itemReader,
            uint tag,
            int itemFixedSize
        )
            : this(itemReader, itemFixedSize) { }

        public ImmutableArray<TItem> Empty => ImmutableArray<TItem>.Empty;
    }
}
