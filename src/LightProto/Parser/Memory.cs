namespace LightProto.Parser
{
    public sealed class MemoryProtoWriter<T> : IProtoWriter, IProtoWriter<Memory<T>>
    {
        IProtoWriter<T> ItemWriter { get; }
        uint Tag { get; }
        int ItemFixedSize { get; }
        bool IsPacked => WireFormat.GetTagWireType(Tag) == WireFormat.WireType.LengthDelimited;

        public MemoryProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            ItemWriter = itemWriter;
            Tag = tag;
            ItemFixedSize = itemFixedSize;
        }

        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((Memory<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((Memory<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Memory<T>)value);

        public int CalculateSize(Memory<T> value)
        {
            var size = CalculateLongSize(value);
            if (size > int.MaxValue)
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            return (int)size;
        }

        public long CalculateLongSize(Memory<T> value)
        {
            var count = value.Length;
            if (count == 0)
                return 0;

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var dataSize = CalculatePackedDataSize(value, count);
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(dataSize) + dataSize;
            }

            return CodedOutputStream.ComputeRawVarint32Size(Tag) * count + CalculateAllItemSize(value);
        }

        public void WriteTo(ref WriterContext output, Memory<T> value)
        {
            if (value.IsEmpty)
                return;

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var size = CalculatePackedDataSize(value, value.Length);
                output.WriteTag(Tag);
                output.WriteLongLength(size);

                foreach (var item in value.Span)
                {
                    ItemWriter.WriteMessageTo(ref output, item);
                }
                return;
            }

            foreach (var item in value.Span)
            {
                if (item is null)
                    throw new Exception("Sequence contained null element");
                output.WriteTag(Tag);
                ItemWriter.WriteMessageTo(ref output, item);
            }
        }

        private long CalculatePackedDataSize(Memory<T> value, int count)
        {
            return ItemFixedSize != 0 ? ItemFixedSize * count : CalculateAllItemSize(value);
        }

        private long CalculateAllItemSize(Memory<T> value)
        {
            long size = 0;
            foreach (var item in value.Span)
            {
                if (item is null)
                    throw new Exception("Sequence contained null element");
                size += ItemWriter.CalculateLongMessageSize(item);
            }
            return size;
        }
    }

    public sealed class MemoryProtoReader<TItem> : ICollectionReader<Memory<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public MemoryProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayProtoReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public MemoryProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public Memory<TItem> ParseFrom(ref ReaderContext input)
        {
            return _arrayProtoReader.ParseFrom(ref input);
        }

        public Memory<TItem> Empty => Memory<TItem>.Empty;
    }
}
