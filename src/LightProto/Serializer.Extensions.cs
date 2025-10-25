using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
#if NET7_0_OR_GREATER
    public static byte[] ToByteArray<T>(this T message)
        where T : IProtoParser<T> => ToByteArray(message, T.ProtoWriter);

    public static byte[] ToByteArray<T>(this ICollection<T> message)
        where T : IProtoParser<T> => ToByteArray(message, T.ProtoWriter.GetCollectionWriter());

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied stream.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void SerializeTo<T>(this ICollection<T> instance, Stream destination)
        where T : IProtoParser<T> => Serialize(destination, instance);

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    /// <param name="instance">The existing instance to be serialized (cannot be null).</param>
    /// <param name="destination">The destination stream to write to.</param>
    public static void SerializeTo<T>(this ICollection<T> instance, IBufferWriter<byte> destination)
        where T : IProtoParser<T> => Serialize(destination, instance);
#endif

    public static void WriteMessageTo<T>(
        this IProtoWriter<T> writer,
        ref WriterContext output,
        T value
    )
    {
        if (writer.IsMessage)
        {
            var lengthSpan = output.GetLengthSpan();
            var oldWritten = output.WrittenCount;
            writer.WriteTo(ref output, value);
            var length = output.WrittenCount - oldWritten;
            output.WriteLength(lengthSpan, length);
        }
        else
        {
            writer.WriteTo(ref output, value);
        }
    }

    public static T ParseMessageFrom<T>(this IProtoReader<T> reader, ref ReaderContext input)
    {
        if (reader.IsMessage)
        {
            int length = ParsingPrimitives.ParseLength(ref input.buffer, ref input.state);
            if (input.state.recursionDepth >= input.state.recursionLimit)
            {
                throw InvalidProtocolBufferException.RecursionLimitExceeded();
            }

            int oldLimit = SegmentedBufferHelper.PushLimit(ref input.state, length);
            ++input.state.recursionDepth;
            var message = reader.ParseFrom(ref input);

            if (input.state.lastTag != 0)
            {
                throw InvalidProtocolBufferException.MoreDataAvailable();
            }

            // Check that we've read exactly as much data as expected.
            if (!SegmentedBufferHelper.IsReachedLimit(ref input.state))
            {
                throw InvalidProtocolBufferException.TruncatedMessage();
            }

            --input.state.recursionDepth;
            SegmentedBufferHelper.PopLimit(ref input.state, oldLimit);
            return message;
        }
        else
        {
            return reader.ParseFrom(ref input);
        }
    }

    public static byte[] ToByteArray<T>(this T message, IProtoWriter<T> writer)
    {
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper<T>.ProtoWriter.From(writer);
        }

        using var bufferWriter = new ByteArrayPoolBufferWriter();
        WriterContext ctx = new WriterContext(bufferWriter);
        writer.WriteTo(ref ctx, message);
        return bufferWriter.WrittenMemory.ToArray();
    }

    public static void SerializeTo<T>(
        this T instance,
        Stream destination,
        IProtoWriter<T> writer
    ) => Serialize(destination, instance, writer);

    public static void SerializeTo<T>(
        this T instance,
        IBufferWriter<byte> destination,
        IProtoWriter<T> writer
    ) => Serialize(destination, instance, writer);
}
