using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LightProto
{
    public static class PackedRepeatedOptimizer
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool TryWritePackedRepeatedFieldLittleEndian<T>(ref WriterContext output, ReadOnlySpan<T> values, int itemFixedSize)
        {
            if (!TryGetBytes(values, itemFixedSize, out var bytes))
            {
                return false;
            }

            WritingPrimitives.WriteRawBytes(ref output.buffer, ref output.state, bytes);
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int ParseRepeatedFieldIntoSpan<T>(
            ref ReaderContext input,
            IProtoReader<T> itemReader,
            Span<T> destination,
            int itemFixedSize
        )
        {
            var tag = input.state.lastTag;
            var writtenCount = 0;

            if (WireFormat.GetTagWireType(tag) is WireFormat.WireType.LengthDelimited && PackedRepeated.Support<T>())
            {
                var length = input.ReadLength();
                if (length <= 0)
                {
                    return 0;
                }

                var oldLimit = SegmentedBufferHelper.PushLimit(ref input.state, length);
                try
                {
                    if (itemFixedSize > 0 && length % itemFixedSize == 0 && ParsingPrimitives.IsDataAvailable(ref input.state, length))
                    {
                        var itemCount = length / itemFixedSize;
                        if (
                            itemCount <= destination.Length
                            && TryReadPackedRepeatedFieldLittleEndian(ref input, length, destination.Slice(0, itemCount), itemFixedSize)
                        )
                        {
                            return itemCount;
                        }
                    }

                    while (!SegmentedBufferHelper.IsReachedLimit(ref input.state))
                    {
                        var item = itemReader.ParseMessageFrom(ref input);
                        if (writtenCount < destination.Length)
                        {
                            destination[writtenCount] = item;
                        }

                        writtenCount++;
                    }

                    return Math.Min(writtenCount, destination.Length);
                }
                finally
                {
                    SegmentedBufferHelper.PopLimit(ref input.state, oldLimit);
                }
            }

            do
            {
                var item = itemReader.ParseMessageFrom(ref input);
                if (writtenCount < destination.Length)
                {
                    destination[writtenCount] = item;
                }

                writtenCount++;
            } while (ParsingPrimitives.MaybeConsumeTag(ref input.buffer, ref input.state, tag));

            return Math.Min(writtenCount, destination.Length);
        }

        internal static bool TryWritePackedRepeatedFieldLittleEndian<TCollection, TItem>(
            ref WriterContext output,
            TCollection collection,
            int count,
            int itemFixedSize
        )
            where TCollection : IEnumerable<TItem>
        {
            if (collection is TItem[] array)
            {
                return TryWritePackedRepeatedFieldLittleEndian(ref output, array.AsSpan(0, count), itemFixedSize);
            }

#if NET5_0_OR_GREATER
            if (collection is List<TItem> list)
            {
                return TryWritePackedRepeatedFieldLittleEndian(ref output, CollectionsMarshal.AsSpan(list).Slice(0, count), itemFixedSize);
            }
#endif

            return false;
        }

#if NET8_0_OR_GREATER
        internal static bool TryReadPackedRepeatedFieldLittleEndian<T>(
            ref ReaderContext input,
            long byteLength,
            List<T> destination,
            int count,
            int itemFixedSize
        )
        {
            if (!CanUseLittleEndianPackedMemoryCopy<T>(itemFixedSize))
            {
                return false;
            }

            CollectionsMarshal.SetCount(destination, count);
            return TryReadPackedRepeatedFieldLittleEndian(
                ref input,
                byteLength,
                CollectionsMarshal.AsSpan(destination).Slice(0, count),
                itemFixedSize
            );
        }
#endif

        internal static bool TryReadPackedRepeatedFieldLittleEndian<T>(
            ref ReaderContext input,
            long byteLength,
            Span<T> destination,
            int itemFixedSize
        )
        {
            if (byteLength > int.MaxValue || !TryGetBytes(destination, itemFixedSize, out var bytes))
            {
                return false;
            }

            var length = (int)byteLength;
            if (bytes.Length < length)
            {
                return false;
            }

            ParsingPrimitives.ReadPackedFieldLittleEndian(ref input.buffer, ref input.state, length, bytes.Slice(0, length));
            return true;
        }

        private static bool TryGetBytes<T>(ReadOnlySpan<T> values, int itemFixedSize, out ReadOnlySpan<byte> bytes)
        {
            bytes = default;
            if (!CanUseLittleEndianPackedMemoryCopy<T>(itemFixedSize))
            {
                return false;
            }

            if (values.IsEmpty)
            {
                bytes = ReadOnlySpan<byte>.Empty;
                return true;
            }

            bytes = CreateSpan(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)), checked(values.Length * itemFixedSize));
            return true;
        }

        private static bool TryGetBytes<T>(Span<T> values, int itemFixedSize, out Span<byte> bytes)
        {
            bytes = default;
            if (!CanUseLittleEndianPackedMemoryCopy<T>(itemFixedSize))
            {
                return false;
            }

            if (values.IsEmpty)
            {
                bytes = Span<byte>.Empty;
                return true;
            }

            bytes = CreateSpan(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)), checked(values.Length * itemFixedSize));
            return true;
        }

#if NETSTANDARD2_0
        private static unsafe Span<T> CreateSpan<T>(ref T reference, int length)
            where T : unmanaged
        {
            fixed (T* ptr = &reference)
            {
                return new Span<T>(ptr, length);
            }
        }
#else
        private static Span<T> CreateSpan<T>(ref T reference, int length)
        {
            return MemoryMarshal.CreateSpan(ref reference, length);
        }
#endif

        private static bool CanUseLittleEndianPackedMemoryCopy<T>(int itemFixedSize)
        {
            if (!BitConverter.IsLittleEndian || itemFixedSize <= 0)
            {
                return false;
            }

            var type = typeof(T);
            if (
                type != typeof(int)
                && type != typeof(uint)
                && type != typeof(long)
                && type != typeof(ulong)
                && type != typeof(float)
                && type != typeof(double)
            )
            {
                return false;
            }

            return Unsafe.SizeOf<T>() == itemFixedSize;
        }
    }
}
