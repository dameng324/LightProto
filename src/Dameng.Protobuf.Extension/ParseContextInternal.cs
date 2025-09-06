using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public static class ParseContextInternal
{
    public static T Read<T>(ref ParseContext ctx, DataFormat format)
    {
        if (typeof(T) == typeof(bool))
        {
            var value = ctx.ReadBool();
            return Unsafe.As<bool, T>(ref value);
        }

        if (typeof(T) == typeof(string))
        {
            var value = ctx.ReadString();
            return Unsafe.As<string, T>(ref value);
        }

        if (typeof(T) == typeof(int))
        {
            int value = format switch
            {
                DataFormat.Default => ctx.ReadInt32(),
                DataFormat.ZigZag => ctx.ReadSInt32(),
                DataFormat.FixedSize => ctx.ReadSFixed32(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
            return Unsafe.As<int, T>(ref value);
        }

        if (typeof(T) == typeof(double))
        {
            var value = ctx.ReadDouble();
            return Unsafe.As<double, T>(ref value);
        }

        if (typeof(T) == typeof(ByteString))
        {
            var value = ctx.ReadBytes();
            return Unsafe.As<ByteString, T>(ref value);
        }

        if (typeof(T) == typeof(byte[]) || typeof(T) == typeof(List<byte>))
        {
            int length = ParsingPrimitives.ParseLength(ref ctx.buffer, ref ctx.state);
            var bytes = ParsingPrimitives.ReadRawBytes(ref ctx.buffer, ref ctx.state, length);
            return Unsafe.As<byte[], T>(ref bytes);
        }

        if (typeof(T) == typeof(long))
        {
            long value = format switch
            {
                DataFormat.Default => ctx.ReadInt64(),
                DataFormat.ZigZag => ctx.ReadSInt64(),
                DataFormat.FixedSize => ctx.ReadSFixed64(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
            return Unsafe.As<long, T>(ref value);
        }

        if (typeof(T) == typeof(float))
        {
            var value = ctx.ReadFloat();
            return Unsafe.As<float, T>(ref value);
        }

        if (typeof(T) == typeof(uint))
        {
            uint value = format switch
            {
                DataFormat.Default => ctx.ReadUInt32(),
                DataFormat.FixedSize => ctx.ReadFixed32(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
            return Unsafe.As<uint, T>(ref value);
        }

        if (typeof(T) == typeof(ulong))
        {
            ulong value = format switch
            {
                DataFormat.Default => ctx.ReadUInt64(),
                DataFormat.FixedSize => ctx.ReadFixed64(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
            return Unsafe.As<ulong, T>(ref value);
        }

        if (typeof(T) == typeof(Enum))
        {
            var value = ctx.ReadEnum();
            return Unsafe.As<int, T>(ref value);
        }

        throw new NotSupportedException($"Type {typeof(T)} is not supported");
    }

    public static TCollection ReadRepeated<TCollection, TItem>(
        ref ParseContext ctx,
        DataFormat format,
        Func<int, TCollection> factory,
        Action<TCollection, TItem> addItem
    )
    {
        uint tag = ctx.state.lastTag;

        SizeCalculator.TryGetFixedSize<TItem>(format, out var fixedSize);
        if (FieldCodec<TItem>.IsPackedRepeatedField(tag))
        {
            int length = ctx.ReadLength();
            if (length <= 0)
                return factory(0);
            int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);

            try
            {
                // If the content is fixed size then we can calculate the length
                // of the repeated field and pre-initialize the underlying collection.
                //
                // Check that the supplied length doesn't exceed the underlying buffer.
                // That prevents a malicious length from initializing a very large collection.
                if (
                    fixedSize > 0
                    && length % fixedSize == 0
                    && ParsingPrimitives.IsDataAvailable(ref ctx.state, length)
                )
                {
                    var count = length / fixedSize;
                    var collection = factory(count);
                    // if littleEndian treat array as bytes and directly copy from buffer for improved performance
                    if (
                        collection is List<TItem> list
                        && BitConverter.IsLittleEndian
                        && Marshal.SizeOf<TItem>() == fixedSize
                    )
                    {
                        var itemSpan = CollectionsMarshal.AsSpan(list);

                        var byteSpan = MemoryMarshal.CreateSpan(
                            ref Unsafe.As<TItem, byte>(ref MemoryMarshal.GetReference(itemSpan)),
                            checked(itemSpan.Length * fixedSize)
                        );
                        ParsingPrimitives.ReadPackedFieldLittleEndian(
                            ref ctx.buffer,
                            ref ctx.state,
                            length,
                            byteSpan
                        );
                        CollectionsMarshal.SetCount(list, count);
                    }
                    else
                    {
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            // Only FieldCodecs with a fixed size can reach here, and they are all known
                            // types that don't allow the user to specify a custom reader action.
                            // reader action will never return null.
                            addItem(collection, Read<TItem>(ref ctx, format));
                        }
                    }

                    return collection;
                }
                else
                {
                    var collection = factory(4);
                    // Content is variable size so add until we reach the limit.
                    while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                    {
                        addItem(collection, Read<TItem>(ref ctx, format));
                    }

                    return collection;
                }
            }
            finally
            {
                SegmentedBufferHelper.PopLimit(ref ctx.state, oldLimit);
            }
        }
        else
        {
            // Not packed... (possibly not packable)
            var collection = factory(4);
            do
            {
                addItem(collection, Read<TItem>(ref ctx, format));
            } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));
            return collection;
        }
    }
}
