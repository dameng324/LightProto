namespace LightProto.Parser;

public interface ICollectionReader;
public class IEnumerableProtoReader<TCollection, TItem> : IProtoReader<TCollection>,ICollectionReader
    where TCollection : IEnumerable<TItem>
{
    private readonly uint _tag;
    private readonly Func<TCollection, TCollection>? _completeAction;
    public IProtoReader<TItem> ItemReader { get; }
    public Func<int, TCollection> CreateWithCapacity { get; }
    public Func<TCollection, TItem,TCollection> AddItem { get; }
    public int ItemFixedSize { get; }
    public bool IsPacked { get; }
    public IEnumerableProtoReader(
        IProtoReader<TItem> itemReader,
        uint tag,
        Func<int, TCollection> createWithCapacity,
        Func<TCollection, TItem,TCollection> addItem,
        int itemFixedSize,
        bool isPacked,
        Func<TCollection, TCollection>? completeAction = null
    )
    {
        _tag = tag;
        _completeAction = completeAction;
        ItemReader = itemReader;
        CreateWithCapacity = createWithCapacity;
        AddItem = addItem;
        ItemFixedSize = itemFixedSize;
        IsPacked = isPacked;
    }

    public TCollection ParseFrom(ref ReaderContext ctx)
    {
        var fixedSize = ItemFixedSize;
        if (IsPacked && PackedRepeated.Support<TItem>())
        {
            int length = ctx.ReadLength();
            if (length <= 0)
                return CreateWithCapacity(0);
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
                    var collection = CreateWithCapacity(count);
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
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            // Only FieldCodecs with a fixed size can reach here, and they are all known
                            // types that don't allow the user to specify a custom reader action.
                            // reader action will never return null.
                            collection= AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
                        }
                    }

                    return collection;
                }
                else
                {
                    var collection = CreateWithCapacity(4);
                    // Content is variable size so add until we reach the limit.
                    while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                    {
                        collection= AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
                    }

                    return collection;
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
            var collection = CreateWithCapacity(4);
            do
            {
                collection= AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
            } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, _tag));

            return _completeAction is null ? collection : _completeAction.Invoke(collection);
        }
    }
}
