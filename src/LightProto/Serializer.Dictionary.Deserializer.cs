using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        ReadOnlyMemory<byte> source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull =>
        Deserialize<TDictionary, TKey, TValue>(source.Span, keyReader, valueReader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        ReadOnlySequence<byte> source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        var collectionReader = GetDictionaryReader<TDictionary, TKey, TValue>(
            keyReader,
            valueReader
        );
        ReaderContext.Initialize(source, out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        ReadOnlySpan<byte> source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        var collectionReader = GetDictionaryReader<TDictionary, TKey, TValue>(
            keyReader,
            valueReader
        );
        ReaderContext.Initialize(source, out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }


    internal static IEnumerableKeyValuePairProtoReader<
        TDictionary,
        TKey,
        TValue
    > GetDictionaryReader<TDictionary, TKey, TValue>(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        uint tag2 = tag;
        uint keyTag = WireFormat.MakeTag(1, keyReader.WireType);
        uint valueTag = WireFormat.MakeTag(2, valueReader.WireType);
        return new IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue>(
            keyReader,
            valueReader,
            tag,
            keyTag,
            valueTag,
            static capacity => new(),
            static (dic, pair) =>
            {
                dic[pair.Key] = pair.Value;
                return dic;
            },
            tag2
        );
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        Stream source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        var collectionReader = GetDictionaryReader<TDictionary, TKey, TValue>(
            keyReader,
            valueReader
        );
        ReaderContext.Initialize(new CodedInputStream(source), out var ctx);
        return ReadCollectionFromContext(ref ctx, collectionReader);
    }
}
