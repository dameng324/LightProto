using System.Buffers;

namespace Dameng.Protobuf;

public static partial class Serializer
{
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    /// <typeparam name="T">The type to be created.</typeparam>
    /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
    /// <returns>A new, initialized instance.</returns>
    public static T Deserialize<T>(Stream source)
        where T : IProtoParser<T> => Deserialize(source, T.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlyMemory<byte> source)
        where T : IProtoParser<T> => Deserialize(source, T.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySequence<byte> source)
        where T : IProtoParser<T> => Deserialize(source, T.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> source)
        where T : IProtoParser<T> => Deserialize(source, T.Reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlyMemory<byte> source, IProtoReader<T> reader) =>
        Deserialize(source.Span, reader);

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySequence<byte> source, IProtoReader<T> reader)
    {
        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> source, IProtoReader<T> reader)
    {
        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(Stream source, IProtoReader<T> reader)
    {
        ReaderContext.Initialize(new CodedInputStream(source), out var ctx);
        return reader.ParseFrom(ref ctx);
    }
}
