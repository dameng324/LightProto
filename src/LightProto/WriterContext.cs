#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion

using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace LightProto
{
    /// <summary>
    /// An opaque struct that represents the current serialization state and is passed along
    /// as the serialization proceeds.
    /// All the public methods are intended to be invoked only by the generated code,
    /// users should never invoke them directly.
    /// </summary>
    [SecuritySafeCritical]
    public ref struct WriterContext
    {
        private readonly IBufferWriter<byte> _bufferWriter;
        public int WrittenCount { get; set; }

        public WriterContext(IBufferWriter<byte> bufferWriter)
        {
            _bufferWriter = bufferWriter;
            WrittenCount = 0;
        }

        /// <summary>
        /// Writes a double field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteDouble(double value)
        {
            var span = _bufferWriter.GetSpan(8);
            BinaryPrimitives.WriteUInt64LittleEndian(
                span,
                (ulong)BitConverter.DoubleToInt64Bits(value)
            );
            Advance(8);
        }

        void Advance(int count)
        {
            _bufferWriter.Advance(count);
            WrittenCount += count;

            if (_bufferWriter is ByteArrayPoolBufferWriter writer)
            {
                if (writer.WrittenCount != WrittenCount)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Writes a float field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteFloat(float value)
        {
            var span = _bufferWriter.GetSpan(4);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(span), value);
            Advance(4);
        }

        /// <summary>
        /// Writes a uint64 field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt64(ulong value) => WriteRawVarint64(value);

        private void WriteRawVarint64(ulong value)
        {
            // Optimize for the common case of a single byte value
            if (value < 128)
            {
                var span = _bufferWriter.GetSpan(1);
                span[0] = (byte)value;
                Advance(1);
                return;
            }

            // Fast path when capacity is available
            while (true)
            {
                if (value > 127)
                {
                    var span = _bufferWriter.GetSpan(1);
                    span[0] = (byte)((value & 0x7F) | 0x80);
                    Advance(1);
                    value >>= 7;
                }
                else
                {
                    var span = _bufferWriter.GetSpan(1);
                    span[0] = (byte)value;
                    Advance(1);
                    return;
                }
            }
        }

        /// <summary>
        /// Writes an int64 field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteInt64(long value) => WriteRawVarint64((ulong)value);

        /// <summary>
        /// Writes an int32 field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteInt32(int value)
        {
            WriteRawVarint64((ulong)value);
        }

        /// <summary>
        /// Writes a fixed64 field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteFixed64(ulong value)
        {
            const int length = sizeof(ulong);
            var span = _bufferWriter.GetSpan(length);
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
            Advance(length);
        }

        /// <summary>
        /// Writes a fixed32 field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteFixed32(uint value)
        {
            const int length = sizeof(uint);
            var span = _bufferWriter.GetSpan(length);
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
            Advance(length);
        }

        /// <summary>
        /// Writes a bool field value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteBool(bool value)
        {
            var span = _bufferWriter.GetSpan(1);
            span[0] = (byte)(value ? 1 : 0);
            Advance(1);
        }

        /// <summary>
        /// Writes a string field value, without a tag.
        /// The data is length-prefixed.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteString(string value)
        {
            var lengthSpan = _bufferWriter.GetSpan(5);
            Advance(5);

            var maxByteCount = (value.Length + 1) * 3;
            var dest = _bufferWriter.GetSpan(maxByteCount);
#if NET7_0_OR_GREATER
            var status = System.Text.Unicode.Utf8.FromUtf16(
                value,
                dest,
                out var _,
                out var bytesWritten,
                replaceInvalidSequences: false
            );
            if (status != OperationStatus.Done)
            {
                throw InvalidProtocolBufferException.StringWriteFailed(status.ToString());
            }
#else
            int bytesWritten;
            unsafe
            {
                fixed (char* sourceChars = &MemoryMarshal.GetReference(value.AsSpan()))
                fixed (byte* destinationBytes = &MemoryMarshal.GetReference(dest))
                {
                    bytesWritten = Encoding.UTF8.GetBytes(
                        sourceChars,
                        value.Length,
                        destinationBytes,
                        maxByteCount
                    );
                }
            }
#endif
            WriteLength(lengthSpan, bytesWritten);
            Advance(bytesWritten);
        }

        public void WriteBytes(Span<byte> bytes)
        {
            var lengthSpan = _bufferWriter.GetSpan(5);
            WriteLength(lengthSpan, bytes.Length);
            Advance(5);
            var span = _bufferWriter.GetSpan(bytes.Length);
            bytes.CopyTo(span);
            Advance(bytes.Length);
        }

        /// <summary>
        /// Writes a uint32 value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteUInt32(uint value)
        {
            WriteRawVarint64(value);
        }

        /// <summary>
        /// Writes an enum value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteEnum(int value) => WriteRawVarint64((ulong)value);

        /// <summary>
        /// Writes an sfixed32 value, without a tag.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteSFixed32(int value)
        {
            WriteFixed32((uint)value);
        }

        /// <summary>
        /// Writes an sfixed64 value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteSFixed64(long value) => WriteFixed64((ulong)value);

        /// <summary>
        /// Writes an sint32 value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteSInt32(int value)
        {
            WriteRawVarint64(EncodeZigZag32(value));
        }

        static uint EncodeZigZag32(int n)
        {
            // Note:  the right-shift must be arithmetic
            return (uint)((n << 1) ^ (n >> 31));
        }

        public static ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }

        /// <summary>
        /// Writes an sint64 value, without a tag.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void WriteSInt64(long value) => WriteRawVarint64(EncodeZigZag64(value));

        /// <summary>
        /// Writes a length (in bytes) for length-delimited data.
        /// </summary>
        /// <remarks>
        /// This method simply writes a rawint, but exists for clarity in calling code.
        /// </remarks>
        /// <param name="length">Length value, in bytes.</param>
        public void WriteLength(Span<byte> span, int value)
        {
            span[0] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
            span[1] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
            span[2] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
            span[3] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
            span[4] = (byte)((value & 0x7F) | 0x00);
        }

        /// <summary>
        /// Writes an already-encoded tag.
        /// </summary>
        /// <param name="tag">The encoded tag</param>
        public void WriteTag(uint tag) => WriteRawVarint64(tag);

        internal Span<byte> GetLengthSpan()
        {
            var span = _bufferWriter.GetSpan(5);
            Advance(5);
            return span;
        }
    }
}
