using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableStackProtoWriter<T> : IEnumerableProtoWriter<ImmutableStack<T>, T>
    {
        public ImmutableStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count(), itemFixedSize) { }
    }

    public sealed class ImmutableStackProtoReader<TItem> : ICollectionReader<ImmutableStack<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public ImmutableStack<TItem> ParseFrom(ref ReaderContext input)
        {
            var items = _arrayReader.ParseFrom(ref input);
            System.Array.Reverse(items);
            return ImmutableStack.Create(items);
        }

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ImmutableStackProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ImmutableStackProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public ImmutableStack<TItem> Empty => ImmutableStack<TItem>.Empty;
    }
}
