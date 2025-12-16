using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(IBufferWriter<byte> destination, T instance) =>
        Serialize(destination, instance, GetProtoWriter<T>());

    internal static void Serialize<T>(
        IBufferWriter<byte> destination,
        T instance,
        IProtoWriter<T> writer
    )
    {
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper<T>.ProtoWriter.From(writer);
        }
        WriterContext.Initialize(destination, out var ctx);
        writer.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied stream.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void Serialize<T>(Stream destination, T instance) =>
        Serialize(destination, instance, GetProtoWriter<T>());

    internal static void Serialize<T>(Stream destination, T instance, IProtoWriter<T> writer)
    {
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper<T>.ProtoWriter.From(writer);
        }
        using var codedOutputStream = new CodedOutputStream(destination, leaveOpen: true);
        WriterContext.Initialize(codedOutputStream, out var ctx);
        writer.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    /// <summary>
    /// Creates a deep clone of the given message
    /// </summary>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T DeepClone<T>(T message) =>
        DeepClone(message, GetProtoReader<T>(), GetProtoWriter<T>());

    internal static T DeepClone<T>(T message, IProtoReader<T> reader, IProtoWriter<T> writer)
    {
        var size = writer.CalculateSize(message);
        var array = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var buffer = array.AsSpan(0, size);
            WriterContext.Initialize(ref buffer, out var ctx);
            writer.WriteTo(ref ctx, message);
            ctx.Flush();
            return Deserialize(buffer, reader);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
