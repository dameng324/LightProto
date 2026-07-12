using System.Buffers;
using System.Collections.Concurrent;

namespace LightProto
{
    /// <summary>
    /// A pooled, segmented buffer used to bridge synchronous codecs with asynchronous stream I/O.
    /// </summary>
    internal sealed class PooledSegmentBufferWriter : IBufferWriter<byte>
    {
        private const int MinimumSegmentSize = 4096;

        private static readonly ConcurrentBag<PooledSegmentBufferWriter> WriterPool = new();
        private static readonly ConcurrentBag<Segment> SegmentPool = new();

        private Segment? first;
        private Segment? current;

        private PooledSegmentBufferWriter() { }

        public static PooledSegmentBufferWriter Rent()
        {
            if (WriterPool.TryTake(out var writer))
            {
                return writer;
            }

            return new PooledSegmentBufferWriter();
        }

        public static void Return(PooledSegmentBufferWriter writer)
        {
            writer.Reset();
            WriterPool.Add(writer);
        }

        public void Advance(int count)
        {
            if (current is null)
            {
                if (count == 0)
                {
                    return;
                }

                throw new InvalidOperationException("No buffer has been requested.");
            }

            var segment = current;
            if ((uint)count > (uint)(segment.Buffer.Length - segment.Written))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            segment.Written += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            var segment = GetWritableSegment(sizeHint);
            return segment.Buffer.AsMemory(segment.Written);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            var segment = GetWritableSegment(sizeHint);
            return segment.Buffer.AsSpan(segment.Written);
        }

        public async Task ReadToEndAsync(Stream source, CancellationToken cancellationToken)
        {
            while (true)
            {
                var segment = GetWritableSegment(MinimumSegmentSize);
                var read = await source
                    .ReadAsync(segment.Buffer, segment.Written, segment.Buffer.Length - segment.Written, cancellationToken)
                    .ConfigureAwait(false);
                if (read == 0)
                {
                    return;
                }

                segment.Written += read;
            }
        }

        public async Task ReadExactlyAsync(Stream source, int length, CancellationToken cancellationToken)
        {
            var remaining = length;
            while (remaining > 0)
            {
                var segment = GetWritableSegment(Math.Min(remaining, MinimumSegmentSize));
                var read = await source
                    .ReadAsync(
                        segment.Buffer,
                        segment.Written,
                        Math.Min(remaining, segment.Buffer.Length - segment.Written),
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                if (read == 0)
                {
                    throw InvalidProtocolBufferException.TruncatedMessage();
                }

                segment.Written += read;
                remaining -= read;
            }
        }

        public async Task WriteToAsync(Stream destination, CancellationToken cancellationToken)
        {
            for (var segment = first; segment is not null; segment = segment.NextSegment)
            {
                if (segment.Written == 0)
                {
                    continue;
                }

                await destination.WriteAsync(segment.Buffer, 0, segment.Written, cancellationToken).ConfigureAwait(false);
            }
        }

        public ReadOnlySequence<byte> GetReadOnlySequence()
        {
            if (first is null || current is null)
            {
                return ReadOnlySequence<byte>.Empty;
            }

            for (var segment = first; segment is not null; segment = segment.NextSegment)
            {
                segment.SetMemory();
            }

            return new ReadOnlySequence<byte>(first, 0, current, current.Written);
        }

        private Segment GetWritableSegment(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeHint));
            }

            if (current is null || current.Buffer.Length - current.Written < Math.Max(sizeHint, 1))
            {
                var segment = RentSegment(Math.Max(sizeHint, MinimumSegmentSize));
                if (current is null)
                {
                    first = segment;
                }
                else
                {
                    segment.SetRunningIndex(current.RunningIndex + current.Written);
                    current.NextSegment = segment;
                }

                current = segment;
            }

            return current;
        }

        private void Reset()
        {
            var segment = first;
            first = null;
            current = null;

            while (segment is not null)
            {
                var next = segment.NextSegment;
                segment.Reset();
                SegmentPool.Add(segment);
                segment = next;
            }
        }

        private static Segment RentSegment(int minimumSize)
        {
            if (!SegmentPool.TryTake(out var segment))
            {
                segment = new Segment();
            }

            segment.Buffer = ArrayPool<byte>.Shared.Rent(minimumSize);
            return segment;
        }

        private sealed class Segment : ReadOnlySequenceSegment<byte>
        {
            public byte[] Buffer = null!;
            public int Written;

            public Segment? NextSegment
            {
                get => (Segment?)Next;
                set => Next = value;
            }

            public void SetMemory() => Memory = Buffer.AsMemory(0, Written);

            public void SetRunningIndex(long runningIndex) => RunningIndex = runningIndex;

            public void Reset()
            {
                ArrayPool<byte>.Shared.Return(Buffer);
                Buffer = null!;
                Written = 0;
                Memory = default;
                Next = null;
                RunningIndex = 0;
            }
        }
    }
}
