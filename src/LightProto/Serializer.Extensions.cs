using System.Buffers;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    public static int CalculateSize<T>(this T message)
        where T : IProtoMessage<T>
    {
        return T.Writer.CalculateSize(message);
    }

    public static int CalculateMessageSize<T>(this IProtoWriter<T> writer, T value)
    {
        var size = writer.CalculateSize(value);
        if (writer.IsMessage)
        {
            return CodedOutputStream.ComputeLengthSize(size) + size;
        }
        else
        {
            return size;
        }
    }

    public static void WriteMessageTo<T>(
        this IProtoWriter<T> writer,
        ref WriterContext output,
        T value
    )
    {
        if (writer.IsMessage)
        {
            output.WriteLength(writer.CalculateSize(value));
        }

        writer.WriteTo(ref output, value);
    }

    public static T ParseFrom<T>(this IProtoReader<T> reader, Stream stream)
    {
        ReaderContext.Initialize(new CodedInputStream(stream), out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    public static T ParseFrom<T>(this IProtoReader<T> reader, ReadOnlySequence<byte> bytes)
    {
        ReaderContext.Initialize(bytes, out var ctx);
        return reader.ParseFrom(ref ctx);
    }

    public static T ParseFrom<T>(this IProtoReader<T> reader, ReadOnlySpan<byte> bytes)
    {
        ReaderContext.Initialize(bytes, out var ctx);
        return reader.ParseFrom(ref ctx);
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
        var buffer = new byte[writer.CalculateSize(message)];
        CodedOutputStream output = new CodedOutputStream(buffer);
        WriterContext.Initialize(output, out var ctx);
        writer.WriteTo(ref ctx, message);
        return buffer;
    }

    public static byte[] ToByteArray<T>(this T message)
        where T : IProtoParser<T>
    {
        return ToByteArray(message, T.Writer);
    }

    public static byte[] ToByteArray<T>(this ICollection<T> message, IProtoWriter<T> writer)
    {
        return ToByteArray(message, writer.GetCollectionWriter());
    }

    public static byte[] ToByteArray<T>(this ICollection<T> message)
        where T : IProtoParser<T>
    {
        return ToByteArray(message, T.Writer);
    }

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

    /// <summary>
    /// Writes a protocol-buffer representation of the given instance to the supplied writer.
    /// </summary>
    public static void SerializeTo<T>(
        this ICollection<T> instance,
        IBufferWriter<byte> destination,
        IProtoWriter<T> writer
    ) => Serialize(destination, instance, writer);

    public static void SerializeTo<T>(
        this ICollection<T> instance,
        Stream destination,
        IProtoWriter<T> writer
    ) => Serialize(destination, instance, writer);
}
