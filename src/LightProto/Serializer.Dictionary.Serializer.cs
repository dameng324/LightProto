using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    public static void Serialize<TKey, TValue>(
        IBufferWriter<byte> destination,
        IDictionary<TKey, TValue> instance,
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull
    {
        var collectionWriter = GetDictionaryWriter(keyWriter, valueWriter);
        WriterContext.Initialize(destination, out var ctx);
        collectionWriter.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    public static void SerializeTo<TKey, TValue>(
        this IDictionary<TKey, TValue> instance,
        IBufferWriter<byte> destination,
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull => Serialize(destination, instance, keyWriter, valueWriter);

    public static byte[] ToByteArray<TKey, TValue>(
        this IDictionary<TKey, TValue> instance,
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull
    {
        var collectionWriter = GetDictionaryWriter(keyWriter, valueWriter);
        return ToByteArray(instance, collectionWriter);
    }

    public static void Serialize<TKey, TValue>(
        Stream destination,
        IDictionary<TKey, TValue> instance,
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull
    {
        using var output = new CodedOutputStream(destination, leaveOpen: true);
        var collectionWriter = GetDictionaryWriter(keyWriter, valueWriter);
        WriterContext.Initialize(output, out var ctx);
        collectionWriter.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    public static void SerializeTo<TKey, TValue>(
        this IDictionary<TKey, TValue> instance,
        Stream destination,
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull => Serialize(destination, instance, keyWriter, valueWriter);

    internal static IProtoWriter<IDictionary<TKey, TValue>> GetDictionaryWriter<TKey, TValue>(
        IProtoWriter<TKey> keyWriter,
        IProtoWriter<TValue> valueWriter
    )
        where TKey : notnull
    {
        uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        uint keyTag = WireFormat.MakeTag(1, keyWriter.WireType);
        uint valueTag = WireFormat.MakeTag(2, valueWriter.WireType);
        return new IEnumerableKeyValuePairProtoWriter<IDictionary<TKey, TValue>, TKey, TValue>(
            keyWriter,
            valueWriter,
            tag,
            (dic) => dic.Count
        );
    }
}
