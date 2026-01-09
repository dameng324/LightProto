using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination to write to.</param>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static void SerializeNonGeneric(Stream destination, object instance)
    {
        var writer = GetProtoWriter(instance.GetType());
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper.ProtoWriter.From(writer);
        }
        using var codedOutputStream = new CodedOutputStream(destination, leaveOpen: true);
        WriterContext.Initialize(codedOutputStream, out var ctx);
        writer.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination to write to.</param>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static void SerializeNonGeneric(IBufferWriter<byte> destination, object instance)
    {
        var writer = GetProtoWriter(instance.GetType());
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper.ProtoWriter.From(writer);
        }
        WriterContext.Initialize(destination, out var ctx);
        writer.WriteTo(ref ctx, instance);
        ctx.Flush();
    }

    /// <summary>
    /// Serializes the given message to a byte array.
    /// </summary>
    /// <param name="instance"></param>
    /// <returns> </returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static byte[] SerializeToArrayNonGeneric(object instance)
    {
        var writer = GetProtoWriter(instance.GetType());

        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper.ProtoWriter.From(writer);
        }
        var buffer = new byte[writer.CalculateSize(instance)];
        var span = buffer.AsSpan();
        WriterContext.Initialize(ref span, out var ctx);
        writer.WriteTo(ref ctx, instance);
        ctx.Flush();
        return buffer;
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
    /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
    /// <param name="type">The type to be created.</param>
    /// <returns>A new, initialized instance.</returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static object DeserializeNonGeneric(
        Stream source,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type
    )
    {
        var reader = GetProtoReader(type);
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper.ProtoReader.From(reader);
        }
        using var codedStream = new CodedInputStream(source, leaveOpen: true);
        ReaderContext.Initialize(codedStream, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static object DeserializeNonGeneric(
        ReadOnlySequence<byte> source,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type
    )
    {
        var reader = GetProtoReader(type);
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper.ProtoReader.From(reader);
        }
        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    /// <summary>
    /// Creates a new instance from a protocol-buffer stream
    /// </summary>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static object DeserializeNonGeneric(
        ReadOnlySpan<byte> source,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type
    )
    {
        var reader = GetProtoReader(type);
        if (reader.IsMessage == false)
        {
            reader = MessageWrapper.ProtoReader.From(reader);
        }
        ReaderContext.Initialize(source, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static IProtoReader GetProtoReader(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type
    ) => (IProtoReader)GetProtoParser(type, isReader: true);

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(AOTWarning)]
    [RequiresUnreferencedCode(AOTWarning)]
#endif
    public static IProtoWriter GetProtoWriter(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type
    ) => (IProtoWriter)GetProtoParser(type, isReader: false);
}
