namespace LightProto.Parser
{
    public interface ICollectionReader
    {
        public WireFormat.WireType ItemWireType { get; }
        public object Empty { get; }
    }

    public interface ICollectionReader<out TCollection> : ICollectionReader
    {
        public new TCollection Empty { get; }
    }

    public interface ICollectionItemReader<out TItem> : ICollectionReader
    {
        public IProtoReader<TItem> ItemReader { get; }
    }

    public interface ICollectionReader<out TCollection, out TItem>
        : IProtoReader,
            IProtoReader<TCollection>,
            ICollectionReader<TCollection>,
            ICollectionItemReader<TItem>;

    public class IEnumerableProtoReader<TCollection, TItem> : ICollectionReader<TCollection, TItem>
        where TCollection : IEnumerable<TItem>
    {
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
        public bool IsMessage => false;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        private readonly Func<TCollection, TCollection>? _completeAction;
        private readonly bool _useListFastPath;
        public IProtoReader<TItem> ItemReader { get; }
        public Func<int, TCollection> CreateWithCapacity { get; }
        public TCollection Empty => CreateWithCapacity(0);
        public Func<TCollection, TItem, TCollection> AddItem { get; }
        int ItemFixedSize { get; }
        public WireFormat.WireType ItemWireType => ItemReader.WireType;
        object ICollectionReader.Empty => Empty;

        public IEnumerableProtoReader(
            IProtoReader<TItem> itemReader,
            Func<int, TCollection> createWithCapacity,
            Func<TCollection, TItem, TCollection> addItem,
            int itemFixedSize,
            Func<TCollection, TCollection>? completeAction = null
        )
        {
            _completeAction = completeAction;
            // Custom readers may provide AddItem behavior beyond List.Add.
            _useListFastPath = GetType() == typeof(ListProtoReader<TItem>);
            ItemReader = itemReader;
            CreateWithCapacity = createWithCapacity;
            AddItem = addItem;
            ItemFixedSize = itemFixedSize;
        }

        public TCollection ParseFrom(ref ReaderContext ctx)
        {
            var tag = ctx.state.lastTag;
            var fixedSize = ItemFixedSize;
            if (WireFormat.GetTagWireType(tag) is WireFormat.WireType.LengthDelimited && PackedRepeated.Support<TItem>())
            {
                var length = ctx.ReadInt64();
                if (length <= 0)
                    return CreateWithCapacity(0);
                var oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);

                try
                {
                    // If the content is fixed size then we can calculate the length
                    // of the repeated field and pre-initialize the underlying collection.
                    //
                    // Check that the supplied length doesn't exceed the underlying buffer.
                    // That prevents a malicious length from initializing a very large collection.
                    if (fixedSize > 0 && length % fixedSize == 0 && ParsingPrimitives.IsDataAvailable(ref ctx.state, length))
                    {
                        var count = length / fixedSize;
                        var collection = CreateWithCapacity((int)count);
#if NET8_0_OR_GREATER
                        if (
                            _useListFastPath
                            && collection is List<TItem> optimizedList
                            && PackedRepeatedOptimizer.TryReadPackedRepeatedFieldLittleEndian(
                                ref ctx,
                                length,
                                optimizedList,
                                (int)count,
                                fixedSize
                            )
                        )
                        {
                            return collection;
                        }
#endif
                        if (_useListFastPath && collection is List<TItem> list)
                        {
                            while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                            {
                                // Only fixed-size built-in field codecs reach this path.
                                list.Add(ItemReader.ParseMessageFrom(ref ctx));
                            }
                        }
                        else
                        {
                            while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                            {
                                collection = AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
                            }
                        }

                        return collection;
                    }
                    else
                    {
                        var collection = CreateWithCapacity(4);
                        // Content is variable size so add until we reach the limit.
                        if (_useListFastPath && collection is List<TItem> list)
                        {
                            while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                            {
                                list.Add(ItemReader.ParseMessageFrom(ref ctx));
                            }
                        }
                        else
                        {
                            while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                            {
                                collection = AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
                            }
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
                if (_useListFastPath && collection is List<TItem> list)
                {
                    do
                    {
                        list.Add(ItemReader.ParseMessageFrom(ref ctx));
                    } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));
                }
                else
                {
                    do
                    {
                        collection = AddItem(collection, ItemReader.ParseMessageFrom(ref ctx));
                    } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));
                }

                return _completeAction is null ? collection : _completeAction.Invoke(collection);
            }
        }
    }
}
