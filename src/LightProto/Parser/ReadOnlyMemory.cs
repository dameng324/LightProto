namespace LightProto.Parser
{
    public sealed class ReadOnlyMemoryProtoWriter<T> : IProtoWriter, IProtoWriter<ReadOnlyMemory<T>>, ICollectionWriter
    {
        IProtoWriter<T> ItemWriter { get; }
        uint Tag { get; set; }
        int ItemFixedSize { get; }
        static bool IsByte => typeof(T) == typeof(byte);
        bool IsPacked => WireFormat.GetTagWireType(Tag) == WireFormat.WireType.LengthDelimited;
        WireFormat.WireType ICollectionWriter.ItemWireType => IsByte ? WireFormat.WireType.LengthDelimited : ItemWriter.WireType;

        uint ICollectionWriter.Tag
        {
            get => Tag;
            set => Tag = value;
        }

        public ReadOnlyMemoryProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            ItemWriter = itemWriter;
            Tag = tag;
            ItemFixedSize = itemFixedSize;
        }

        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((ReadOnlyMemory<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((ReadOnlyMemory<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (ReadOnlyMemory<T>)value);

        public int CalculateSize(ReadOnlyMemory<T> value)
        {
            var size = CalculateLongSize(value);
            if (size > int.MaxValue)
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            return (int)size;
        }

        public long CalculateLongSize(ReadOnlyMemory<T> value)
        {
            var count = value.Length;
            if (count == 0)
                return 0;

            if (IsByte)
            {
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(count) + count;
            }

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var dataSize = CalculatePackedDataSize(value, count);
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(dataSize) + dataSize;
            }

            return CodedOutputStream.ComputeRawVarint32Size(Tag) * count + CalculateAllItemSize(value);
        }

        public void WriteTo(ref WriterContext output, ReadOnlyMemory<T> value)
        {
            if (value.IsEmpty)
                return;

            if (IsByte)
            {
                var bytes = (ReadOnlyMemory<byte>)(object)value;
                output.WriteTag(Tag);
                output.WriteLongLength(bytes.Length);
                WritingPrimitives.WriteRawBytes(ref output.buffer, ref output.state, bytes.Span);
                return;
            }

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

        private long CalculatePackedDataSize(ReadOnlyMemory<T> value, int count)
        {
            return ItemFixedSize != 0 ? ItemFixedSize * count : CalculateAllItemSize(value);
        }

        private long CalculateAllItemSize(ReadOnlyMemory<T> value)
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

    public sealed class ReadOnlyMemoryProtoReader<TItem> : ICollectionReader<ReadOnlyMemory<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;
        static bool IsByte => typeof(TItem) == typeof(byte);

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => IsByte ? WireFormat.WireType.LengthDelimited : ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ReadOnlyMemoryProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayProtoReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ReadOnlyMemoryProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public ReadOnlyMemory<TItem> ParseFrom(ref ReaderContext input)
        {
            if (IsByte)
            {
                var length = input.ReadLength();
                var bytes = ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length);
                return (ReadOnlyMemory<TItem>)(object)new ReadOnlyMemory<byte>(bytes);
            }
            return _arrayProtoReader.ParseFrom(ref input);
        }

        public ReadOnlyMemory<TItem> Empty => ReadOnlyMemory<TItem>.Empty;
    }
}
