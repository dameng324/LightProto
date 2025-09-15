namespace LightProto.Parser;

public interface ICollectionWriter;

public class IEnumerableProtoWriter<TCollection, TItem>
    : IProtoWriter<TCollection>,
        ICollectionWriter
    where TCollection : IEnumerable<TItem>
{
    public IProtoWriter<TItem> ItemWriter { get; }
    public uint Tag { get; }
    public Func<TCollection, int> GetCount { get; }
    public int ItemFixedSize { get; }
    public bool IsPacked { get; }
    public uint Tag2 { get; }

    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public IEnumerableProtoWriter(
        IProtoWriter<TItem> itemWriter,
        uint tag,
        Func<TCollection, int> getCount,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
    {
        ItemWriter = itemWriter;
        Tag = tag;
        GetCount = getCount;
        ItemFixedSize = itemFixedSize;
        IsPacked = isPacked;
        Tag2 = tag2;
    }

    private int CalculatePackedDataSize(TCollection collection, int count)
    {
        return ItemFixedSize != 0 ? ItemFixedSize * count : GetAllItemSize(collection);
    }

    int GetAllItemSize(TCollection collection)
    {
        int size = 0;

        if (collection is IList<TItem> list)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var item = list[index];
                if (item is null)
                {
                    throw new Exception("Sequence contained null element");
                }

                size += ItemWriter.CalculateMessageSize(item);
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

                size += ItemWriter.CalculateMessageSize(item);
            }
        }
        return size;
    }

    public int CalculateSize(TCollection value)
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
            return CodedOutputStream.ComputeRawVarint32Size(Tag)
                + CodedOutputStream.ComputeLengthSize(dataSize)
                + dataSize;
        }
        else
        {
            return CodedOutputStream.ComputeRawVarint32Size(Tag) * count + GetAllItemSize(value);
        }
    }

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
            int size = CalculatePackedDataSize(collection, count);
            output.WriteTag(Tag);
            output.WriteLength(size);

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
                    var current = item;
                    if (current is null)
                    {
                        throw new Exception("Sequence contained null element");
                    }

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

                    output.WriteTag(Tag2);
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

                    output.WriteTag(Tag2);
                    ItemWriter.WriteMessageTo(ref output, item);
                }
            }
        }
    }
}
