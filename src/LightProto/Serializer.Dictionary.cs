using System.Buffers;
using System.Collections.Concurrent;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    public static IProtoReader<Dictionary<TKey, TValue>> GetDictionaryReader<TKey, TValue>(
        this IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TKey : notnull
    {
        return new DictionaryProtoReader<TKey, TValue>(keyReader, valueReader);
    }

    public static IProtoReader<ConcurrentDictionary<TKey, TValue>> GetConcurrentDictionaryReader<
        TKey,
        TValue
    >(this IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        where TKey : notnull
    {
        return new ConcurrentDictionaryProtoReader<TKey, TValue>(keyReader, valueReader);
    }

    public static IProtoReader<SortedDictionary<TKey, TValue>> GetSortedDictionaryReader<
        TKey,
        TValue
    >(this IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        where TKey : notnull
    {
        return new SortedDictionaryProtoReader<TKey, TValue>(keyReader, valueReader);
    }

    public static IProtoReader<SortedList<TKey, TValue>> GetSortedListReader<TKey, TValue>(
        this IProtoReader<TKey> keyReader,
        IProtoReader<TValue> valueReader
    )
        where TKey : notnull
    {
        return new SortedListProtoReader<TKey, TValue>(keyReader, valueReader);
    }

    public static IProtoWriter<IDictionary<TKey, TValue>> GetDictionaryWriter<TKey, TValue>(
        this IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull
    {
        uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        return new IEnumerableKeyValuePairProtoWriter<IDictionary<TKey, TValue>, TKey, TValue>(
            keyWriter,
            valueWriter,
            tag,
            (dic) => dic.Count
        );
    }
}
