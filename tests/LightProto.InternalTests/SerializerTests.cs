using System.Buffers;

namespace LightProto.InternalTests;

public partial class SerializerTests
{
    [ProtoContract]
    public partial class TestContract
    {
        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(22)]
        public byte[] Bytes { get; set; } = [];

        [ProtoMember(333)]
        public double[] Doubles { get; set; } = [];

        [ProtoMember(444444444)]
        public float[] Floats { get; set; } = [];
    }

    TestContract CreateTestContract()
    {
        return new TestContract()
        {
            Name = Guid.NewGuid().ToString(),
            Bytes = Enumerable.Range(0, 1000).Select(i => (byte)(i % 256)).ToArray(),
            Doubles = Enumerable.Range(0, 1000).Select(i => (double)(i % 256)).ToArray(),
            Floats = Enumerable.Range(0, 1000).Select(i => (float)(i % 256)).ToArray(),
        };
    }

    class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(byte[] memory)
        {
            Memory = memory;
        }

        public BufferSegment Append(byte[] memory)
        {
            var segment = new BufferSegment(memory) { RunningIndex = this.RunningIndex + this.Memory.Length };
            this.Next = segment;
            return segment;
        }
    }

    public static ReadOnlySequence<byte> GetReadonlySequence(byte[][] bytesArray)
    {
        var firstBufferSegment = new BufferSegment(bytesArray[0]);
        BufferSegment lastBufferSegment = firstBufferSegment;
        for (int i = 1; i < bytesArray.Length; i++)
        {
            lastBufferSegment = lastBufferSegment.Append(bytesArray[i]);
        }
        return new ReadOnlySequence<byte>(firstBufferSegment, 0, lastBufferSegment, lastBufferSegment.Memory.Length);
    }

    [Test]
    public async Task SerializeTest13()
    {
        byte[] Serialize()
        {
            using var ms = new MemoryStream();
            var original = CreateTestContract();
            using var codedOutputStream = new CodedOutputStream(ms, leaveOpen: false);
            WriterContext.Initialize(codedOutputStream, out var ctx);
            TestContract.ProtoWriter.WriteTo(ref ctx, original);
            ctx.Flush();
            return ms.ToArray();
        }
        var bytes = Serialize();
        await Assert.That(bytes.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task DeserializeTest()
    {
        var original = CreateTestContract();
        TestContract Deserialize()
        {
            var bytes = original.ToByteArray<TestContract>(TestContract.ProtoWriter);
            var stream = new MemoryStream(bytes);
            using var codedStream = new CodedInputStream(stream, leaveOpen: false);
            ReaderContext.Initialize(codedStream, out var ctx);
            return TestContract.ProtoReader.ParseFrom(ref ctx);
        }
        var parsed = Deserialize();
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
