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

        [ProtoMember(4)]
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

    public static ReadOnlySequence<byte> GetReadonlySequence(byte[][] bytesArray)
    {
        var firstBufferSegment = new BufferSegment(bytesArray[0]);
        BufferSegment lastBufferSegment = firstBufferSegment;
        for (int i = 1; i < bytesArray.Length; i++)
        {
            lastBufferSegment = lastBufferSegment.Append(bytesArray[i]);
        }
        return new ReadOnlySequence<byte>(
            firstBufferSegment,
            0,
            lastBufferSegment,
            lastBufferSegment.Memory.Length
        );
    }

    [Test]
    public async Task TestReadOnlySequence()
    {
        var obj = CreateTestContract();
        var bytes = obj.ToByteArray();
        var bytesArray = bytes.Chunk(1).ToArray();
        await Assert.That(bytesArray.Length).IsGreaterThan(1);
        var sequence = GetReadonlySequence(bytesArray);
        var parsed = Serializer.Deserialize<TestContract>(sequence);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestReadOnlySequence2()
    {
        var obj = CreateTestContract();
        var bytes = obj.ToByteArray();
        byte[][] bytesArray = [bytes];
        var sequence = GetReadonlySequence(bytesArray);
        var parsed = Serializer.Deserialize<TestContract>(sequence);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task Int32CollectionTest()
    {
        using var ms = new MemoryStream();
        var original = new List<int>() { 1, 2, 3 };
        Serializer.Serialize(ms, original, Int32ProtoParser.ProtoWriter);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<int>, int>(ms, Int32ProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }
    [Test]
    public async Task MessageCollectionTest()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        Serializer.Serialize(ms, original);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest3()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        Serializer.Serialize(ms, original);

        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(ms.ToArray());
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task DeepCloneTest()
    {
        using var ms = new MemoryStream();
        var original = CreateTestContract();
        var parsed = Serializer.DeepClone(original);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest4()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        original.SerializeTo(bufferWriter);

        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(
            GetReadonlySequence(bufferWriter.WrittenSpan.ToArray().Chunk(2).ToArray())
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task DictionaryTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        original.SerializeTo(bufferWriter, Int32ProtoParser.ProtoWriter, TestContract.ProtoWriter);
        var parsed = Serializer.Deserialize<Dictionary<int, TestContract>, int, TestContract>(
            GetReadonlySequence(bufferWriter.WrittenSpan.ToArray().Chunk(2).ToArray()),
            Int32ProtoParser.ProtoReader,
            TestContract.ProtoReader
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task DictionaryTest2()
    {
        using var ms = new MemoryStream();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        original.SerializeTo(ms, Int32ProtoParser.ProtoWriter, TestContract.ProtoWriter);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<Dictionary<int, TestContract>, int, TestContract>(
            ms,
            Int32ProtoParser.ProtoReader,
            TestContract.ProtoReader
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
