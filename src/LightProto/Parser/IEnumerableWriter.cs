namespace LightProto.Parser;

public interface ICollectionWriter { }

public class IEnumerableProtoWriter<TCollection, TItem>
    : IProtoWriter<TCollection>,
        ICollectionWriter
    where TCollection : IEnumerable<TItem>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => false;
    public IProtoWriter<TItem> ItemWriter { get; }
    public uint Tag { get; set; }
    public Func<TCollection, int> GetCount { get; }
    public int ItemFixedSize { get; }

    public IEnumerableProtoWriter(
        IProtoWriter<TItem> itemWriter,
        uint tag,
        Func<TCollection, int> getCount,
        int itemFixedSize
    )
    {
        ItemWriter = itemWriter;
        Tag = tag;
        GetCount = getCount;
        ItemFixedSize = itemFixedSize;
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
            //int size = CalculatePackedDataSize(collection, count);
            output.WriteTag(Tag);
            var lengthSpan = output.GetLengthSpan();

            var oldWritten = output.WrittenCount;
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
            var length = output.WrittenCount - oldWritten;
            output.WriteLength(lengthSpan, length);
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
