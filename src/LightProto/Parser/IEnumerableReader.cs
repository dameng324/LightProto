using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LightProto.Internal;

namespace LightProto.Parser;

public interface ICollectionReader
{
    public WireFormat.WireType ItemWireType { get; }
}

public interface ICollectionReader<out TCollection> : ICollectionReader
{
    public TCollection Empty { get; }
}

public interface ICollectionItemReader<out TItem> : ICollectionReader
{
    public IProtoReader<TItem> ItemReader { get; }
}

public interface ICollectionReader<out TCollection, out TItem>
    : IProtoReader<TCollection>,
        ICollectionReader<TCollection>,
        ICollectionItemReader<TItem> { }

public class IEnumerableProtoReader<TCollection, TItem> : ICollectionReader<TCollection, TItem>
    where TCollection : IEnumerable<TItem>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => false;
    private readonly Func<List<TItem>, TCollection> _listToCollectionFunc;
    public IProtoReader<TItem> ItemReader { get; }
    public int ItemFixedSize { get; }
    public WireFormat.WireType ItemWireType => ItemReader.WireType;
    public TCollection Empty { get; }

    public IEnumerableProtoReader(
        IProtoReader<TItem> itemReader,
        int itemFixedSize,
        Func<List<TItem>, TCollection> listToCollectionFunc,
        TCollection empty
    )
    {
        _listToCollectionFunc = listToCollectionFunc;
        ItemReader = itemReader;
        ItemFixedSize = itemFixedSize;
        Empty = empty;
    }

    public TCollection ParseFrom(ref ReaderContext ctx)
    {
        var tag = ctx.state.lastTag;
        var fixedSize = ItemFixedSize;
        var list = ListPool<TItem>.Default.Get();
        try
        {
            if (
                WireFormat.GetTagWireType(tag) is WireFormat.WireType.LengthDelimited
                && PackedRepeated.Support<TItem>()
            )
            {
                int length = ctx.ReadLength();
                if (length <= 0)
                    return Empty;
                int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);

                try
                {
#if NET7_0_OR_GREATER
                    // If the content is fixed size then we can calculate the length
                    // of the repeated field and pre-initialize the underlying collection.
                    //
                    // Check that the supplied length doesn't exceed the underlying buffer.
                    // That prevents a malicious length from initializing a very large collection.
                    if (
                        fixedSize > 0
                        && length % fixedSize == 0
                        && ParsingPrimitives.IsDataAvailable(ref ctx.state, length)
                        // if littleEndian treat array as bytes and directly copy from buffer for improved performance
                        && BitConverter.IsLittleEndian
                        && Marshal.SizeOf<TItem>() == fixedSize
                    )
                    {
                        var count = length / fixedSize;
                        CollectionsMarshal.SetCount(list, count);
                        var itemSpan = CollectionsMarshal.AsSpan(list);
                        var byteSpan = MemoryMarshal.CreateSpan(
                            ref Unsafe.As<TItem, byte>(ref MemoryMarshal.GetReference(itemSpan)),
                            checked(itemSpan.Length * fixedSize)
                        );
                        ParsingPrimitives.ReadPackedFieldLittleEndian(
                            ref ctx.buffer,
                            ref ctx.state,
                            length,
                            byteSpan
                        );
                        return _listToCollectionFunc(list);
                    }
                    else
#endif
                    {
                        // Content is variable size so add until we reach the limit.
                        int i = 0;
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            var item = ItemReader.ParseMessageFrom(ref ctx);
                            if (i >= list.Count)
                            {
                                list.Add(item);
                                i++;
                            }
                            else
                                list[i++] = item;
                        }

                        if (list.Count > i)
                            list.RemoveRange(i, list.Count - i);
                        return _listToCollectionFunc(list);
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
                int i = 0;
                do
                {
                    var item = ItemReader.ParseMessageFrom(ref ctx);
                    if (i >= list.Count)
                    {
                        list.Add(item);
                        i++;
                    }
                    else
                    {
                        list[i++] = item;
                    }
                } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));

                if (list.Count > i)
                    list.RemoveRange(i, list.Count - i);
                return _listToCollectionFunc(list);
            }
        }
        finally
        {
            ListPool<TItem>.Default.Return(list);
        }
    }
}
