using System.Buffers;

namespace LightProto;

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
        ctx.Flush();
    }

    public static void Serialize<T>(Stream destination, T instance, IProtoWriter<T> writer)
    {
        using var codedOutputStream = new CodedOutputStream(destination);
        WriterContext.Initialize(codedOutputStream, out var ctx);
        writer.WriteTo(ref ctx, instance);
    }

    public static T DeepClone<T>(T message)
        where T : IProtoParser<T>
    {
        unsafe
        {
            var size = T.Writer.CalculateSize(message);
            Span<byte> buffer;
            if (size < 256)
            {
#pragma warning disable CS9081 // A result of a stackalloc expression of this type in this context may be exposed outside of the containing method
                buffer = stackalloc byte[size];
#pragma warning restore CS9081 // A result of a stackalloc expression of this type in this context may be exposed outside of the containing method
            }
            else
            {
                buffer = new byte[size];
            }
            WriterContext.Initialize(ref buffer, out var ctx);
            T.Writer.WriteTo(ref ctx, message);
            return Deserialize<T>(buffer);
        }
    }
}
