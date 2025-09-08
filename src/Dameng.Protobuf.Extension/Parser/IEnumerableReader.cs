using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public class IEnumerableProtoReader<TCollection, TItem> : IProtoReader<TCollection>
    where TCollection : IEnumerable<TItem>
{
    public IProtoReader<TItem> ItemReader { get; }
    public Func<int, TCollection> CreateWithCapacity { get; }
    public Action<TCollection, int, TItem> AddItem { get; }
    public int ItemFixedSize { get; }

    public IEnumerableProtoReader(
        IProtoReader<TItem> itemReader,
        uint tag,
        Func<int, TCollection> createWithCapacity,
        Action<TCollection, int, TItem> addItem,
        int itemFixedSize
    )
    {
        ItemReader = itemReader;
        CreateWithCapacity = createWithCapacity;
        AddItem = addItem;
        ItemFixedSize = itemFixedSize;
    }

    public TCollection ParseFrom(ref ParseContext ctx)
    {
        uint tag = ctx.state.lastTag;

        var fixedSize = ItemFixedSize;
        if (FieldCodec<TItem>.IsPackedRepeatedField(tag))
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
                        int i = 0;
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            // Only FieldCodecs with a fixed size can reach here, and they are all known
                            // types that don't allow the user to specify a custom reader action.
                            // reader action will never return null.
                            AddItem(collection, i++, ItemReader.ParseFrom(ref ctx));
                        }
                    }

                    return collection;
                }
                else
                {
                    var collection = CreateWithCapacity(4);
                    int i = 0;
                    // Content is variable size so add until we reach the limit.
                    while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                    {
                        AddItem(collection, i++, ItemReader.ParseFrom(ref ctx));
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
            int i = 0;
            do
            {
                AddItem(collection, i++, ItemReader.ParseFrom(ref ctx));
            } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));
            return collection;
        }
    }
}
