using System.Buffers;
using LightProto.Parser;

namespace LightProto.Tests;

public partial class SerializerTests
{
    [ProtoContract]
    public partial class TestContract
    {
        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(2)]
        public byte[] Bytes { get; set; } = [];

        [ProtoMember(3)]
        public double[] Doubles { get; set; } = [];
    }

    TestContract CreateTestContract()
    {
        return new TestContract()
        {
            Name = Guid.NewGuid().ToString(),
            Bytes = Enumerable.Range(0, 1000).Select(i => (byte)(i % 256)).ToArray(),
            Doubles = Enumerable.Range(0, 1000).Select(i => (double)(i % 256)).ToArray(),
        };
    }

    [Test]
    public async Task TestBufferWriter()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        var obj = CreateTestContract();
        Serializer.Serialize(bufferWriter, obj);
        var data = bufferWriter.WrittenSpan.ToArray();

        var parsed = Serializer.Deserialize<TestContract>(data);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestStream()
    {
        using var ms = new MemoryStream();
        var obj = CreateTestContract();
        Serializer.Serialize(ms, obj);

        using var ms2 = new MemoryStream(ms.ToArray());
        var parsed = Serializer.Deserialize<TestContract>(ms2);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(byte[] memory)
        {
            Memory = memory;
        }

        public BufferSegment Append(byte[] memory)
        {
            var segment = new BufferSegment(memory)
            {
                RunningIndex = this.RunningIndex + this.Memory.Length,
            };
            this.Next = segment;
            return segment;
        }
    }

    [Test]
    public async Task TestReadOnlySequence()
    {
        var obj = CreateTestContract();
        var bytes = obj.ToByteArray();
        var bytesArray = bytes.Chunk(1).ToArray();
        await Assert.That(bytesArray.Length).IsGreaterThan(1);
        var firstBufferSegment = new BufferSegment(bytesArray[0]);
        BufferSegment lastBufferSegment = firstBufferSegment;
        for (int i = 1; i < bytesArray.Length; i++)
        {
            lastBufferSegment = lastBufferSegment.Append(bytesArray[i]);
        }
        var sequence = new ReadOnlySequence<byte>(
            firstBufferSegment,
            0,
            lastBufferSegment,
            lastBufferSegment.Memory.Length
        );
        var parsed = Serializer.Deserialize<TestContract>(sequence);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task CollectionTests()
    {
        using var ms = new MemoryStream();
        var original = new List<int>() { 1, 2, 3 };
        Serializer.Serialize(ms, original, Int32ProtoParser.ProtoWriter);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<int>, int>(ms, Int32ProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
