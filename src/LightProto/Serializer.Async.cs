using System.Runtime.CompilerServices;

namespace LightProto
{
#pragma warning disable RS0026 // CancellationToken is intentionally optional for the asynchronous API surface.
    public static partial class Serializer
    {
        /// <summary>
        /// Asynchronously writes a fully serialized protocol-buffer message to the supplied stream.
        /// The message is encoded synchronously into a pooled buffer before asynchronous I/O begins.
        /// </summary>
        public static async Task SerializeAsync<T>(
            Stream destination,
            T instance,
            IProtoWriter<T> writer,
            CancellationToken cancellationToken = default
        )
        {
            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var buffer = PooledSegmentBufferWriter.Rent();
            try
            {
                Serialize(buffer, instance, writer);
                await buffer.WriteToAsync(destination, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                PooledSegmentBufferWriter.Return(buffer);
            }
        }

        /// <summary>
        /// Asynchronously reads a protocol-buffer message from the supplied stream until the end of the stream.
        /// The accumulated message is decoded synchronously after all input has been received.
        /// </summary>
        public static async Task<T> DeserializeAsync<T>(
            Stream source,
            IProtoReader<T> reader,
            CancellationToken cancellationToken = default
        )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var buffer = PooledSegmentBufferWriter.Rent();
            try
            {
                await buffer.ReadToEndAsync(source, cancellationToken).ConfigureAwait(false);
                return Deserialize(buffer.GetReadOnlySequence(), reader);
            }
            finally
            {
                PooledSegmentBufferWriter.Return(buffer);
            }
        }

#if NET7_0_OR_GREATER
        /// <summary>
        /// Asynchronously writes a fully serialized protocol-buffer message to the supplied stream.
        /// The message is encoded synchronously into a pooled buffer before asynchronous I/O begins.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task SerializeAsync<T>(Stream destination, T instance, CancellationToken cancellationToken = default)
            where T : IProtoParser<T> => SerializeAsync(destination, instance, T.ProtoWriter, cancellationToken);

        /// <summary>
        /// Asynchronously reads a protocol-buffer message from the supplied stream until the end of the stream.
        /// The accumulated message is decoded synchronously after all input has been received.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> DeserializeAsync<T>(Stream source, CancellationToken cancellationToken = default)
            where T : IProtoParser<T> => DeserializeAsync(source, T.ProtoReader, cancellationToken);
#endif
    }
#pragma warning restore RS0026
}
