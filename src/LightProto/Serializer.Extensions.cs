using System.Buffers;
using System.Collections.Concurrent;
using LightProto.Parser;

namespace LightProto
{
    public static partial class Serializer
    {
#if NET7_0_OR_GREATER
        public static int CalculateSize<T>(T message)
            where T : IProtoParser<T>
        {
            return T.ProtoWriter.CalculateSize(message);
        }

        /// <summary>
        /// Serializes the message to a byte array.
        /// </summary>
        /// <param name="message"> The message to serialize. </param>
        /// <typeparam name="T"> The type of the message. </typeparam>
        /// <returns> A byte array containing the serialized message. </returns>
        public static byte[] ToByteArray<T>(this T message)
            where T : IProtoParser<T> => ToByteArray(message, T.ProtoWriter);
#endif

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

        public static T ParseMessageFrom<T>(this IProtoReader<T> reader, ref ReaderContext input)
        {
            if (reader.IsMessage)
            {
                var length = input.ReadInt64();
                if (input.state.recursionDepth >= input.state.recursionLimit)
                {
                    throw InvalidProtocolBufferException.RecursionLimitExceeded();
                }

                var oldLimit = SegmentedBufferHelper.PushLimit(ref input.state, length);
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

        /// <summary>
        /// Serializes the message to a byte array.
        /// </summary>
        /// <param name="message"> The message to serialize. </param>
        /// <param name="writer"> The proto writer for the message. </param>
        /// <typeparam name="T"> The type of the message. </typeparam>
        /// <returns> A byte array containing the serialized message. </returns>
        public static byte[] ToByteArray<T>(this T message, IProtoWriter<T> writer)
        {
            if (!writer.IsMessage && writer is not ICollectionWriter)
            {
                writer = MessageWrapper<T>.ProtoWriter.From(writer);
            }
            var buffer = new byte[writer.CalculateSize(message)];
            var span = buffer.AsSpan();
            WriterContext.Initialize(ref span, out var ctx);
            writer.WriteTo(ref ctx, message);
            ctx.Flush();
            return buffer;
        }

        /// <summary>
        /// Serializes the instance to the given destination stream.
        /// </summary>
        /// <param name="instance"> The instance to serialize. </param>
        /// <param name="destination"> The destination buffer to serialize to. </param>
        /// <param name="writer"> The proto writer to use for serialization. </param>
        /// <typeparam name="T"> The type of the instance to serialize. </typeparam>
        public static void SerializeTo<T>(
            this T instance,
            Stream destination,
            IProtoWriter<T> writer
        ) => Serialize(destination, instance, writer);

        /// <summary>
        /// Serializes the instance to the given destination buffer.
        /// </summary>
        /// <param name="instance"> The instance to serialize. </param>
        /// <param name="destination"> The destination buffer to serialize to. </param>
        /// <param name="writer"> The proto writer to use for serialization. </param>
        /// <typeparam name="T"> The type of the instance to serialize. </typeparam>
        public static void SerializeTo<T>(
            this T instance,
            IBufferWriter<byte> destination,
            IProtoWriter<T> writer
        ) => Serialize(destination, instance, writer);
    }
}
