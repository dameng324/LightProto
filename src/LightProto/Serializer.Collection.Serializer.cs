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
        where T : IProtoParser<T> => Serialize(destination, instance, T.ProtoWriter);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(IBufferWriter<byte> destination, ICollection<T> instance)
        where T : IProtoParser<T> => Serialize(destination, instance, T.ProtoWriter);
#endif

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
        ctx.Flush();
    }

    public static IProtoWriter<ICollection<T>> GetCollectionWriter<T>(this IProtoWriter<T> writer)
    {
        uint tag = WireFormat.MakeTag(1, writer.WireType);
        uint tag2 = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
        return new IEnumerableProtoWriter<ICollection<T>, T>(
            writer,
            tag,
            (collection) => collection.Count,
            itemFixedSize: 0
        );
    }

    public static void Serialize<T>(
        Stream destination,
        ICollection<T> instance,
        IProtoWriter<T> writer
    )
    {
        var protoWriter = GetCollectionWriter<T>(writer);
        using var codedOutputStream = new CodedOutputStream(destination, leaveOpen: true);
        WriterContext.Initialize(codedOutputStream, out var ctx);
        protoWriter.WriteTo(ref ctx, instance);
        ctx.Flush();
    }
}
