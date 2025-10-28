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
#if NET6_0_OR_GREATER
        Serializer.Serialize(bufferWriter, obj);
        var data = bufferWriter.WrittenSpan.ToArray();
        var parsed = Serializer.Deserialize<TestContract>(data);
#else
        Serializer.Serialize(bufferWriter, obj, TestContract.ProtoWriter);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = Serializer.Deserialize<TestContract>(data, TestContract.ProtoReader);
#endif
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestStream()
    {
        using var ms = new MemoryStream();
        var obj = CreateTestContract();

#if NET6_0_OR_GREATER
        Serializer.Serialize(ms, obj);
#else
        Serializer.Serialize(ms, obj, TestContract.ProtoWriter);
#endif

        using var ms2 = new MemoryStream(ms.ToArray());
#if NET6_0_OR_GREATER
        var parsed = Serializer.Deserialize<TestContract>(ms2);
#else
        var parsed = Serializer.Deserialize<TestContract>(ms2, TestContract.ProtoReader);
#endif
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
#if NET6_0_OR_GREATER
        var bytes = obj.ToByteArray();
#else
        var bytes = obj.ToByteArray(TestContract.ProtoWriter);
#endif
        var bytesArray = bytes.Chunk(1).ToArray();
        await Assert.That(bytesArray.Length).IsGreaterThan(1);
        var sequence = GetReadonlySequence(bytesArray);
#if NET6_0_OR_GREATER
        var parsed = Serializer.Deserialize<TestContract>(sequence);
#else
        var parsed = Serializer.Deserialize<TestContract>(sequence, TestContract.ProtoReader);
#endif
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestReadOnlySequence2()
    {
        var obj = CreateTestContract();
#if NET6_0_OR_GREATER
        var bytes = obj.ToByteArray();
#else
        var bytes = obj.ToByteArray(TestContract.ProtoWriter);
#endif
        byte[][] bytesArray = [bytes];
        var sequence = GetReadonlySequence(bytesArray);

#if NET6_0_OR_GREATER
        var parsed = Serializer.Deserialize<TestContract>(sequence);
#else
        var parsed = Serializer.Deserialize<TestContract>(sequence, TestContract.ProtoReader);
#endif
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
        Serializer.Serialize(ms, original, Int32ProtoParser.ProtoWriter.GetCollectionWriter());
        ms.Position = 0;
        var parsed = Serializer.Deserialize(ms, Int32ProtoParser.ProtoReader.GetListReader());
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task MessageCollectionTest()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

#if NET6_0_OR_GREATER
        Serializer.Serialize(ms, original);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<TestContract>>(
            ms,
            TestContract.ProtoReader.GetListReader()
        );
#else
        Serializer.Serialize(ms, original, TestContract.ProtoWriter.GetCollectionWriter());
        ms.Position = 0;
        var parsed = Serializer.Deserialize(ms, TestContract.ProtoReader.GetListReader());
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest3()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

#if NET6_0_OR_GREATER
        Serializer.Serialize(ms, original);
        var parsed = Serializer.Deserialize<List<TestContract>>(
            ms.ToArray(),
            TestContract.ProtoReader.GetListReader()
        );
#else
        Serializer.Serialize(ms, original, TestContract.ProtoWriter.GetCollectionWriter());
        var parsed = Serializer.Deserialize(ms.ToArray(), TestContract.ProtoReader.GetListReader());
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task DeepCloneTest()
    {
        using var ms = new MemoryStream();
        var original = CreateTestContract();
#if NET6_0_OR_GREATER
        var parsed = Serializer.DeepClone(original);
#else
        var parsed = Serializer.DeepClone(
            original,
            TestContract.ProtoReader,
            TestContract.ProtoWriter
        );
#endif
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
#if NET6_0_OR_GREATER
        var parsed = Serializer.DeepClone(original);
#else
        var parsed = Serializer.DeepClone(
            original,
            TestContract.ProtoReader,
            TestContract.ProtoWriter
        );
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest4()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
#if NET6_0_OR_GREATER
        original.SerializeTo(bufferWriter);
        var parsed = Serializer.Deserialize<List<TestContract>>(
            GetReadonlySequence(bufferWriter.WrittenSpan.ToArray().Chunk(2).ToArray()),
            TestContract.ProtoReader.GetListReader()
        );
#else
        original.SerializeTo(bufferWriter, TestContract.ProtoWriter.GetCollectionWriter());
        var parsed = Serializer.Deserialize(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray()),
            TestContract.ProtoReader.GetListReader<TestContract>()
        );
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest5()
    {
        string[] original = ["", "123"];
        var bytes = original.ToByteArray(StringProtoParser.ProtoWriter.GetCollectionWriter());

        var parsed = Serializer.Deserialize(
            bytes,
            StringProtoParser.ProtoReader.GetListReader<string>()
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest6()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        string[] original = ["", "123"];
        original.SerializeTo(bufferWriter, StringProtoParser.ProtoWriter.GetCollectionWriter());

        var parsed = Serializer.Deserialize(
            bufferWriter.WrittenMemory.Span,
            StringProtoParser.ProtoReader.GetListReader<string>()
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest8()
    {
        using var ms = new MemoryStream();
        string[] original = ["", "123"];
        original.SerializeTo(ms, StringProtoParser.ProtoWriter.GetCollectionWriter());
        ms.Position = 0;

        var parsed = Serializer.Deserialize(
            ms,
            StringProtoParser.ProtoReader.GetListReader<string>()
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest7()
    {
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

#if NET6_0_OR_GREATER
        var bytes = original.ToByteArray();
        var parsed = Serializer.Deserialize<List<TestContract>>(
            bytes,
            TestContract.ProtoReader.GetListReader()
        );
#else
        var bytes = original.ToByteArray(TestContract.ProtoWriter.GetCollectionWriter());
        var parsed = Serializer.Deserialize(
            bytes,
            TestContract.ProtoReader.GetListReader<TestContract>()
        );
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest9()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
#if NET6_0_OR_GREATER
        original.SerializeTo(ms);
        ms.Position = 0;

        var parsed = Serializer.Deserialize<List<TestContract>>(
            ms,
            TestContract.ProtoReader.GetListReader()
        );
#else
        original.SerializeTo(ms, TestContract.ProtoWriter.GetCollectionWriter());
        ms.Position = 0;
        var parsed = Serializer.Deserialize(
            ms,
            TestContract.ProtoReader.GetListReader<TestContract>()
        );
#endif
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task CollectionTest10()
    {
        TestContract[] original = [];
#if NET6_0_OR_GREATER
        var bytes = original.ToByteArray();
#else
        var bytes = original.ToByteArray(TestContract.ProtoWriter.GetCollectionWriter());
#endif
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionTest11()
    {
        TestContract[] original = null!;
#if NET6_0_OR_GREATER
        var bytes = original.ToByteArray();
#else
        var bytes = original.ToByteArray(TestContract.ProtoWriter.GetCollectionWriter());
#endif
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    public async Task CollectionTest12()
    {
        byte[] bytes = [];
#if NET6_0_OR_GREATER
        var parsed = Serializer.Deserialize<List<TestContract>>(
            bytes,
            TestContract.ProtoReader.GetListReader()
        );
#else
        var parsed = Serializer.Deserialize(
            bytes,
            TestContract.ProtoReader.GetListReader<TestContract>()
        );
#endif
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
#if NET6_0_OR_GREATER
            var bytes = original.ToByteArray();
#else
            var bytes = original.ToByteArray(TestContract.ProtoWriter);
#endif
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
        original.SerializeTo(
            bufferWriter,
            Int32ProtoParser.ProtoWriter.GetDictionaryWriter(TestContract.ProtoWriter)
        );
        var parsed = Serializer.Deserialize(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray()),
            Int32ProtoParser.ProtoReader.GetDictionaryReader<int, TestContract>(
                TestContract.ProtoReader
            )
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task ConcurrentDictionaryReaderTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        original.SerializeTo(
            bufferWriter,
            Int32ProtoParser.ProtoWriter.GetDictionaryWriter(TestContract.ProtoWriter)
        );
        var parsed = Serializer.Deserialize(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray()),
            Int32ProtoParser.ProtoReader.GetConcurrentDictionaryReader<int, TestContract>(
                TestContract.ProtoReader
            )
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task SortedDictionaryReaderTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        original.SerializeTo(
            bufferWriter,
            Int32ProtoParser.ProtoWriter.GetDictionaryWriter(TestContract.ProtoWriter)
        );
        var parsed = Serializer.Deserialize(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray()),
            Int32ProtoParser.ProtoReader.GetSortedDictionaryReader<int, TestContract>(
                TestContract.ProtoReader
            )
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task SortedListReaderTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        original.SerializeTo(
            bufferWriter,
            Int32ProtoParser.ProtoWriter.GetDictionaryWriter(TestContract.ProtoWriter)
        );
        var parsed = Serializer.Deserialize(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray()),
            Int32ProtoParser.ProtoReader.GetSortedListReader<int, TestContract>(
                TestContract.ProtoReader
            )
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
        original.SerializeTo(
            ms,
            Int32ProtoParser.ProtoWriter.GetDictionaryWriter(TestContract.ProtoWriter)
        );
        ms.Position = 0;
        var parsed = Serializer.Deserialize(
            ms,
            Int32ProtoParser.ProtoReader.GetDictionaryReader<int, TestContract>(
                TestContract.ProtoReader
            )
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
        IProtoWriter<List<int>> keyWriter = Int32ProtoParser.ProtoWriter.GetCollectionWriter();
        original.SerializeTo(ms, keyWriter.GetDictionaryWriter(TestContract.ProtoWriter));
        ms.Position = 0;

        IProtoReader<List<int>> keyReader = Int32ProtoParser.ProtoReader.GetListReader<int>();

        var parsed = Serializer.Deserialize(
            ms,
            keyReader.GetDictionaryReader<List<int>, TestContract>(TestContract.ProtoReader)
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
    public async Task DictionaryTest4()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        var bytes = map.ToByteArray(
            StringProtoParser.ProtoWriter.GetDictionaryWriter(Int32ProtoParser.ProtoWriter)
        );

        var clone = Serializer.Deserialize(
            bytes.AsSpan(),
            StringProtoParser.ProtoReader.GetDictionaryReader<string, int>(
                Int32ProtoParser.ProtoReader
            )
        );
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
            original.SerializeTo(gzip, ByteArrayProtoParser.ProtoWriter);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize(deZip, ByteArrayProtoParser.ProtoReader);
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
        var parsed = Serializer.Deserialize(ms, Int32ProtoParser.ProtoReader.GetListReader<int>());
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task GetArrayReader2Test()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        original.SerializeTo(ms, Int32ProtoParser.ProtoWriter.GetCollectionWriter());

        ms.Position = 0;
        var parsed = Serializer.Deserialize(ms, Int32ProtoParser.ProtoReader.GetArrayReader());
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
        var parsed = Serializer.Deserialize(deZip, Int32ProtoParser.ProtoReader.GetArrayReader());
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [Arguments(PrefixStyle.Base128)]
    [Arguments(PrefixStyle.Fixed32)]
    [Arguments(PrefixStyle.Fixed32BigEndian)]
    public async Task DeserializeItemsTest(PrefixStyle prefixStyle)
    {
        var ms = new MemoryStream();
#if NET6_0_OR_GREATER
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            prefixStyle,
            TestContract.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            prefixStyle,
            TestContract.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            prefixStyle,
            TestContract.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
        var cloned1 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, prefixStyle);
        var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();
#else
        var cloned1 = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            prefixStyle,
            TestContract.ProtoReader
        );
        var cloned = Serializer
            .DeserializeItems<TestContract>(ms, prefixStyle, TestContract.ProtoReader)
            .ToList();
#endif
        await Assert.That(cloned1).IsNotNull();
        await Assert.That(cloned.Count).IsEqualTo(2);
    }

    [Test]
    public async Task DeserializeItemsTest2()
    {
        PrefixStyle prefixStyle = PrefixStyle.None;
        var ms = new MemoryStream();
#if NET6_0_OR_GREATER
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            prefixStyle,
            TestContract.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
        var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();
#else
        var cloned = Serializer
            .DeserializeItems<TestContract>(ms, prefixStyle, TestContract.ProtoReader)
            .ToList();
#endif
        await Assert.That(cloned.Count).IsEqualTo(0);
    }

    [Test]
    public async Task DeserializeItemsTest3()
    {
        PrefixStyle prefixStyle = (PrefixStyle)4; // Undefined
        var ex1 = Assert.Throws(() =>
        {
            var ms = new MemoryStream();
#if NET6_0_OR_GREATER
            Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), prefixStyle);
#else
            Serializer.SerializeWithLengthPrefix(
                ms,
                CreateTestContract(),
                prefixStyle,
                TestContract.ProtoWriter
            );
#endif
        });
        await Assert.That(ex1).IsTypeOf<ArgumentOutOfRangeException>();
        var ex2 = Assert.Throws(() =>
        {
            var ms = new MemoryStream();
#if NET6_0_OR_GREATER
            var cloned = Serializer.DeserializeItems<TestContract>(ms, prefixStyle).ToList();
#else
            var cloned = Serializer
                .DeserializeItems<TestContract>(ms, prefixStyle, TestContract.ProtoReader)
                .ToList();
#endif
        });
        await Assert.That(ex2).IsTypeOf<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task DeserializeLengthPrefix2()
    {
        var ms = new MemoryStream();
#if NET6_0_OR_GREATER
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
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            fieldNumber: 2,
            TestContract.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            fieldNumber: 2,
            TestContract.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
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
#else
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            fieldNumber: 2,
            TestContract.ProtoReader
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            fieldNumber: 3,
            TestContract.ProtoReader
        );
#endif
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix3()
    {
        var ms = new MemoryStream();
#if NET6_0_OR_GREATER
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.Base128);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.Base128);
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            TestContract.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.Base128,
            TestContract.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.Base128);
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.Base128);
#else
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            TestContract.ProtoReader
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.Base128,
            TestContract.ProtoReader
        );
#endif
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNotNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix_None()
    {
        var ms = new MemoryStream();
#if NET6_0_OR_GREATER
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.None);
        Serializer.SerializeWithLengthPrefix(ms, CreateTestContract(), PrefixStyle.None);
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.None,
            TestContract.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            CreateTestContract(),
            PrefixStyle.None,
            TestContract.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.None);
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(ms, PrefixStyle.None);
#else
        var cloned = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.None,
            TestContract.ProtoReader
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<TestContract>(
            ms,
            PrefixStyle.None,
            TestContract.ProtoReader
        );
#endif
        await Assert.That(cloned).IsNotNull();
        await Assert.That(cloned2).IsNotNull();
    }

    [Test]
    public async Task DeserializeLengthPrefix_NotMessageType()
    {
        var ms = new MemoryStream();
        int number = 10;
#if NET6_0_OR_GREATER
        Serializer.SerializeWithLengthPrefix(
            ms,
            number,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            number,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoWriter
        );
#else
        Serializer.SerializeWithLengthPrefix(
            ms,
            number,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoWriter
        );
        Serializer.SerializeWithLengthPrefix(
            ms,
            number,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoWriter
        );
#endif
        ms.Position = 0;
#if NET6_0_OR_GREATER
        var cloned = Serializer.DeserializeWithLengthPrefix<int>(
            ms,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoReader
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<int>(
            ms,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoReader
        );
#else
        var cloned = Serializer.DeserializeWithLengthPrefix<int>(
            ms,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoReader
        );
        var cloned2 = Serializer.DeserializeWithLengthPrefix<int>(
            ms,
            PrefixStyle.Base128,
            Int32ProtoParser.ProtoReader
        );
#endif
        await Assert.That(cloned).IsEqualTo(number);
        await Assert.That(cloned2).IsEqualTo(number);
    }
}
