using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LightProto.Parser;

namespace LightProto
{
#pragma warning disable RS0026 // CancellationToken is intentionally optional for the asynchronous API surface.
    public static partial class Serializer
    {
        public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, IProtoReader<T> reader)
        {
            return DeserializeItems(source, style, 0, reader);
        }

        public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int fieldNumber, IProtoReader<T> reader)
        {
            while (true)
            {
                var result = DeserializeWithLengthPrefixInternal(source, style, fieldNumber, reader, out var instance);
                switch (result)
                {
                    case DeserializeWithLengthPrefixResult.NoMoreData:
                    case DeserializeWithLengthPrefixResult.PrefixStyleIsNone:
                        yield break;
                    case DeserializeWithLengthPrefixResult.Success:
                        yield return instance;
                        break;
                    case DeserializeWithLengthPrefixResult.FieldNumberIsMismatched:
                        //skip
                        break;
                    default:
                        throw new InvalidOperationException("Unreachable code");
                }
            }
        }

        public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style, IProtoReader<T> reader)
        {
            return DeserializeWithLengthPrefix(source, style, 0, reader);
        }

        internal enum DeserializeWithLengthPrefixResult
        {
            Success,
            PrefixStyleIsNone,
            FieldNumberIsMismatched,
            NoMoreData,
        }

        public static async IAsyncEnumerable<T> DeserializeItemsAsync<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            IProtoReader<T> reader,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
        {
            while (true)
            {
                var (result, instance) = await DeserializeWithLengthPrefixInternalAsync(
                        source,
                        style,
                        fieldNumber,
                        reader,
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                switch (result)
                {
                    case DeserializeWithLengthPrefixResult.NoMoreData:
                    case DeserializeWithLengthPrefixResult.PrefixStyleIsNone:
                        yield break;
                    case DeserializeWithLengthPrefixResult.Success:
                        yield return instance;
                        break;
                    case DeserializeWithLengthPrefixResult.FieldNumberIsMismatched:
                        break;
                    default:
                        throw new InvalidOperationException("Unreachable code");
                }
            }
        }

        public static IAsyncEnumerable<T> DeserializeItemsAsync<T>(
            Stream source,
            PrefixStyle style,
            IProtoReader<T> reader,
            CancellationToken cancellationToken = default
        ) => DeserializeItemsAsync(source, style, 0, reader, cancellationToken);

        public static async Task<T> DeserializeWithLengthPrefixAsync<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            IProtoReader<T> reader,
            CancellationToken cancellationToken = default
        )
        {
            var (_, instance) = await DeserializeWithLengthPrefixInternalAsync(source, style, fieldNumber, reader, cancellationToken)
                .ConfigureAwait(false);
            return instance;
        }

        public static Task<T> DeserializeWithLengthPrefixAsync<T>(
            Stream source,
            PrefixStyle style,
            IProtoReader<T> reader,
            CancellationToken cancellationToken = default
        ) => DeserializeWithLengthPrefixAsync(source, style, 0, reader, cancellationToken);

        static async Task<(DeserializeWithLengthPrefixResult Result, T Instance)> DeserializeWithLengthPrefixInternalAsync<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            IProtoReader<T> reader,
            CancellationToken cancellationToken
        )
        {
            if (style is PrefixStyle.None)
            {
                return (
                    DeserializeWithLengthPrefixResult.PrefixStyleIsNone,
                    await DeserializeAsync(source, reader, cancellationToken).ConfigureAwait(false)
                );
            }

            if (!reader.IsMessage)
            {
                reader = MessageWrapper<T>.ProtoReader.From(reader);
            }

            var prefixBuffer = ArrayPool<byte>.Shared.Rent(5);
            try
            {
                int length;
                var fieldNumberIsMatched = true;
                if (style is PrefixStyle.Base128)
                {
                    if (fieldNumber > 0)
                    {
                        var tag = await ReadVarintFromStreamAsync(source, prefixBuffer, cancellationToken).ConfigureAwait(false);
                        if (tag < 0)
                        {
                            return (DeserializeWithLengthPrefixResult.NoMoreData, default!);
                        }

                        fieldNumberIsMatched = WireFormat.GetTagFieldNumber((uint)tag) == fieldNumber;
                    }

                    length = await ReadVarintFromStreamAsync(source, prefixBuffer, cancellationToken).ConfigureAwait(false);
                    if (length < 0)
                    {
                        return (DeserializeWithLengthPrefixResult.NoMoreData, default!);
                    }
                }
                else if (style is PrefixStyle.Fixed32 || style is PrefixStyle.Fixed32BigEndian)
                {
                    if (!await TryReadExactlyAsync(source, prefixBuffer, 4, cancellationToken).ConfigureAwait(false))
                    {
                        return (DeserializeWithLengthPrefixResult.NoMoreData, default!);
                    }

                    var uintLength =
                        style is PrefixStyle.Fixed32
                            ? BinaryPrimitives.ReadUInt32LittleEndian(prefixBuffer)
                            : BinaryPrimitives.ReadUInt32BigEndian(prefixBuffer);
                    length = (int)uintLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(style));
                }

                if (!fieldNumberIsMatched)
                {
                    await SkipBytesAsync(source, length, cancellationToken).ConfigureAwait(false);
                    return (DeserializeWithLengthPrefixResult.FieldNumberIsMismatched, default!);
                }

                var buffer = PooledSegmentBufferWriter.Rent();
                try
                {
                    await buffer.ReadExactlyAsync(source, length, cancellationToken).ConfigureAwait(false);
                    return (DeserializeWithLengthPrefixResult.Success, Deserialize(buffer.GetReadOnlySequence(), reader));
                }
                finally
                {
                    PooledSegmentBufferWriter.Return(buffer);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(prefixBuffer);
            }
        }

        static async Task<int> ReadVarintFromStreamAsync(Stream source, byte[] buffer, CancellationToken cancellationToken)
        {
            var result = 0;
            var shift = 0;
            for (var i = 0; i < 5; i++)
            {
                if (!await TryReadExactlyAsync(source, buffer, 1, cancellationToken).ConfigureAwait(false))
                {
                    if (i == 0)
                    {
                        return -1;
                    }

                    throw InvalidProtocolBufferException.TruncatedMessage();
                }

                var value = buffer[0];
                result |= (value & 0x7f) << shift;
                if ((value & 0x80) == 0)
                {
                    return result;
                }

                shift += 7;
            }

            throw InvalidProtocolBufferException.MalformedVarint();
        }

        static async Task<bool> TryReadExactlyAsync(Stream source, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            var total = 0;
            while (total < count)
            {
                var read = await source.ReadAsync(buffer, total, count - total, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    if (total == 0)
                    {
                        return false;
                    }

                    throw InvalidProtocolBufferException.TruncatedMessage();
                }

                total += read;
            }

            return true;
        }

        static async Task SkipBytesAsync(Stream source, int length, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(Math.Min(length, 8192));
            try
            {
                var remaining = length;
                while (remaining > 0)
                {
                    var read = await source
                        .ReadAsync(buffer, 0, Math.Min(remaining, buffer.Length), cancellationToken)
                        .ConfigureAwait(false);
                    if (read == 0)
                    {
                        throw InvalidProtocolBufferException.TruncatedMessage();
                    }

                    remaining -= read;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        static DeserializeWithLengthPrefixResult DeserializeWithLengthPrefixInternal<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            IProtoReader<T> reader,
            out T result
        )
        {
            if (style is not PrefixStyle.None)
            {
                if (!reader.IsMessage)
                {
                    reader = MessageWrapper<T>.ProtoReader.From(reader);
                }
                int length;
                if (style is PrefixStyle.Base128)
                {
                    bool fieldNumberIsMatched = true;
                    if (fieldNumber > 0)
                    {
                        //write tag
                        var tag = ReadVarintFromStream(source);
                        if (tag < 0)
                        {
                            //at end;
                            result = default!;
                            return DeserializeWithLengthPrefixResult.NoMoreData;
                        }
                        if (WireFormat.GetTagFieldNumber((uint)tag) != fieldNumber)
                        {
                            fieldNumberIsMatched = false;
                        }
                    }

                    length = ReadVarintFromStream(source);
                    if (length < 0)
                    {
                        // at end
                        result = default!;
                        return DeserializeWithLengthPrefixResult.NoMoreData;
                    }
                    if (!fieldNumberIsMatched)
                    {
                        //skip the message
                        int left = length;
                        byte[]? tempBuffer = null;
                        try
                        {
                            // Use a pooled buffer to avoid large allocations and improve performance.
                            tempBuffer = ArrayPool<byte>.Shared.Rent(Math.Min(left, 8192));
                            while (left > 0)
                            {
                                int toRead = Math.Min(left, tempBuffer.Length);
                                var read = source.Read(tempBuffer, 0, toRead);
                                if (read <= 0)
                                {
                                    // End of stream reached prematurely.
                                    throw InvalidProtocolBufferException.TruncatedMessage();
                                }
                                left -= read;
                            }
                        }
                        finally
                        {
                            if (tempBuffer != null)
                            {
                                ArrayPool<byte>.Shared.Return(tempBuffer);
                            }
                        }
                        result = default!;
                        return DeserializeWithLengthPrefixResult.FieldNumberIsMismatched;
                    }
                }
                else if (style is PrefixStyle.Fixed32)
                {
                    if (!TryReadFixed32FromStream(source, out var UIntLength))
                    {
                        // at end
                        result = default!;
                        return DeserializeWithLengthPrefixResult.NoMoreData;
                    }

                    length = (int)UIntLength;
                }
                else if (style is PrefixStyle.Fixed32BigEndian)
                {
                    if (!TryReadFixed32BigEndianFromStream(source, out var UIntLength))
                    {
                        // at end
                        result = default!;
                        return DeserializeWithLengthPrefixResult.NoMoreData;
                    }
                    length = (int)UIntLength;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(style));
                }
                using var codedStream = new CodedInputStream(source, leaveOpen: true, maxSize: length);
                ReaderContext.Initialize(codedStream, out var ctx);
                result = reader.ParseFrom(ref ctx);
                return DeserializeWithLengthPrefixResult.Success;
            }

            result = Deserialize(source, reader);
            return DeserializeWithLengthPrefixResult.PrefixStyleIsNone;
        }

        static int ReadVarintFromStream(Stream source)
        {
            int result = 0;
            int shift = 0;
            for (int i = 0; i < 5; i++)
            {
                int b = source.ReadByte();
                if (b == -1)
                {
                    if (i == 0)
                    {
                        return -1; // Clean end-of-stream at the start of a new item.
                    }
                    throw InvalidProtocolBufferException.TruncatedMessage();
                }
                result |= (b & 0x7f) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            // If we get here, the 5th byte had the MSB set, which is a malformed varint.
            throw InvalidProtocolBufferException.MalformedVarint();
        }

        static bool TryReadFixed32FromStream(Stream source, out uint value)
        {
            Span<byte> bytes = stackalloc byte[4];
            if (!TryRead4BytesFromStream(source, bytes))
            {
                value = 0;
                return false;
            }
            value = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
            return true;
        }

        static bool TryReadFixed32BigEndianFromStream(Stream source, out uint value)
        {
            Span<byte> bytes = stackalloc byte[4];
            if (!TryRead4BytesFromStream(source, bytes))
            {
                value = 0;
                return false;
            }
            value = BinaryPrimitives.ReadUInt32BigEndian(bytes);
            return true;
        }

        private static bool TryRead4BytesFromStream(Stream source, scoped Span<byte> bytes)
        {
#if NET7_0_OR_GREATER
            try
            {
                source.ReadExactly(bytes);
                return true;
            }
            catch (EndOfStreamException)
            {
                return false;
            }
#else
            byte[] buffer = new byte[4];
            int total = 0;
            while (total < 4)
            {
                int read = source.Read(buffer, total, 4 - total);
                if (read <= 0)
                {
                    if (total == 0)
                    {
                        return false;
                    }
                    throw InvalidProtocolBufferException.TruncatedMessage();
                }
                total += read;
            }
            buffer.CopyTo(bytes);
            return true;
#endif
        }

        public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style, int fieldNumber, IProtoReader<T> reader)
        {
            _ = DeserializeWithLengthPrefixInternal(source, style, fieldNumber, reader, out var instance);
            return instance;
        }

        public static void SerializeWithLengthPrefix<T>(
            Stream destination,
            T instance,
            PrefixStyle style,
            int fieldNumber,
            IProtoWriter<T> writer
        )
        {
            using var codedOutputStream = new CodedOutputStream(destination, leaveOpen: true);
            WriterContext.Initialize(codedOutputStream, out var ctx);
            if (style != PrefixStyle.None)
            {
                if (!writer.IsMessage && writer is not ICollectionWriter)
                {
                    writer = MessageWrapper<T>.ProtoWriter.From(writer);
                }
                var length = writer.CalculateLongSize(instance);
                if (style is PrefixStyle.Base128)
                {
                    if (fieldNumber > 0)
                    {
                        //write tag
                        ctx.WriteTag(WireFormat.MakeTag(fieldNumber, WireFormat.WireType.LengthDelimited));
                    }
                    ctx.WriteLongLength(length);
                }
                else if (style is PrefixStyle.Fixed32)
                {
                    if (length > uint.MaxValue)
                    {
                        throw new OverflowException("Serialized message is too large for Fixed32 length prefix.");
                    }
                    ctx.WriteFixed32((uint)length);
                }
                else if (style is PrefixStyle.Fixed32BigEndian)
                {
                    if (length > uint.MaxValue)
                    {
                        throw new OverflowException("Serialized message is too large for Fixed32BigEndian length prefix.");
                    }
                    ctx.WriteFixedBigEndian32((uint)length);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(style));
                }
                writer.WriteTo(ref ctx, instance);
                ctx.Flush();
                return;
            }

            Serialize(destination, instance, writer);
        }

        public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style, IProtoWriter<T> writer)
        {
            SerializeWithLengthPrefix(destination, instance, style, 0, writer);
        }

        public static async Task SerializeWithLengthPrefixAsync<T>(
            Stream destination,
            T instance,
            PrefixStyle style,
            int fieldNumber,
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
                SerializeWithLengthPrefix(buffer, instance, style, fieldNumber, writer);
                await buffer.WriteToAsync(destination, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                PooledSegmentBufferWriter.Return(buffer);
            }
        }

        public static Task SerializeWithLengthPrefixAsync<T>(
            Stream destination,
            T instance,
            PrefixStyle style,
            IProtoWriter<T> writer,
            CancellationToken cancellationToken = default
        ) => SerializeWithLengthPrefixAsync(destination, instance, style, 0, writer, cancellationToken);

        static void SerializeWithLengthPrefix<T>(
            IBufferWriter<byte> destination,
            T instance,
            PrefixStyle style,
            int fieldNumber,
            IProtoWriter<T> writer
        )
        {
            if (style is PrefixStyle.None)
            {
                Serialize(destination, instance, writer);
                return;
            }

            if (!writer.IsMessage && writer is not ICollectionWriter)
            {
                writer = MessageWrapper<T>.ProtoWriter.From(writer);
            }

            WriterContext.Initialize(destination, out var ctx);
            var length = writer.CalculateLongSize(instance);
            if (style is PrefixStyle.Base128)
            {
                if (fieldNumber > 0)
                {
                    ctx.WriteTag(WireFormat.MakeTag(fieldNumber, WireFormat.WireType.LengthDelimited));
                }

                ctx.WriteLongLength(length);
            }
            else if (style is PrefixStyle.Fixed32)
            {
                if (length > uint.MaxValue)
                {
                    throw new OverflowException("Serialized message is too large for Fixed32 length prefix.");
                }

                ctx.WriteFixed32((uint)length);
            }
            else if (style is PrefixStyle.Fixed32BigEndian)
            {
                if (length > uint.MaxValue)
                {
                    throw new OverflowException("Serialized message is too large for Fixed32BigEndian length prefix.");
                }

                ctx.WriteFixedBigEndian32((uint)length);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(style));
            }

            writer.WriteTo(ref ctx, instance);
            ctx.Flush();
        }

