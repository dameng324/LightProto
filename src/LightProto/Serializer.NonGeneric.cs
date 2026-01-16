using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using LightProto.Parser;

namespace LightProto
{
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
        public static void SerializeNonGeneric(Stream destination, object? instance)
        {
            if (instance is null)
                return;
            SerializeNonGeneric(destination, instance, GetProtoWriter(instance.GetType()));
        }

        /// <summary>
        /// Writes a protocol-buffer representation of the given instance to the supplied writer.
        /// </summary>
        /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
        /// <param name="destination">The destination to write to.</param>
        /// <param name="writer"> The proto-writer to use for serialization, must be IProtoWriter of InstanceType. Can get from <see cref="GetProtoWriter"/> </param>
        public static void SerializeNonGeneric(Stream destination, object? instance, IProtoWriter writer)
        {
            if (instance is null)
                return;
            if (!writer.IsMessage && writer is not ICollectionWriter)
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
        public static void SerializeNonGeneric(IBufferWriter<byte> destination, object? instance)
        {
            if (instance is null)
                return;
            SerializeNonGeneric(destination, instance, GetProtoWriter(instance.GetType()));
        }

        /// <summary>
        /// Writes a protocol-buffer representation of the given instance to the supplied writer.
        /// </summary>
        /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
        /// <param name="destination">The destination to write to.</param>
        /// <param name="writer"> The proto-writer to use for serialization, must be IProtoWriter of InstanceType. Can get from <see cref="GetProtoWriter"/> </param>
        public static void SerializeNonGeneric(IBufferWriter<byte> destination, object? instance, IProtoWriter writer)
        {
            if (instance is null)
                return;
            if (!writer.IsMessage && writer is not ICollectionWriter)
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
        /// <param name="instance"> The message to serialize. </param>
        /// <returns> A byte array containing the serialized message. </returns>
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(AOTWarning)]
        [RequiresUnreferencedCode(AOTWarning)]
#endif
        public static byte[] SerializeToArrayNonGeneric(object? instance)
        {
            if (instance is null)
                return [];
            return SerializeToArrayNonGeneric(instance, GetProtoWriter(instance.GetType()));
        }

        /// <summary>
        /// Serializes the given message to a byte array.
        /// </summary>
        /// <param name="instance"> The message to serialize. </param>
        /// <param name="writer"> The proto-writer to use for serialization, must be IProtoWriter of InstanceType. Can get from <see cref="GetProtoWriter"/> </param>
        /// <returns> A byte array containing the serialized message. </returns>
        public static byte[] SerializeToArrayNonGeneric(object? instance, IProtoWriter writer)
        {
            if (instance is null)
                return [];

            if (!writer.IsMessage && writer is not ICollectionWriter)
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
#endif
        public static object DeserializeNonGeneric(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
            Type type, Stream source) => DeserializeNonGeneric(source, GetProtoReader(type));

        /// <summary>
        /// Creates a new instance from a protocol-buffer stream
        /// </summary>
        /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
        /// <param name="reader"> The proto-reader to use for deserialization, must be IProtoReader of type. Can get from <see cref="GetProtoReader"/> </param>
        /// <returns>A new, initialized instance.</returns>
        public static object DeserializeNonGeneric(Stream source, IProtoReader reader)
        {
            if (!reader.IsMessage)
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
        /// <param name="type">The type to be created.</param>
        /// <param name="source"> The binary stream to apply to the new instance (cannot be null). </param>
        /// <returns>A new, initialized instance.</returns>
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(AOTWarning)]
#endif
        public static object DeserializeNonGeneric(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
            Type type,
            ReadOnlySequence<byte> source
        ) => DeserializeNonGeneric(source, GetProtoReader(type));

        /// <summary>
        /// Creates a new instance from a protocol-buffer stream
        /// </summary>
        /// <param name="reader"> The proto-reader to use for deserialization, must be IProtoReader of type. Can get from <see cref="GetProtoReader"/> </param>
        /// <param name="source"> The binary stream to apply to the new instance (cannot be null). </param>
        /// <returns>A new, initialized instance.</returns>
        public static object DeserializeNonGeneric(ReadOnlySequence<byte> source, IProtoReader reader)
        {
            if (!reader.IsMessage)
            {
                reader = MessageWrapper.ProtoReader.From(reader);
            }
            ReaderContext.Initialize(source, out var ctx);
            return reader.ParseFrom(ref ctx);
        }

        /// <summary>
        /// Creates a new instance from a readonly span of bytes
        /// </summary>
        /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
        /// <param name="type">The type to be created.</param>
        /// <returns>A new, initialized instance.</returns>
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(AOTWarning)]
#endif
        public static object DeserializeNonGeneric(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
            Type type,
            ReadOnlySpan<byte> source
        ) => DeserializeNonGeneric(source, GetProtoReader(type));

        /// <summary>
        /// Creates a new instance from a readonly span of bytes
        /// </summary>
        /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
        /// <param name="reader"> The proto-reader to use for deserialization, must be IProtoReader of type. Can get from <see cref="GetProtoReader"/> </param>
        /// <returns>A new, initialized instance.</returns>
        public static object DeserializeNonGeneric(ReadOnlySpan<byte> source, IProtoReader reader)
        {
            if (!reader.IsMessage)
            {
                reader = MessageWrapper.ProtoReader.From(reader);
            }
            ReaderContext.Initialize(source, out var ctx);
            return reader.ParseFrom(ref ctx);
        }

        /// <summary>
        /// Gets the non-generic proto-reader for the given type.
        /// </summary>
        /// <param name="type"> The type to get the proto-reader for. </param>
        /// <returns></returns>
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(AOTWarning)]
#endif
        public static IProtoReader GetProtoReader(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
            Type type) => (IProtoReader)GetProtoParser(type, isReader: true);

        /// <summary>
        /// Gets the non-generic proto-writer for the given type.
        /// </summary>
        /// <param name="type"> The type to get the proto-writer for. </param>
        /// <returns></returns>
#if NET7_0_OR_GREATER
        [RequiresDynamicCode(AOTWarning)]
#endif
        public static IProtoWriter GetProtoWriter(
#if NET7_0_OR_GREATER
            [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
            Type type) => (IProtoWriter)GetProtoParser(type, isReader: false);
    }
}
