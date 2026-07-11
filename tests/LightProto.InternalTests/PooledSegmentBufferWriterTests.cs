namespace LightProto.InternalTests;

public class PooledSegmentBufferWriterTests
{
    [Test]
    public async Task BufferWriter_ShouldHandleEmptyAndInvalidAdvanceOperations()
    {
        var emptyWriter = PooledSegmentBufferWriter.Rent();
        try
        {
            emptyWriter.Advance(0);
            await Assert.That(emptyWriter.GetReadOnlySequence().IsEmpty).IsTrue();
            Assert.Throws<InvalidOperationException>(() => emptyWriter.Advance(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => emptyWriter.GetSpan(-1));
        }
        finally
        {
            PooledSegmentBufferWriter.Return(emptyWriter);
        }

        var writer = PooledSegmentBufferWriter.Rent();
        try
        {
            var memory = writer.GetMemory(1);
            memory.Span[0] = 42;
            writer.Advance(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(int.MaxValue));

            var span = writer.GetSpan(1);
            span[0] = 43;
            writer.Advance(1);

            var largeMemory = writer.GetMemory(5000);
            largeMemory.Span[0] = 44;
            writer.Advance(1);

            var sequence = writer.GetReadOnlySequence();
            await Assert.That(sequence.Length).IsEqualTo(3);
            await Assert.That(sequence.FirstSpan[0]).IsEqualTo((byte)42);

            using var destination = new MemoryStream();
            await writer.WriteToAsync(destination, CancellationToken.None);
            await Assert.That(destination.ToArray().Length).IsEqualTo(3);
            await Assert.That(destination.ToArray()[0]).IsEqualTo((byte)42);
        }
        finally
        {
            PooledSegmentBufferWriter.Return(writer);
        }
    }

    [Test]
    public async Task BufferWriter_ShouldReadExactlyOrToEnd()
    {
        var toEndWriter = PooledSegmentBufferWriter.Rent();
        try
        {
            await toEndWriter.ReadToEndAsync(new MemoryStream([1, 2, 3]), CancellationToken.None);
            await Assert.That(toEndWriter.GetReadOnlySequence().Length).IsEqualTo(3);
        }
        finally
        {
            PooledSegmentBufferWriter.Return(toEndWriter);
        }

        var exactWriter = PooledSegmentBufferWriter.Rent();
        try
        {
            await exactWriter.ReadExactlyAsync(new MemoryStream([1, 2, 3]), 3, CancellationToken.None);
            await Assert.That(exactWriter.GetReadOnlySequence().Length).IsEqualTo(3);

            await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
                await exactWriter.ReadExactlyAsync(new MemoryStream(), 1, CancellationToken.None)
            );
        }
        finally
        {
            PooledSegmentBufferWriter.Return(exactWriter);
        }
    }
}
