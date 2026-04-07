using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableStackProtoWriter<T> : IProtoWriter, IProtoWriter<ImmutableStack<T>>, ICollectionWriter
    {
        private readonly ArrayProtoWriter<T> _arrayWriter;

        public WireFormat.WireType WireType => _arrayWriter.WireType;
        public bool IsMessage => _arrayWriter.IsMessage;

        uint ICollectionWriter.Tag
        {
            get => _arrayWriter.Tag;
            set => _arrayWriter.Tag = value;
        }

        WireFormat.WireType ICollectionWriter.ItemWireType => ((ICollectionWriter)_arrayWriter).ItemWireType;

        public ImmutableStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            _arrayWriter = new ArrayProtoWriter<T>(itemWriter, tag, itemFixedSize);
        }

        int IProtoWriter.CalculateSize(object value) => CalculateSize((ImmutableStack<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((ImmutableStack<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (ImmutableStack<T>)value);

        public int CalculateSize(ImmutableStack<T> collection) => _arrayWriter.CalculateSize(collection.ToArray());

        public long CalculateLongSize(ImmutableStack<T> collection) => _arrayWriter.CalculateLongSize(collection.ToArray());

        public void WriteTo(ref WriterContext output, ImmutableStack<T> collection) =>
            _arrayWriter.WriteTo(ref output, collection.ToArray());
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
