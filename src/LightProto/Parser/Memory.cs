using System.Runtime.CompilerServices;

namespace LightProto.Parser
{
    public sealed class MemoryProtoWriter<T> : IProtoWriter, IProtoWriter<Memory<T>>, ICollectionWriter
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

            if (IsByte)
            {
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(count) + count;
            }

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var dataSize = CalculatePackedDataSize(value, count);
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(dataSize) + dataSize;
            }

            return CodedOutputStream.ComputeRawVarint32Size(Tag) * count + CalculateAllItemSize(value.Span);
        }

        public void WriteTo(ref WriterContext output, Memory<T> value)
        {
            if (value.IsEmpty)
                return;

            if (IsByte)
            {
                var bytes = Unsafe.As<Memory<T>, Memory<byte>>(ref value);
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

                CollectionWriteHelper<T>.WritePacked(ref output, value.Span, ItemWriter);
                return;
            }

            CollectionWriteHelper<T>.WriteUnpacked(ref output, value.Span, Tag, ItemWriter);
        }

        private long CalculatePackedDataSize(Memory<T> value, int count)
        {
            return ItemFixedSize != 0 ? ItemFixedSize * count : CalculateAllItemSize(value.Span);
        }

        private long CalculateAllItemSize(ReadOnlySpan<T> values)
        {
            return CollectionWriteHelper<T>.CalculateAllItemSize(values, ItemWriter);
        }
    }

    public sealed class MemoryProtoReader<TItem> : ICollectionReader<Memory<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;
        static bool IsByte => typeof(TItem) == typeof(byte);

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => IsByte ? WireFormat.WireType.LengthDelimited : ItemReader.WireType;
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
            if (IsByte)
            {
                var length = input.ReadLength();
                var bytes = ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length);
                var result = new Memory<byte>(bytes);
                return Unsafe.As<Memory<byte>, Memory<TItem>>(ref result);
            }
            return _arrayProtoReader.ParseFrom(ref input);
        }

        public Memory<TItem> Empty => Memory<TItem>.Empty;
    }
}
