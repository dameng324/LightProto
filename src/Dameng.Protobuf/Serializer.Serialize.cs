using System.Buffers;

namespace Dameng.Protobuf;

public static partial class Serializer
{

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied stream.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(Stream destination, T instance)
        where T : IProtoParser<T> => Serialize(destination, instance, T.Writer);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(IBufferWriter<byte> destination, T instance)
        where T : IProtoParser<T> => Serialize(destination, instance, T.Writer);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    public static void Serialize<T>(
        IBufferWriter<byte> destination,
        T instance,
        IProtoWriter<T> writer
    )
    {
        WriterContext.Initialize(destination, out var ctx);
        writer.WriteTo(ref ctx, instance);
    }

    public static void Serialize<T>(Stream destination, T instance, IProtoWriter<T> writer)
    {
        WriterContext.Initialize(new CodedOutputStream(destination), out var ctx);
        writer.WriteTo(ref ctx, instance);
    }
}
