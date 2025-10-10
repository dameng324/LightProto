using System.Buffers;
using System.Collections.Concurrent;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
    /// <returns>A new, initialized instance.</returns>
    public static TCollection Deserialize<TCollection, TItem>(Stream source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection>(
            source,
            GetCollectionMessageReader<TCollection, TItem>(TItem.ProtoReader)
        );

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySequence<byte> source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection>(
            source,
            GetCollectionMessageReader<TCollection, TItem>(TItem.ProtoReader)
        );

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySpan<byte> source)
        where TCollection : ICollection<TItem>, new()
        where TItem : IProtoParser<TItem> =>
        Deserialize<TCollection>(
            source,
            GetCollectionMessageReader<TCollection, TItem>(TItem.ProtoReader)
        );

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied stream.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(Stream destination, ICollection<T> instance)
        where T : IProtoParser<T> =>
        Serialize(destination, instance, T.ProtoWriter.GetCollectionWriter());

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(IBufferWriter<byte> destination, ICollection<T> instance)
        where T : IProtoParser<T> =>
        Serialize(destination, instance, T.ProtoWriter.GetCollectionWriter());
#endif

    public static IProtoWriter<ICollection<T>> GetCollectionWriter<T>(this IProtoWriter<T> writer)
    {
        uint tag = WireFormat.MakeTag(1, writer.WireType);
        return new IEnumerableProtoWriter<ICollection<T>, T>(
            writer,
            tag,
            (collection) => collection.Count,
            itemFixedSize: 0
        );
    }

    public static IEnumerableProtoReader<TCollection, TItem> GetCollectionReader<
        TCollection,
        TItem
    >(this IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new() =>
        new(
            reader,
            (capacity) => new TCollection(),
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize: 0
        );

    public static IProtoReader<TCollection> GetCollectionMessageReader<TCollection, TItem>(
        this IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new() =>
        new CollectionMessageReader<TCollection, TItem>(
            GetCollectionReader<TCollection, TItem>(reader)
        );

    public static IProtoReader<TItem[]> GetArrayMessageReader<TItem>(
        this IProtoReader<TItem> reader
    ) => new CollectionMessageReader<TItem[], TItem>(GetArrayReader(reader));

    internal static IProtoReader<TCollection> GetCollectionMessageReader<TCollection, TItem>(
        this IProtoReader<TItem> reader,
        Func<int, TCollection> capacityFactory
    )
        where TCollection : ICollection<TItem>
    {
        return GetEnumerableMessageReader(
            reader,
            capacityFactory,
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            }
        );
    }

    public static IProtoReader<TCollection> GetEnumerableMessageReader<TCollection, TItem>(
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
        return reader.GetCollectionMessageReader<List<TItem>, TItem>(
            static capacity => new List<TItem>(capacity)
        );
    }

    public static IProtoReader<HashSet<TItem>> GetHashSetReader<TItem>(
        this IProtoReader<TItem> reader
    )
    {
        return reader.GetCollectionMessageReader<HashSet<TItem>, TItem>(
#if NET7_0_OR_GREATER
            static capacity => new HashSet<TItem>(capacity)
#else
            static capacity => new HashSet<TItem>()
#endif
        );
    }

    public static IProtoReader<ConcurrentBag<TItem>> GetConcurrentBagReader<TItem>(
        this IProtoReader<TItem> reader
    )
    {
        return reader.GetEnumerableMessageReader<ConcurrentBag<TItem>, TItem>(
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
        return reader.GetEnumerableMessageReader<ConcurrentQueue<TItem>, TItem>(
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
}

public readonly struct CollectionMessageReader<TCollection, T> : IProtoReader<TCollection>
    where TCollection : IEnumerable<T>
{
    private readonly ICollectionReader<TCollection, T> collectionReader;
    private readonly uint packedTag;
    private readonly uint unPackedTag;

    public CollectionMessageReader(ICollectionReader<TCollection, T> collectionReader)
    {
        this.collectionReader = collectionReader;
        packedTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        unPackedTag = WireFormat.MakeTag(1, collectionReader.ItemReader.WireType);
    }

    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => false;

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
