



namespace Dameng.Protobuf.Parser;

public sealed class ArrayProtoWriter<T> : IEnumerableProtoWriter<T[],T>
{
    public ArrayProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize,bool isPacked)
        : base(itemWriter, tag, static collection => collection.Length, itemFixedSize,isPacked)
    {
    }
}
public sealed class ArrayProtoReader<TItem> : IProtoReader<TItem[]>
{
    private readonly uint _tag;
    public IProtoReader<TItem> ItemReader { get; }
    public int ItemFixedSize { get; }
    public bool IsPacked { get; }

    public ArrayProtoReader(
        IProtoReader<TItem> itemReader,
        uint tag,
        int itemFixedSize,
        bool isPacked
    )
    {
        _tag = tag;
        ItemReader = itemReader;
        ItemFixedSize = itemFixedSize;
        IsPacked = isPacked;
    }

    public TItem[] ParseFrom(ref ReaderContext ctx)
    {
        uint tag = _tag;

        var fixedSize = ItemFixedSize;
        if (IsPacked)
        {
            int length = ctx.ReadLength();
            if (length <= 0)
                return [];
            int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);

            try
            {
                // If the content is fixed size then we can calculate the length
                // of the repeated field and pre-initialize the underlying collection.
                //
                // Check that the supplied length doesn't exceed the underlying buffer.
                // That prevents a malicious length from initializing a very large collection.
                if (
                    fixedSize > 0
                    && length % fixedSize == 0
                    && ParsingPrimitives.IsDataAvailable(ref ctx.state, length)
                )
                {
                    var count = length / fixedSize;
                    var collection = new TItem[count];
                    // if littleEndian treat array as bytes and directly copy from buffer for improved performance
                    // if (
                    //     collection is List<TItem> list
                    //     && BitConverter.IsLittleEndian
                    //     && Marshal.SizeOf<TItem>() == fixedSize
                    // )
                    // {
                    //     var itemSpan = CollectionsMarshal.AsSpan(list);
                    //
                    //     var byteSpan = MemoryMarshal.CreateSpan(
                    //         ref Unsafe.As<TItem, byte>(ref MemoryMarshal.GetReference(itemSpan)),
                    //         checked(itemSpan.Length * fixedSize)
                    //     );
                    //     ParsingPrimitives.ReadPackedFieldLittleEndian(
                    //         ref ctx.buffer,
                    //         ref ctx.state,
                    //         length,
                    //         byteSpan
                    //     );
                    //     CollectionsMarshal.SetCount(list, count);
                    // }
                    // else
                    {
                        int i = 0;
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            // Only FieldCodecs with a fixed size can reach here, and they are all known
                            // types that don't allow the user to specify a custom reader action.
                            // reader action will never return null.
                            collection[i++] =  ItemReader.ParseFrom(ref ctx);
                        }
                    }

                    return collection;
                }
                else
                {
                    var collection = new List<TItem>(4);
                    // Content is variable size so add until we reach the limit.
                    while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                    {
                        collection.Add(ItemReader.ParseFrom(ref ctx));
                    }
                    return collection.ToArray();
                }
            }
            finally
            {
                SegmentedBufferHelper.PopLimit(ref ctx.state, oldLimit);
            }
        }
        else
        {
            // Not packed... (possibly not packable)
            var collection = new List<TItem>(4);
            do
            {
                collection.Add(ItemReader.ParseFrom(ref ctx));
            } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state,  tag));
            return collection.ToArray();
        }
    }
}