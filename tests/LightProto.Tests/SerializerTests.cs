using System.Buffers;
using System.IO.Compression;
using LightProto.Parser;

namespace LightProto.Tests;

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
    public async Task TestReadRawBytesSlow()
    {
        var obj = Enumerable.Range(0, 1000).Select(o => (byte)o).ToList();
        var bytes = obj.ToByteArray(ByteListProtoParser.ProtoWriter);
        byte[][] bytesArray = bytes.Chunk(1).ToArray();
        var sequence = GetReadonlySequence(bytesArray);
        var parsed = Serializer.Deserialize<List<byte>>(sequence, ByteListProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(obj);
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
    public async Task SmallDeepCloneTest()
    {
        using var ms = new MemoryStream();
        var original = new TestContract()
        {
            Name = Guid.NewGuid().ToString(),
            Bytes = Enumerable.Range(0, 10).Select(i => (byte)(i % 256)).ToArray(),
            Doubles = Enumerable.Range(0, 10).Select(i => (double)(i % 256)).ToArray(),
            Floats = Enumerable.Range(0, 10).Select(i => (float)(i % 256)).ToArray(),
        };
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
    public async Task CollectionTest5()
    {
        string[] original = ["", "123"];
        var bytes = original.ToByteArray(StringProtoParser.ProtoWriter);

        var parsed = Serializer.Deserialize<List<string>, string>(
            bytes,
            StringProtoParser.ProtoReader
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest6()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        string[] original = ["", "123"];
        original.SerializeTo(bufferWriter, StringProtoParser.ProtoWriter);

        var parsed = Serializer.Deserialize<List<string>, string>(
            bufferWriter.WrittenSpan,
            StringProtoParser.ProtoReader
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest8()
    {
        using var ms = new MemoryStream();
        string[] original = ["", "123"];
        original.SerializeTo(ms, StringProtoParser.ProtoWriter);
        ms.Position = 0;

        var parsed = Serializer.Deserialize<List<string>, string>(
            ms,
            StringProtoParser.ProtoReader
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest7()
    {
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        var bytes = original.ToByteArray();

        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest9()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        original.SerializeTo(ms);
        ms.Position = 0;

        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest10()
    {
        TestContract[] original = [];
        var bytes = original.ToByteArray();
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionTest11()
    {
        TestContract[] original = null!;
        var bytes = original.ToByteArray();
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionTest12()
    {
        byte[] bytes = [];
        var parsed = Serializer.Deserialize<List<TestContract>, TestContract>(bytes);
        await Assert.That(bytes.Length).IsEqualTo(0);
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
            var bytes = original.ToByteArray();
            var stream = new MemoryStream(bytes);
            using var codedStream = new CodedInputStream(stream, leaveOpen: false);
            ReaderContext.Initialize(codedStream, out var ctx);
            return TestContract.ProtoReader.ParseFrom(ref ctx);
        }
        var parsed = Deserialize();
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

    [Test]
    public async Task DictionaryTest3()
    {
        using var ms = new MemoryStream();
        Dictionary<List<int>, TestContract> original = new()
        {
            [[1, 3]] = CreateTestContract(),
            [[2, 3]] = CreateTestContract(),
        };
        original.SerializeTo(
            ms,
            Int32ProtoParser.ProtoWriter.GetCollectionWriter(),
            TestContract.ProtoWriter
        );
        ms.Position = 0;

        var parsed = Serializer.Deserialize<
            Dictionary<List<int>, TestContract>,
            List<int>,
            TestContract
        >(
            ms,
            Int32ProtoParser.ProtoReader.GetCollectionReader<List<int>, int>(),
            TestContract.ProtoReader
        );
        await Assert.That(parsed.Count).IsEquivalentTo(original.Count);
        foreach (var kv in original)
        {
            await Assert
                .That(parsed.FirstOrDefault(o => o.Key.SequenceEqual(kv.Key)).Value)
                .IsEquivalentTo(kv.Value);
        }
    }

    [Test]
    public async Task LargeObjectTest()
    {
        using var ms = new FileStream(
            "test.bin",
            new FileStreamOptions()
            {
                Access = FileAccess.Read,
                BufferSize = 10,
                Mode = FileMode.Open,
            }
        );
        var db = Serializer.Deserialize<LightProto.Database>(ms);
        await Assert.That(db.Orders.Count).IsGreaterThan(0);
        var bytes = db.ToByteArray();
        await Assert.That(bytes.Length).IsGreaterThan(0);
    }

    [Test]
    public async Task Test_TriggersLargeSizeSlowPath()
    {
        var original = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            original.SerializeTo(gzip, ByteArrayProtoParser.ProtoWriter);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize<byte[]>(deZip, ByteArrayProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task ExtensibleShouldBeNull()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        IExtension extension = null!;
        await Assert.That(Extensible.GetExtensionObject(ref extension, true)).IsNull();
        var obj = new Extensible();
        await Assert.That(obj.GetExtensionObject(true)).IsNull();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Test]
    public async Task GetArrayReaderTest()
    {
        var original = Enumerable.Range(0, 100).ToList();

        using var ms = new MemoryStream();
        original.SerializeTo(ms, Int32ProtoParser.ProtoWriter.GetCollectionWriter());

        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<int>, int>(ms, Int32ProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task GetArrayReader2Test()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        original.SerializeTo(ms, Int32ProtoParser.ProtoWriter.GetCollectionWriter());

        ms.Position = 0;
        var parsed = Serializer.Deserialize<int[]>(
            ms,
            Int32ProtoParser.ProtoReader.GetArrayMessageReader()
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task GetArrayReaderWithGzipTest()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            original.SerializeTo(gzip, Int32ProtoParser.ProtoWriter.GetCollectionWriter());

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize<int[]>(
            deZip,
            Int32ProtoParser.ProtoReader.GetArrayMessageReader<int>()
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
