using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableQueueProtoWriter<T> : IEnumerableProtoWriter<ImmutableQueue<T>, T>
    {
        public ImmutableQueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count(), itemFixedSize) { }
    }

    public sealed class ImmutableQueueProtoReader<TItem> : ICollectionReader<ImmutableQueue<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public ImmutableQueue<TItem> ParseFrom(ref ReaderContext input)
        {
            return ImmutableQueue.Create(_arrayReader.ParseFrom(ref input));
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ImmutableQueueProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ImmutableQueueProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public ImmutableQueue<TItem> Empty => ImmutableQueue<TItem>.Empty;
    }
}
