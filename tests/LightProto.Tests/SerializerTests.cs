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
        var data = bufferWriter.WrittenMemory.ToArray();
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
        var parsed = Serializer.Deserialize(sequence, ByteListProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(obj);
    }

    [Test]
    public async Task Int32CollectionTest()
    {
        using var ms = new MemoryStream();
        var original = new List<int>() { 1, 2, 3 };
        Serializer.Serialize(ms, original);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<int>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task MessageCollectionTest()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.Serialize(ms, original);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<TestContract>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest3()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.Serialize(ms, original);
        var parsed = Serializer.Deserialize<List<TestContract>>(ms.ToArray());
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
        var parsed = Serializer.Deserialize<List<TestContract>>(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray())
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest5()
    {
        string[] original = ["", "123"];
        var bytes = original.ToByteArray();

        var parsed = Serializer.Deserialize<List<string>>(bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest6()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        string[] original = ["", "123"];
        original.SerializeTo(bufferWriter);

        var parsed = Serializer.Deserialize<List<string>>(bufferWriter.WrittenMemory.Span);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest8()
    {
        using var ms = new MemoryStream();
        string[] original = ["", "123"];
        original.SerializeTo(ms);
        ms.Position = 0;

        var parsed = Serializer.Deserialize<List<string>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest7()
    {
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        var bytes = original.ToByteArray();
        var parsed = Serializer.Deserialize<List<TestContract>>(bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest9()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        original.SerializeTo(ms);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<TestContract>>(ms);
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
        var parsed = Serializer.Deserialize<List<TestContract>>(bytes);
        await Assert.That(parsed.Count).IsEqualTo(0);
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
        original.SerializeTo(bufferWriter);
        var parsed = Serializer.Deserialize<Dictionary<int, TestContract>>(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray())
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
        original.SerializeTo(ms);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<Dictionary<int, TestContract>>(ms);
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
        original.SerializeTo(ms);
        ms.Position = 0;

        var parsed = Serializer.Deserialize<Dictionary<List<int>, TestContract>>(ms);
        await Assert.That(parsed.Count).IsEquivalentTo(original.Count);
        foreach (var kv in original)
        {
            await Assert
                .That(parsed.FirstOrDefault(o => o.Key.SequenceEqual(kv.Key)).Value)
                .IsEquivalentTo(kv.Value);
        }
    }

    [Test]
    public async Task DictionaryTest4()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        var bytes = map.ToByteArray();

        var clone = Serializer.Deserialize<Dictionary<string, int>>(bytes.AsSpan());
        await Assert.That(clone).IsEquivalentTo(map);
    }

#if NET6_0_OR_GREATER
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
#endif

    [Test]
    public async Task Test_TriggersLargeSizeSlowPath()
    {
        var original = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            original.SerializeTo(gzip);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize<byte[]>(deZip);
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
        original.SerializeTo(ms);

        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<int>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task GetArrayReader2Test()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        original.SerializeTo(ms);

        ms.Position = 0;
        var parsed = Serializer.Deserialize<int[]>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task GetArrayReaderWithGzipTest()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            original.SerializeTo(gzip);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize<int[]>(deZip);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [Arguments(PrefixStyle.Base128)]
    [Arguments(PrefixStyle.Fixed32)]
    [Arguments(PrefixStyle.Fixed32BigEndian)]
    public async Task DeserializeItems(PrefixStyle prefixStyle)
    {
        var ms = new MemoryStream();

        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        ms.Position = 0;

        var cloned1 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, prefixStyle);
        var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();

        await Assert.That(cloned1).IsNotNull();
        await Assert.That(cloned.Count).IsEqualTo(2);
    }

    [Test]
    public async Task DeserializeItems2()
    {
        PrefixStyle prefixStyle = PrefixStyle.None;
        var ms = new MemoryStream();
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();
        await Assert.That(cloned.Count).IsEqualTo(0);
    }

    [Test]
    public async Task DeserializeItems3()
    {
        PrefixStyle prefixStyle = (PrefixStyle)4; // Undefined
        var ex1 = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        });
        var ex2 = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var ms = new MemoryStream();
            var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();
        });
    }

    [Test]
    public async Task DeserializeLengthPrefix2()
    {
        var ms = new MemoryStream();
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            fieldNumber: 2
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            fieldNumber: 2
        );
        ms.Position = 0;
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            fieldNumber: 2
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            fieldNumber: 3
        );
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix3()
    {
        var ms = new MemoryStream();
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.Base128);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.Base128);
        ms.Position = 0;
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.Base128);
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.Base128);
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNotNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix_None()
    {
        var ms = new MemoryStream();
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.None);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.None);
        ms.Position = 0;
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.None);
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.None);
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNotNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix_NotMessageType()
    {
        var ms = new MemoryStream();
        int number = 10;
        Serializer.SerializeWithLengthPrefix(ms, number, PrefixStyle.Base128);
        Serializer.SerializeWithLengthPrefix(ms, number, PrefixStyle.Base128);
        ms.Position = 0;
        var cloned = Serializer.DeserializeWithLengthPrefix<int>(ms, PrefixStyle.Base128);
        var cloned2 = Serializer.DeserializeWithLengthPrefix<int>(ms, PrefixStyle.Base128);
        await Assert.That(cloned).IsEqualTo(number);
        await Assert.That(cloned2).IsEqualTo(number);
    }
}
