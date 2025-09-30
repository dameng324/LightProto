using System.Buffers;
using System.Collections.Concurrent;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
    /// <returns>A new, initialized instance.</returns>
    public static TCollection Deserialize<TCollection, TItem>(Stream source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection, TItem>(source, TItem.ProtoReader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySequence<byte> source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection, TItem>(source, TItem.ProtoReader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySpan<byte> source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection, TItem>(source, TItem.ProtoReader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(
        ReadOnlySequence<byte> source,
        IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new() =>
        Deserialize(source, GetCollectionReader<TCollection, TItem>(reader));

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(
        ReadOnlySpan<byte> source,
        IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new() =>
        Deserialize(source, GetCollectionReader<TCollection, TItem>(reader));

    public static IProtoReader<TCollection> GetCollectionReader<TCollection, TItem>(
        this IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = new IEnumerableProtoReader<TCollection, TItem>(
            reader,
            (capacity) => new TCollection(),
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize: 0
        );
        return new CollectionMessageReader<TCollection, TItem>(collectionReader);
    }

    public static IProtoReader<TCollection> GetCollectionReader<TCollection, TItem>(
        this IProtoReader<TItem> reader,
        Func<int, TCollection> capacityFactory
    )
        where TCollection : ICollection<TItem>
    {
        return GetEnumerableReader(
            reader,
            capacityFactory,
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            }
        );
    }

    public static IProtoReader<TCollection> GetEnumerableReader<TCollection, TItem>(
        this IProtoReader<TItem> reader,
        Func<int, TCollection> capacityFactory,
        Func<TCollection, TItem, TCollection> addItem
    )
        where TCollection : IEnumerable<TItem>
    {
        var collectionReader = new IEnumerableProtoReader<TCollection, TItem>(
            reader,
            capacityFactory,
            addItem,
            itemFixedSize: 0
        );
        return new CollectionMessageReader<TCollection, TItem>(collectionReader);
    }

    public static ArrayProtoReader<TItem> GetArrayReader<TItem>(this IProtoReader<TItem> reader)
    {
        return new ArrayProtoReader<TItem>(reader, itemFixedSize: 0);
    }

    public static IProtoReader<List<TItem>> GetListReader<TItem>(this IProtoReader<TItem> reader)
    {
        return reader.GetCollectionReader<List<TItem>, TItem>(static capacity => new List<TItem>(
            capacity
        ));
    }

    public static IProtoReader<HashSet<TItem>> GetHashSetReader<TItem>(
        this IProtoReader<TItem> reader
    )
    {
        return reader.GetCollectionReader<HashSet<TItem>, TItem>(
            static capacity => new HashSet<TItem>(capacity)
        );
    }

    public static IProtoReader<ConcurrentBag<TItem>> GetConcurrentBagReader<TItem>(
        this IProtoReader<TItem> reader
    )
    {
        return reader.GetEnumerableReader<ConcurrentBag<TItem>, TItem>(
            capacityFactory: static capacity => new ConcurrentBag<TItem>(),
            addItem: (
                (bag, item) =>
                {
                    bag.Add(item);
                    return bag;
                }
            )
        );
    }

    public static IProtoReader<ConcurrentQueue<TItem>> GetConcurrentQueueReader<TItem>(
        this IProtoReader<TItem> reader
    )
    {
        return reader.GetEnumerableReader<ConcurrentQueue<TItem>, TItem>(
            capacityFactory: static capacity => new ConcurrentQueue<TItem>(),
            addItem: (
                (bag, item) =>
                {
                    bag.Enqueue(item);
                    return bag;
                }
            )
        );
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(
        Stream source,
        IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new() =>
        Deserialize(source, GetCollectionReader<TCollection, TItem>(reader));
}

public sealed class CollectionMessageReader<TCollection, T> : IProtoReader<TCollection>
    where TCollection : IEnumerable<T>
{
    private readonly IEnumerableProtoReader<TCollection, T> collectionReader;
    private readonly uint packedTag;
    private readonly uint unPackedTag;

    public CollectionMessageReader(IEnumerableProtoReader<TCollection, T> collectionReader)
    {
        this.collectionReader = collectionReader;
        packedTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        unPackedTag = WireFormat.MakeTag(1, collectionReader.ItemReader.WireType);
    }

    public TCollection ParseFrom(ref ReaderContext ctx)
    {
        uint tag;
        while ((tag = ctx.ReadTag()) != 0)
        {
            if ((tag & 7) == 4)
            {
                break;
            }

            if (tag == packedTag || tag == unPackedTag)
            {
                return collectionReader.ParseMessageFrom(ref ctx);
            }
        }
        return collectionReader.CreateWithCapacity(0);
    }
}
