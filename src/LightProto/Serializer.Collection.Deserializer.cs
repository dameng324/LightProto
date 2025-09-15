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
        where TItem : IProtoParser<TItem> => Deserialize<TCollection, TItem>(source, TItem.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySequence<byte> source)
        where TCollection : ICollection<TItem>, new() 
        where TItem : IProtoParser<TItem> => Deserialize<TCollection, TItem>(source, TItem.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySpan<byte> source)
        where TCollection : ICollection<TItem>, new() 
        where TItem : IProtoParser<TItem>  => Deserialize<TCollection, TItem>(source, TItem.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlyMemory<byte> source, IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new() =>
        Deserialize<TCollection, TItem>(source.Span, reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySequence<byte> source, IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        ReaderContext.Initialize(source, out var ctx);
        return collectionReader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(ReadOnlySpan<byte> source, IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        ReaderContext.Initialize(source, out var ctx);
        return collectionReader.ParseFrom(ref ctx);
    }

    internal static IProtoReader<TCollection> GetCollectionReader<TCollection, TItem>(IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new()
    {
        uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        uint tag2 = tag;
        return new IEnumerableProtoReader<TCollection, TItem>(
            reader,
            tag,
            (collection) => new TCollection(),
            addItem: (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize: 0,
            isPacked: false,
            tag2
        );
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TCollection Deserialize<TCollection, TItem>(Stream source, IProtoReader<TItem> reader)
        where TCollection : ICollection<TItem>, new()
    {
        var collectionReader = GetCollectionReader<TCollection, TItem>(reader);
        ReaderContext.Initialize(new CodedInputStream(source), out var ctx);
        return collectionReader.ParseFrom(ref ctx);
    }
}