#if NET7_0_OR_GREATER
        public static IAsyncEnumerable<T> DeserializeItemsAsync<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> => DeserializeItemsAsync(source, style, fieldNumber, T.ProtoReader, cancellationToken);

        public static IAsyncEnumerable<T> DeserializeItemsAsync<T>(
            Stream source,
            PrefixStyle style,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> => DeserializeItemsAsync(source, style, 0, T.ProtoReader, cancellationToken);

        public static Task<T> DeserializeWithLengthPrefixAsync<T>(
            Stream source,
            PrefixStyle style,
            int fieldNumber,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> => DeserializeWithLengthPrefixAsync(source, style, fieldNumber, T.ProtoReader, cancellationToken);

        public static Task<T> DeserializeWithLengthPrefixAsync<T>(
            Stream source,
            PrefixStyle style,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> => DeserializeWithLengthPrefixAsync(source, style, T.ProtoReader, cancellationToken);

        public static Task SerializeWithLengthPrefixAsync<T>(
            Stream destination,
            T instance,
            PrefixStyle style,
            int fieldNumber,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> =>
            SerializeWithLengthPrefixAsync(destination, instance, style, fieldNumber, T.ProtoWriter, cancellationToken);

        public static Task SerializeWithLengthPrefixAsync<T>(
            Stream destination,
            T instance,
            PrefixStyle style,
            CancellationToken cancellationToken = default
        )
            where T : IProtoParser<T> => SerializeWithLengthPrefixAsync(destination, instance, style, T.ProtoWriter, cancellationToken);

        public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int fieldNumber)
            where T : IProtoParser<T>
        {
            return DeserializeItems(source, style, fieldNumber, T.ProtoReader);
        }

        public static IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style)
            where T : IProtoParser<T>
        {
            return DeserializeItems<T>(source, style, 0);
        }

        public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style)
            where T : IProtoParser<T>
        {
            return DeserializeWithLengthPrefix(source, style, T.ProtoReader);
        }

        public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style, int fieldNumber)
            where T : IProtoParser<T>
        {
            return DeserializeWithLengthPrefix(source, style, fieldNumber, T.ProtoReader);
        }

        public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style, int fieldNumber)
            where T : IProtoParser<T>
        {
            SerializeWithLengthPrefix(destination, instance, style, fieldNumber, T.ProtoWriter);
        }

        public static void SerializeWithLengthPrefix<T>(Stream destination, T instance, PrefixStyle style)
            where T : IProtoParser<T>
        {
            SerializeWithLengthPrefix(destination, instance, style, T.ProtoWriter);
        }
#endif
    }

    /// <summary>
    /// Specifies the type of prefix that should be applied to messages.
    /// </summary>
    public enum PrefixStyle
    {
        /// <summary>
        /// No length prefix is applied to the data; the data is terminated only by the end of the stream.
        /// </summary>
        None = 0,

        /// <summary>
        /// A base-128 ("varint", the default prefix format in protobuf) length prefix is applied to the data (efficient for short messages).
        /// </summary>
        Base128 = 1,

        /// <summary>
        /// A fixed-length (little-endian) length prefix is applied to the data (useful for compatibility).
        /// </summary>
        Fixed32 = 2,

        /// <summary>
        /// A fixed-length (big-endian) length prefix is applied to the data (useful for compatibility).
        /// </summary>
        Fixed32BigEndian = 3,
    }
}
