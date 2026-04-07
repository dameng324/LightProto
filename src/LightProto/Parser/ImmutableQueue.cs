using System.Collections.Immutable;

namespace LightProto.Parser
{
    public sealed class ImmutableQueueProtoWriter<T> : IProtoWriter, IProtoWriter<ImmutableQueue<T>>, ICollectionWriter
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

        public ImmutableQueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            _arrayWriter = new ArrayProtoWriter<T>(itemWriter, tag, itemFixedSize);
        }

        int IProtoWriter.CalculateSize(object value) => CalculateSize((ImmutableQueue<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((ImmutableQueue<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (ImmutableQueue<T>)value);

        public int CalculateSize(ImmutableQueue<T> collection) => _arrayWriter.CalculateSize(collection.ToArray());

        public long CalculateLongSize(ImmutableQueue<T> collection) => _arrayWriter.CalculateLongSize(collection.ToArray());

        public void WriteTo(ref WriterContext output, ImmutableQueue<T> collection) =>
            _arrayWriter.WriteTo(ref output, collection.ToArray());
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
