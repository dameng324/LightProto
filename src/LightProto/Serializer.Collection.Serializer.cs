using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
#if NET7_0_OR_GREATER
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
}
