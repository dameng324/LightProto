

using System.Runtime.CompilerServices;

namespace Dameng.Protobuf;

public static class Extensions
{
    public static int CalculateSize<T>(this T message) where T : IProtoMessage<T>
    {
        return T.Writer.CalculateSize(message);
    }
    public static int CalculateMessageSize<T>(this T message) where T : IProtoMessage<T>
    {
        var size = T.Writer.CalculateSize(message);
        return CodedOutputStream.ComputeLengthSize(size) + size;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
}