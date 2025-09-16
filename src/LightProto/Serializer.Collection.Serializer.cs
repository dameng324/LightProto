using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied stream.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(Stream destination, ICollection<T> instance)
        where T : IProtoParser<T> => Serialize(destination, instance, T.Writer);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(IBufferWriter<byte> destination, ICollection<T> instance)
        where T : IProtoParser<T> => Serialize(destination, instance, T.Writer);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    public static void Serialize<T>(
        IBufferWriter<byte> destination,
        ICollection<T> instance,
        IProtoWriter<T> writer
    )
    {
        var collectionWriter = GetCollectionWriter<T>(writer);
        WriterContext.Initialize(destination, out var ctx);
        collectionWriter.WriteTo(ref ctx, instance);
    }

    internal static IProtoWriter<ICollection<T>> GetCollectionWriter<T>(
        this IProtoWriter<T> writer,
        int fieldNumber = 1
    )
    {
        uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        uint tag2 = WireFormat.MakeTag(1, writer.WireType);
        return new IEnumerableProtoWriter<ICollection<T>, T>(
            writer,
            tag,
            (collection) => collection.Count,
            itemFixedSize: 0,
            isPacked: false,
            tag2
        );
    }

    public static void Serialize<T>(
        Stream destination,
        ICollection<T> instance,
        IProtoWriter<T> writer
    )
    {
        var protoWriter = GetCollectionWriter<T>(writer);
        WriterContext.Initialize(new CodedOutputStream(destination), out var ctx);
        protoWriter.WriteTo(ref ctx, instance);
    }
}
