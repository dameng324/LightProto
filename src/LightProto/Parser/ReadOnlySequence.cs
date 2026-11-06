using System.Buffers;
using System.Runtime.CompilerServices;

namespace LightProto.Parser
{
    public sealed class ReadOnlySequenceProtoWriter<T> : IProtoWriter, IProtoWriter<ReadOnlySequence<T>>, ICollectionWriter
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

        public ReadOnlySequenceProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            ItemWriter = itemWriter;
            Tag = tag;
            ItemFixedSize = itemFixedSize;
        }

        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((ReadOnlySequence<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((ReadOnlySequence<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (ReadOnlySequence<T>)value);

        public int CalculateSize(ReadOnlySequence<T> value)
        {
            var size = CalculateLongSize(value);
            if (size > int.MaxValue)
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            return (int)size;
        }

        public long CalculateLongSize(ReadOnlySequence<T> value)
        {
            if (IsByte)
            {
                var length = value.Length;
                if (length == 0)
                    return 0;
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(length) + length;
            }

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

        public void WriteTo(ref WriterContext output, ReadOnlySequence<T> value)
        {
            if (value.IsEmpty)
                return;

            if (IsByte)
            {
                output.WriteTag(Tag);
                output.WriteLongLength(value.Length);
                foreach (var segment in value)
                {
                    if (segment.IsEmpty)
                        continue;
                    var segmentBytes = segment;
                    var bytes = Unsafe.As<ReadOnlyMemory<T>, ReadOnlyMemory<byte>>(ref segmentBytes);
                    WritingPrimitives.WriteRawBytes(ref output.buffer, ref output.state, bytes.Span);
                }
                return;
            }

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var size = CalculatePackedDataSize(value, value.Length);
                output.WriteTag(Tag);
                output.WriteLongLength(size);

                foreach (var item in value)
                {
                    CollectionWriteHelper<T>.WritePacked(ref output, item.Span, ItemWriter);
                }
                return;
            }

            foreach (var item in value)
            {
                CollectionWriteHelper<T>.WriteUnpacked(ref output, item.Span, Tag, ItemWriter);
            }
        }

        private long CalculatePackedDataSize(ReadOnlySequence<T> value, long count)
        {
            return ItemFixedSize != 0 ? ItemFixedSize * count : CalculateAllItemSize(value);
        }

        private long CalculateAllItemSize(ReadOnlySequence<T> value)
        {
            long size = 0;
            foreach (var item in value)
            {
                size += CollectionWriteHelper<T>.CalculateAllItemSize(item.Span, ItemWriter);
            }
            return size;
        }
    }

    public sealed class ReadOnlySequenceProtoReader<TItem> : ICollectionReader<ReadOnlySequence<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;
        static bool IsByte => typeof(TItem) == typeof(byte);

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => IsByte ? WireFormat.WireType.LengthDelimited : ItemReader.WireType;
        object ICollectionReader.Empty => Empty;
        public IProtoReader<TItem> ItemReader { get; }

        public ReadOnlySequenceProtoReader(IProtoReader<TItem> itemReader, int itemFixedSize)
        {
            _arrayProtoReader = new ArrayProtoReader<TItem>(itemReader, itemFixedSize);
            ItemReader = itemReader;
        }

        public ReadOnlySequenceProtoReader(IProtoReader<TItem> itemReader, uint tag, int itemFixedSize)
            : this(itemReader, itemFixedSize) { }

        public ReadOnlySequence<TItem> ParseFrom(ref ReaderContext input)
        {
            if (IsByte)
            {
                var length = input.ReadLength();
                if (length == 0)
                    return default;
                var bytes = ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length);
                var sequence = new ReadOnlySequence<byte>(bytes);
                return Unsafe.As<ReadOnlySequence<byte>, ReadOnlySequence<TItem>>(ref sequence);
            }

            var array = _arrayProtoReader.ParseFrom(ref input);
            return array.Length == 0 ? default : new ReadOnlySequence<TItem>(array);
        }

        public ReadOnlySequence<TItem> Empty => default;
    }
}
