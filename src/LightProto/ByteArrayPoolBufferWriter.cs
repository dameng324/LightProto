// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copied from https://github.com/dotnet/corefx/blob/b0751dcd4a419ba6731dcaa7d240a8a1946c934c/src/System.Text.Json/src/System/Text/Json/Serialization/ArrayBufferWriter.cs

using System.Buffers;
using System.Diagnostics;

namespace LightProto;

// Note: this is currently an internal class that will be replaced with a shared version.
internal sealed class ByteArrayPoolBufferWriter : IBufferWriter<byte>, IDisposable
{
    private byte[] _rentedBuffer;
    private int _index;

    private const int MinimumBufferSize = 256;

    public ByteArrayPoolBufferWriter()
    {
        _rentedBuffer = ArrayPool<byte>.Shared.Rent(MinimumBufferSize);
        _index = 0;
    }

    public ByteArrayPoolBufferWriter(int initialCapacity)
    {
        if (initialCapacity <= 0)
            throw new ArgumentException(nameof(initialCapacity));

        _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        _index = 0;
    }

    public ReadOnlyMemory<byte> WrittenMemory
    {
        get
        {
            CheckIfDisposed();

            return _rentedBuffer.AsMemory(0, _index);
        }
    }

    public void WriteToStream(Stream stream)
    {
        stream.Write(_rentedBuffer, 0, _index);
    }

    public int WrittenCount
    {
        get
        {
            CheckIfDisposed();

            return _index;
        }
    }

    public int Capacity
    {
        get
        {
            CheckIfDisposed();

            return _rentedBuffer.Length;
        }
    }

    public int FreeCapacity
    {
        get
        {
            CheckIfDisposed();

            return _rentedBuffer.Length - _index;
        }
    }

    public void Clear()
    {
        CheckIfDisposed();

        ClearHelper();
    }

    private void ClearHelper()
    {
        Debug.Assert(_rentedBuffer != null);

        _rentedBuffer.AsSpan(0, _index).Clear();
        _index = 0;
    }

    // Returns the rented buffer back to the pool
    public void Dispose()
    {
        if (_rentedBuffer == null)
        {
            return;
        }

        ClearHelper();
        ArrayPool<byte>.Shared.Return(_rentedBuffer);
        _rentedBuffer = null!;
    }

    private void CheckIfDisposed()
    {
        if (_rentedBuffer == null)
            throw new ObjectDisposedException(nameof(ByteArrayPoolBufferWriter));
    }

    public void Advance(int count)
    {
        CheckIfDisposed();

        if (count < 0)
            throw new ArgumentException(nameof(count));

        if (_index > _rentedBuffer.Length - count)
            ThrowInvalidOperationException(_rentedBuffer.Length);

        _index += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        CheckIfDisposed();

        CheckAndResizeBuffer(sizeHint);
        return _rentedBuffer.AsMemory(_index);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        CheckIfDisposed();

        CheckAndResizeBuffer(sizeHint);
        return _rentedBuffer.AsSpan(_index);
    }

    private void CheckAndResizeBuffer(int sizeHint)
    {
        Debug.Assert(_rentedBuffer != null);

        if (sizeHint < 0)
            throw new ArgumentException(nameof(sizeHint));

        if (sizeHint == 0)
        {
            sizeHint = MinimumBufferSize;
        }

        int availableSpace = _rentedBuffer!.Length - _index;

        if (sizeHint > availableSpace)
        {
            int growBy = Math.Max(sizeHint, _rentedBuffer.Length);

            int newSize = checked(_rentedBuffer.Length + growBy);

            byte[] oldBuffer = _rentedBuffer;

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

            Debug.Assert(oldBuffer.Length >= _index);
            Debug.Assert(_rentedBuffer.Length >= _index);

            Span<byte> previousBuffer = oldBuffer.AsSpan(0, _index);
            previousBuffer.CopyTo(_rentedBuffer);
            previousBuffer.Clear();
            ArrayPool<byte>.Shared.Return(oldBuffer);
        }

        Debug.Assert(_rentedBuffer.Length - _index > 0);
        Debug.Assert(_rentedBuffer.Length - _index >= sizeHint);
    }

    private static void ThrowInvalidOperationException(int capacity)
    {
        throw new InvalidOperationException("BufferWriterAdvancedTooFar");
    }
}
