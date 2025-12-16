using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    /// <typeparam name="T">The type to be created.</typeparam>
    /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
    /// <returns>A new, initialized instance.</returns>
    public static T Deserialize<T>(Stream source) => Deserialize(source, GetProtoReader<T>());

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySequence<byte> source) =>
        Deserialize(source, GetProtoReader<T>());

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> source) =>
        Deserialize(source, GetProtoReader<T>());

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySequence<byte> source, IProtoReader<T> reader)
    {
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper<T>.ProtoReader.From(reader);
        }

        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    public static T Deserialize<T>(ReadOnlySpan<byte> source, IProtoReader<T> reader)
    {
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper<T>.ProtoReader.From(reader);
        }
        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    internal static T Deserialize<T>(Stream source, IProtoReader<T> reader)
    {
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper<T>.ProtoReader.From(reader);
        }
        using var codedStream = new CodedInputStream(source, leaveOpen: true);
        ReaderContext.Initialize(codedStream, out var ctx);
        return reader.ParseFrom(ref ctx);
    }
}
