using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Transactions;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension;

public static class WriteContextInternal
{
    public static void Write<T>(ref WriteContext output, T t, DataFormat format, uint tag)
    {
        output.WriteTag(tag);
        Write(ref output, t, format);
    }

    public static void WriteRepeated<TCollection, TItem>(
        ref WriteContext ctx,
        TCollection collection,
        DataFormat format,
        uint tag,
        Func<TCollection, int> getCount,
        Func<TCollection, IEnumerator<TItem>> getEnumerator
    )
    {
        if (collection == null)
        {
            return;
        }

        var count = getCount(collection);
        if (count == 0)
        {
            return;
        }

        if (FieldCodec<TItem>.IsPackedRepeatedField(tag))
        {
            // Packed primitive type
            int size = SizeCalculator.CalculatePackedDataSize(collection, format, count, getEnumerator);
            ctx.WriteTag(tag);
            ctx.WriteLength(size);

            // if littleEndian and elements has fixed size, treat array as bytes (and write it as bytes to buffer) for improved performance
            // if(TryGetArrayAsSpanPinnedUnsafe(codec, out Span<byte> span, out GCHandle handle))
            // {
            //     span = span.Slice(0, Count * codec.FixedSize);
            //
            //     WritingPrimitives.WriteRawBytes(ref ctx.buffer, ref ctx.state, span);
            //     handle.Free();
            // }
            // else
            {
                using IEnumerator<TItem> en = getEnumerator(collection);
                while (en.MoveNext())
                {
                    var current = en.Current;
                    if (current is null)
                    {
                        throw new Exception("Sequence contained null element");
                    }

                    Write(ref ctx, en.Current, format);
                }
            }
        }
        else
        {
            using IEnumerator<TItem> en = getEnumerator(collection);
            while (en.MoveNext())
            {
                Write(ref ctx, en.Current, format, tag);
            }
        }
    }

    public static void Write<T>(ref WriteContext output, T t, DataFormat format)
    {
        if (typeof(T) == typeof(bool))
        {
            var value = Unsafe.As<T, bool>(ref t);
            output.WriteBool(value);
            return;
        }

        if (typeof(T) == typeof(string))
        {
            var value = Unsafe.As<T, string>(ref t);
            output.WriteString(value);
            return;
        }

        if (typeof(T) == typeof(int))
        {
            var value = Unsafe.As<T, int>(ref t);
            switch (format)
            {
                case DataFormat.Default:
                    output.WriteInt32(value);
                    break;
                case DataFormat.ZigZag:
                    output.WriteSInt32(value);
                    break;
                case DataFormat.FixedSize:
                    output.WriteSFixed32(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return;
        }

        if (typeof(T) == typeof(double))
        {
            var value = Unsafe.As<T, double>(ref t);
            output.WriteDouble(value);
            return;
        }

        if (typeof(T) == typeof(ByteString))
        {
            var value = Unsafe.As<T, ByteString>(ref t);
            output.WriteBytes(value);
            return;
        }

        if (typeof(T) == typeof(long))
        {
            var value = Unsafe.As<T, long>(ref t);
            switch (format)
            {
                case DataFormat.Default:
                    output.WriteInt64(value);
                    break;
                case DataFormat.ZigZag:
                    output.WriteSInt64(value);
                    break;
                case DataFormat.FixedSize:
                    output.WriteSFixed64(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return;
        }

        if (typeof(T) == typeof(float))
        {
            var value = Unsafe.As<T, float>(ref t);
            output.WriteFloat(value);
            return;
        }

        if (typeof(T) == typeof(uint))
        {
            var value = Unsafe.As<T, uint>(ref t);
            switch (format)
            {
                case DataFormat.Default:
                    output.WriteUInt32(value);
                    break;
                case DataFormat.FixedSize:
                    output.WriteFixed32(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return;
        }

        if (typeof(T) == typeof(ulong))
        {
            var value = Unsafe.As<T, ulong>(ref t);
            switch (format)
            {
                case DataFormat.Default:
                    output.WriteUInt64(value);
                    break;
                case DataFormat.FixedSize:
                    output.WriteFixed64(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return;
        }

        if (typeof(T).IsEnum)
        {
            var value = Unsafe.As<T, int>(ref t);
            output.WriteEnum(value);
            return;
        }

        if (typeof(T) == typeof(Timestamp))
        {
            var value = Unsafe.As<T, Timestamp>(ref t);
            output.WriteMessage(value);
            return;
        }
        if (typeof(T) == typeof(Duration))
        {
            var value = Unsafe.As<T, Duration>(ref t);
            output.WriteMessage(value);
            return;
        }
        if (typeof(T).IsAssignableTo(typeof(IMessage)))
        {
            var value = Unsafe.As<T, IMessage>(ref t);
            output.WriteMessage(value);
            return;
        }

        throw new NotSupportedException($"Type {typeof(T)} is not supported");
    }
}
