using System.Runtime.InteropServices;

namespace LightProto.Parser
{
    public sealed class ReadOnlyMemoryProtoWriter<T> : IProtoWriter, IProtoWriter<ReadOnlyMemory<T>>
    {
        private readonly ArrayProtoWriter<T> _arrayProtoWriter;

        public ReadOnlyMemoryProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        {
            _arrayProtoWriter = new ArrayProtoWriter<T>(itemWriter, tag, itemFixedSize);
        }

        public WireFormat.WireType WireType => _arrayProtoWriter.WireType;
        public bool IsMessage => false;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((ReadOnlyMemory<T>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((ReadOnlyMemory<T>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (ReadOnlyMemory<T>)value);

        public int CalculateSize(ReadOnlyMemory<T> value)
        {
            return _arrayProtoWriter.CalculateSize(ToArray(value));
        }

        public long CalculateLongSize(ReadOnlyMemory<T> value)
        {
            return _arrayProtoWriter.CalculateLongSize(ToArray(value));
        }

        public void WriteTo(ref WriterContext output, ReadOnlyMemory<T> value)
        {
            _arrayProtoWriter.WriteTo(ref output, ToArray(value));
        }

        private static T[] ToArray(ReadOnlyMemory<T> value)
        {
            if (value.IsEmpty)
                return [];
            if (MemoryMarshal.TryGetArray(value, out ArraySegment<T> segment))
            {
                if (segment.Array is null)
                    return [];
                if (segment.Offset == 0 && segment.Count == segment.Array.Length)
                    return segment.Array;
            }
            return value.ToArray();
        }
    }

    public sealed class ReadOnlyMemoryProtoReader<TItem> : ICollectionReader<ReadOnlyMemory<TItem>, TItem>
    {
        private readonly ArrayProtoReader<TItem> _arrayProtoReader;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType ItemWireType => ItemReader.WireType;
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
            return _arrayProtoReader.ParseFrom(ref input);
        }

        public ReadOnlyMemory<TItem> Empty => ReadOnlyMemory<TItem>.Empty;
    }
}
