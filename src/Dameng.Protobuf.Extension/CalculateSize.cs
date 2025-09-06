using System.Collections;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension;

public static class SizeCalculator
{
    public static bool TryGetFixedSize<T>(DataFormat format, out int fixedSize)
    {
        if (typeof(T) == typeof(double))
        {
            fixedSize = CodedOutputStream.DoubleSize;

            return true;
        }

        if (typeof(T) == typeof(float))
        {
            fixedSize = CodedOutputStream.FloatSize;

            return true;
        }

        if (typeof(T) == typeof(bool))
        {
            fixedSize = CodedOutputStream.BoolSize;

            return true;
        }

        if (format is DataFormat.FixedSize)
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            {
                fixedSize = 4;

                return true;
            }

            if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            {
                fixedSize = 8;

                return true;
            }

            throw new ArgumentOutOfRangeException(
                "FixedSize is only supported for int, uint, long, ulong"
            );
        }

        fixedSize = 0;
        return false;
    }

    public static int Calculate<T>(T t, DataFormat format, int tagSize)
    {
        return tagSize + Calculate(t, format);
    }

    public static int CalculateRepeated<TCollection, TItem>(
        TCollection collection,
        DataFormat format,
        uint tag,
        Func<TCollection, int> getCount,
        Func<TCollection, IEnumerator<TItem>> getEnumerator
    )
    {
        if (collection is null)
            return 0;
        var count = getCount(collection);
        if (count == 0)
        {
            return 0;
        }

        if (FieldCodec<TItem>.IsPackedRepeatedField(tag))
        {
            var dataSize = CalculatePackedDataSize(collection, format, count, getEnumerator);
            return CodedOutputStream.ComputeRawVarint32Size(tag)
                + CodedOutputStream.ComputeLengthSize(dataSize)
                + dataSize;
        }
        else
        {
            return CodedOutputStream.ComputeRawVarint32Size(tag) * count
                + GetAllItemSize(collection, format, getEnumerator);
        }
    }

    static int GetAllItemSize<TCollection, TItem>(
        TCollection collection,
        DataFormat format,
        Func<TCollection, IEnumerator<TItem>> getEnumerator
    )
    {
        int size = 0;
        using IEnumerator<TItem> en = getEnumerator(collection);
        while (en.MoveNext())
        {
            var itemSize = Calculate(en.Current, format);
            // if (itemSize <= 0)
            // {
            //     throw new ArgumentException("repeated item size should be greater than 0");
            // }
            size += itemSize;
        }

        return size;
    }

    public static int CalculatePackedDataSize<TCollection, TItem>(
        TCollection collection,
        DataFormat format,
        int count,
        Func<TCollection, IEnumerator<TItem>> getEnumerator
    )
    {
        TryGetFixedSize<TItem>(format, out var fixedSize);
        return fixedSize == 0
            ? GetAllItemSize(collection, format, getEnumerator)
            : fixedSize * count;
    }

    public static int Calculate<T>(T t, DataFormat format)
    {
        if (typeof(T) == typeof(bool))
        {
            var value = Unsafe.As<T, bool>(ref t);
            return CodedOutputStream.ComputeBoolSize(value);
        }

        if (typeof(T) == typeof(string))
        {
            var value = Unsafe.As<T, string>(ref t);
            return CodedOutputStream.ComputeStringSize(value);
        }

        if (typeof(T) == typeof(int))
        {
            var value = Unsafe.As<T, int>(ref t);
            return format switch
            {
                DataFormat.Default => CodedOutputStream.ComputeInt32Size(value),
                DataFormat.ZigZag => CodedOutputStream.ComputeSInt32Size(value),
                DataFormat.FixedSize => CodedOutputStream.ComputeSFixed32Size(value),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
        }

        if (typeof(T) == typeof(double))
        {
            var value = Unsafe.As<T, double>(ref t);
            return CodedOutputStream.ComputeDoubleSize(value);
        }

        if (typeof(T) == typeof(ByteString))
        {
            var value = Unsafe.As<T, ByteString>(ref t);
            return CodedOutputStream.ComputeBytesSize(value);
        }

        if (typeof(T) == typeof(long))
        {
            var value = Unsafe.As<T, long>(ref t);

            return format switch
            {
                DataFormat.Default => CodedOutputStream.ComputeInt64Size(value),
                DataFormat.ZigZag => CodedOutputStream.ComputeSInt64Size(value),
                DataFormat.FixedSize => CodedOutputStream.ComputeSFixed64Size(value),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
        }

        if (typeof(T) == typeof(float))
        {
            var value = Unsafe.As<T, float>(ref t);
            return CodedOutputStream.ComputeFloatSize(value);
        }

        if (typeof(T) == typeof(uint))
        {
            var value = Unsafe.As<T, uint>(ref t);
            return format switch
            {
                DataFormat.Default => CodedOutputStream.ComputeUInt32Size(value),
                DataFormat.FixedSize => CodedOutputStream.ComputeFixed32Size(value),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
        }

        if (typeof(T) == typeof(ulong))
        {
            var value = Unsafe.As<T, ulong>(ref t);
            return format switch
            {
                DataFormat.Default => CodedOutputStream.ComputeUInt64Size(value),
                DataFormat.FixedSize => CodedOutputStream.ComputeFixed64Size(value),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
            };
        }

        if (typeof(T).IsEnum)
        {
            var value = Unsafe.As<T, int>(ref t);
            return CodedOutputStream.ComputeInt32Size(value);
        }

        if (typeof(T) == typeof(Timestamp))
        {
            var value = Unsafe.As<T, Timestamp>(ref t);
            return CodedOutputStream.ComputeMessageSize(value);
        }
        if (typeof(T) == typeof(Duration))
        {
            var value = Unsafe.As<T, Duration>(ref t);
            return CodedOutputStream.ComputeMessageSize(value);
        }

        if (typeof(T).IsAssignableTo(typeof(IProtoBufMessage)))
        {
            var value = Unsafe.As<T, IProtoBufMessage>(ref t);
            var size = value.CalculateSize();
            return CodedOutputStream.ComputeLengthSize(size) + size;
        }

        throw new NotSupportedException($"Type {typeof(T)} is not supported");
    }
}
