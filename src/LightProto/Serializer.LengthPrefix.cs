using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    public static IEnumerable<T> DeserializeItems<T>(
        Stream source,
        PrefixStyle style,
        int fieldNumber,
        IProtoReader<T> reader
    )
    {
        while (true)
        {
            var result = DeserializeWithLengthPrefixInternal(
                source,
                style,
                fieldNumber,
                reader,
                out var instance
            );
            if (result is DeserializeWithLengthPrefixResult.NoMoreData)
            {
                yield break;
            }
            if (result is DeserializeWithLengthPrefixResult.PrefixStyleIsNone)
            {
                yield break;
            }
            if (result is DeserializeWithLengthPrefixResult.Success)
            {
                yield return instance;
            }
            else if (result is DeserializeWithLengthPrefixResult.FieldNumberIsMismatched)
            {
                //skip
            }
            else
            {
                throw new NotSupportedException(
                    "DeserializeWithLengthPrefix result is not supported."
                );
            }
        }
    }

    public static T DeserializeWithLengthPrefix<T>(
        Stream source,
        PrefixStyle style,
        IProtoReader<T> reader
    )
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
            if (reader.IsMessage == false)
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
                if (fieldNumberIsMatched == false)
                {
                    //skip the message
                    int left = length;
                    var tempBuffer = new byte[length];
                    while (left > 0)
                    {
                        var read = source.Read(tempBuffer, 0, left);
                        left -= read;
                    }
                    result = default!;
                    return DeserializeWithLengthPrefixResult.FieldNumberIsMismatched;
                }
            }
            else if (style is PrefixStyle.Fixed32)
            {
                if (TryReadFixed32FromStream(source, out var UIntLength) == false)
                {
                    // at end
                    result = default!;
                    return DeserializeWithLengthPrefixResult.NoMoreData;
                }

                length = (int)UIntLength;
            }
            else if (style is PrefixStyle.Fixed32BigEndian)
            {
                if (TryReadFixed32BigEndianFromStream(source, out var UIntLength) == false)
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
        var tmp = source.ReadByte();
        if (tmp < 128)
        {
            return tmp;
        }
        int result = tmp & 0x7f;
        if ((tmp = source.ReadByte()) < 128)
        {
            result |= tmp << 7;
        }
        else
        {
            result |= (tmp & 0x7f) << 7;
            if ((tmp = source.ReadByte()) < 128)
            {
                result |= tmp << 14;
            }
            else
            {
                result |= (tmp & 0x7f) << 14;
                if ((tmp = source.ReadByte()) < 128)
                {
                    result |= tmp << 21;
                }
                else
                {
                    result |= (tmp & 0x7f) << 21;
                    result |= (tmp = source.ReadByte()) << 28;
                    if (tmp >= 128)
                    {
                        // Discard upper 32 bits.
                        // Note that this has to use ReadRawByte() as we only ensure we've
                        // got at least 5 bytes at the start of the method. This lets us
                        // use the fast path in more cases, and we rarely hit this section of code.
                        for (int i = 0; i < 5; i++)
                        {
                            if (source.ReadByte() < 128)
                            {
                                return result;
                            }
                        }
                        throw InvalidProtocolBufferException.MalformedVarint();
                    }
                }
            }
        }
        return result;
    }

    static bool TryReadFixed32FromStream(Stream source, out uint value)
    {
#if NET7_0_OR_GREATER
        Span<byte> bytes = stackalloc byte[4];
        int read = source.Read(bytes);
#else
        byte[] bytes = new byte[4];
        int read = source.Read(bytes, 0, 4);
#endif
        if (read < 4)
        {
            value = 0;
            return false;
        }
        value = BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        return true;
    }

    static bool TryReadFixed32BigEndianFromStream(Stream source, out uint value)
    {
#if NET7_0_OR_GREATER
        Span<byte> bytes = stackalloc byte[4];
        int read = source.Read(bytes);
#else
        byte[] bytes = new byte[4];
        int read = source.Read(bytes, 0, 4);
#endif
        if (read < 4)
        {
            value = 0;
            return false;
        }
        value = BinaryPrimitives.ReadUInt32BigEndian(bytes);
        return true;
    }

    public static T DeserializeWithLengthPrefix<T>(
        Stream source,
        PrefixStyle style,
        int fieldNumber,
        IProtoReader<T> reader
    )
    {
        _ = DeserializeWithLengthPrefixInternal(
            source,
            style,
            fieldNumber,
            reader,
            out var instance
        );
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
            if (writer.IsMessage == false && writer is not ICollectionWriter)
            {
                writer = MessageWrapper<T>.ProtoWriter.From(writer);
            }
            var length = writer.CalculateSize(instance);
            if (style is PrefixStyle.Base128)
            {
                if (fieldNumber > 0)
                {
                    //write tag
                    ctx.WriteTag(
                        WireFormat.MakeTag(fieldNumber, WireFormat.WireType.LengthDelimited)
                    );
                }
                ctx.WriteInt32(length);
            }
            else if (style is PrefixStyle.Fixed32)
            {
                ctx.WriteFixed32((uint)length);
            }
            else if (style is PrefixStyle.Fixed32BigEndian)
            {
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

    public static void SerializeWithLengthPrefix<T>(
        Stream destination,
        T instance,
        PrefixStyle style,
        IProtoWriter<T> writer
    )
    {
        SerializeWithLengthPrefix(destination, instance, style, 0, writer);
    }

#if NET7_0_OR_GREATER
    public static IEnumerable<T> DeserializeItems<T>(
        Stream source,
        PrefixStyle style,
        int fieldNumber
    )
        where T : IProtoParser<T>
    {
        return DeserializeItems(source, style, fieldNumber, T.ProtoReader);
    }

    public static T DeserializeWithLengthPrefix<T>(Stream source, PrefixStyle style)
        where T : IProtoParser<T>
    {
        return DeserializeWithLengthPrefix(source, style, T.ProtoReader);
    }

    public static T DeserializeWithLengthPrefix<T>(
        Stream source,
        PrefixStyle style,
        int fieldNumber,
        IProtoWriter<T> writer
    )
        where T : IProtoParser<T>
    {
        return DeserializeWithLengthPrefix(source, style, fieldNumber, T.ProtoReader);
    }

    public static void SerializeWithLengthPrefix<T>(
        Stream destination,
        T instance,
        PrefixStyle style,
        int fieldNumber
    )
        where T : IProtoParser<T>
    {
        SerializeWithLengthPrefix(destination, instance, style, fieldNumber, T.ProtoWriter);
    }

    public static void SerializeWithLengthPrefix<T>(
        Stream destination,
        T instance,
        PrefixStyle style
    )
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
    /// No length prefix is applied to the data; the data is terminated only be the end of the stream.
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
