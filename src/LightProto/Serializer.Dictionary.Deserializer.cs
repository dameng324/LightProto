using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        ReadOnlySequence<byte> source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull =>
        Deserialize(
            source,
            GetDictionaryMessageReader<TDictionary, TKey, TValue>(keyReader, valueReader)
        );

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static TDictionary Deserialize<TDictionary, TKey, TValue>(
        ReadOnlySpan<byte> source,
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull =>
        Deserialize(
            source,
            GetDictionaryMessageReader<TDictionary, TKey, TValue>(keyReader, valueReader)
        );

    public static IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue> GetDictionaryReader<
        TDictionary,
        TKey,
        TValue
    >(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        return new IEnumerableKeyValuePairProtoReader<TDictionary, TKey, TValue>(
            keyReader,
            valueReader,
            static capacity => new(),
            static (dic, pair) =>
            {
                dic[pair.Key] = pair.Value;
                return dic;
            }
        );
    }

    public static IProtoReader<TDictionary> GetDictionaryMessageReader<TDictionary, TKey, TValue>(
        IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull
    {
        return new CollectionMessageReader<TDictionary, KeyValuePair<TKey, TValue>>(
            GetDictionaryReader<TDictionary, TKey, TValue>(keyReader, valueReader)
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
        where TKey : notnull =>
        Deserialize(
            source,
            GetDictionaryMessageReader<TDictionary, TKey, TValue>(keyReader, valueReader)
        );
}
