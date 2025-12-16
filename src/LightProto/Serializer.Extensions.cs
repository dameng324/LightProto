using System.Buffers;
using System.Collections.Concurrent;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    public static int CalculateSize<T>(T message)
    {
        return GetProtoWriter<T>().CalculateSize(message);
    }

    public static byte[] ToByteArray<T>(this T message) =>
        ToByteArray(message, GetProtoWriter<T>());

    public static int CalculateMessageSize<T>(this IProtoWriter<T> writer, T value)
    {
        var size = writer.CalculateSize(value);
        return writer.IsMessage ? CodedOutputStream.ComputeLengthSize(size) + size : size;
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

    internal static byte[] ToByteArray<T>(this T message, IProtoWriter<T> writer)
    {
        if (writer.IsMessage == false && writer is not ICollectionWriter)
        {
            writer = MessageWrapper<T>.ProtoWriter.From(writer);
        }
        var buffer = new byte[writer.CalculateSize(message)];
        using CodedOutputStream output = new CodedOutputStream(buffer);
        WriterContext.Initialize(output, out var ctx);
        writer.WriteTo(ref ctx, message);
        ctx.Flush();
        return buffer;
    }

    public static void SerializeTo<T>(this T instance, Stream destination) =>
        Serialize(destination, instance);

    public static void SerializeTo<T>(this T instance, IBufferWriter<byte> destination) =>
        Serialize(destination, instance);
}
