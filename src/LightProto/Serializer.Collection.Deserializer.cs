using System.Buffers;
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
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        ReaderContext.Initialize(source, out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(
        ReadOnlySpan<byte> source,
        IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        ReaderContext.Initialize(source, out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }

    private static TCollection ReadCollectionFromContext<TCollection, TItem>(
        ref ReaderContext ctx,
        IEnumerableProtoReader<TCollection, TItem> collectionReader
    )
        where TCollection : ICollection<TItem>, new()
    {
        var packedTag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        var unPackedTag = WireFormat.MakeTag(1, collectionReader.ItemReader.WireType);

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
        return new TCollection();
    }

    internal static IEnumerableProtoReader<TCollection, TItem> GetCollectionReader<
        TCollection,
        TItem
    >(this IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new()
    {
        return new IEnumerableProtoReader<TCollection, TItem>(
            reader,
            (collection) => new TCollection(),
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize: 0
        );
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(
        Stream source,
        IProtoReader<TItem> reader
    )
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        using var codedStream = new CodedInputStream(source, leaveOpen: true);
        ReaderContext.Initialize(codedStream, out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }
}
