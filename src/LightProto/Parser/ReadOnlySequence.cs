using System.Buffers;

namespace LightProto.Parser
{
    public sealed class ReadOnlySequenceProtoWriter<T> : IProtoWriter, IProtoWriter<ReadOnlySequence<T>>
    {
        IProtoWriter<T> ItemWriter { get; }
        uint Tag { get; }
        int ItemFixedSize { get; }
        bool IsPacked => WireFormat.GetTagWireType(Tag) == WireFormat.WireType.LengthDelimited;

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

            if (IsPacked && PackedRepeated.Support<T>())
            {
                var size = CalculatePackedDataSize(value, value.Length);
                output.WriteTag(Tag);
                output.WriteLongLength(size);

                foreach (var item in value)
                {
                    foreach (var current in item.Span)
                    {
                        ItemWriter.WriteMessageTo(ref output, current);
                    }
                }
                return;
            }

            foreach (var item in value)
            {
                foreach (var current in item.Span)
                {
                    if (current is null)
                        throw new Exception("Sequence contained null element");
                    output.WriteTag(Tag);
                    ItemWriter.WriteMessageTo(ref output, current);
                }
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
                foreach (var current in item.Span)
                {
                    if (current is null)
                        throw new Exception("Sequence contained null element");
                    size += ItemWriter.CalculateLongMessageSize(current);
                }
            }
            return size;
        }
    }

    public sealed class ReadOnlySequenceProtoReader<TItem> : ICollectionReader<ReadOnlySequence<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
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
            var array = _arrayProtoReader.ParseFrom(ref input);
            return array.Length == 0 ? default : new ReadOnlySequence<TItem>(array);
        }

        public ReadOnlySequence<TItem> Empty => default;
    }
}
