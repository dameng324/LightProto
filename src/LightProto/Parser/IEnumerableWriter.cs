namespace LightProto.Parser
{
    public interface ICollectionWriter
    {
        internal uint Tag { get; set; }
        internal WireFormat.WireType ItemWireType { get; }
    }

    public class IEnumerableProtoWriter<TCollection, TItem> : IProtoWriter, IProtoWriter<TCollection>, ICollectionWriter
        where TCollection : IEnumerable<TItem>
    {
        int IProtoWriter.CalculateSize(object value) => CalculateSize((TCollection)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (TCollection)value);

        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        WireFormat.WireType ICollectionWriter.ItemWireType => ItemWriter.WireType;
        public IProtoWriter<TItem> ItemWriter { get; }
        public uint Tag { get; set; }
        public Func<TCollection, int> GetCount { get; }
        int ItemFixedSize { get; }

        public IEnumerableProtoWriter(IProtoWriter<TItem> itemWriter, uint tag, Func<TCollection, int> getCount, int itemFixedSize)
        {
            ItemWriter = itemWriter;
            Tag = tag;
            GetCount = getCount;
            ItemFixedSize = itemFixedSize;
        }

        private long CalculatePackedDataSize(TCollection collection, int count)
        {
            return ItemFixedSize != 0 ? ItemFixedSize * count : GetAllItemSize(collection);
        }

        long GetAllItemSize(TCollection collection)
        {
            long size = 0;

            if (collection is IList<TItem> list)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var item = list[index];
                    if (item is null)
                    {
                        throw new Exception("Sequence contained null element");
                    }

                    size += ItemWriter.CalculateLongMessageSize(item);
                }
            }
            else
            {
                foreach (var item in collection)
                {
                    if (item is null)
                    {
                        throw new Exception("Sequence contained null element");
                    }

                    size += ItemWriter.CalculateLongMessageSize(item);
                }
            }
            return size;
        }

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((TCollection)value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CalculateSize(TCollection value)
        {
            var longSize = CalculateLongSize(value);
            if (longSize > int.MaxValue)
            {
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            }
            return (int)longSize;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long CalculateLongSize(TCollection value)
        {
            if (value is null)
                return 0;
            var count = GetCount(value);
            if (count == 0)
            {
                return 0;
            }

            if (IsPacked && PackedRepeated.Support<TItem>())
            {
                var dataSize = CalculatePackedDataSize(value, count);
                return CodedOutputStream.ComputeRawVarint32Size(Tag) + CodedOutputStream.ComputeLongLengthSize(dataSize) + dataSize;
            }
            else
            {
                return CodedOutputStream.ComputeRawVarint32Size(Tag) * count + GetAllItemSize(value);
            }
        }

        public bool IsPacked => WireFormat.GetTagWireType(Tag) == WireFormat.WireType.LengthDelimited;

        public void WriteTo(ref WriterContext output, TCollection collection)
        {
            if (collection == null)
            {
                return;
            }

            var count = GetCount(collection);
            if (count == 0)
            {
                return;
            }

            if (IsPacked && PackedRepeated.Support<TItem>())
            {
                // Packed primitive type
                long size = CalculatePackedDataSize(collection, count);
                output.WriteTag(Tag);
                output.WriteLongLength(size);

                // if littleEndian and elements has fixed size, treat array as bytes (and write it as bytes to buffer) for improved performance
                // if(TryGetArrayAsSpanPinnedUnsafe(codec, out Span<byte> span, out GCHandle handle))
                // {
                //     span = span.Slice(0, Count * codec.FixedSize);
                //
                //     WritingPrimitives.WriteRawBytes(ref ctx.buffer, ref ctx.state, span);
                //     handle.Free();
                // }
                // else
                {
                    foreach (var item in collection)
                    {
                        ItemWriter.WriteMessageTo(ref output, item);
                    }
                }
            }
            else
            {
                if (collection is IList<TItem> list)
                {
                    for (var index = 0; index < list.Count; index++)
                    {
                        var item = list[index];
                        if (item is null)
                        {
                            throw new Exception("Sequence contained null element");
                        }

                        output.WriteTag(Tag);
                        ItemWriter.WriteMessageTo(ref output, item);
                    }
                }
                else
                {
                    foreach (var item in collection)
                    {
                        var current = item;
                        if (current is null)
                        {
                            throw new Exception("Sequence contained null element");
                        }

                        output.WriteTag(Tag);
                        ItemWriter.WriteMessageTo(ref output, item);
                    }
                }
            }
        }
    }
}
