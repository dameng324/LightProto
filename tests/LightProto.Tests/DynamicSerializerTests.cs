using System.Buffers;
using System.IO.Compression;
using System.Net;
using LightProto.Parser;

namespace LightProto.Tests;

public partial class DynamicSerializerTests
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
    public async Task TestEnum()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        var obj = HttpStatusCode.OK;
        Serializer.SerializeDynamically(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = Serializer.DeserializeDynamically<HttpStatusCode>(data);
        await Assert.That(parsed).IsEquivalentTo(obj);
    }

    [Test]
    [SkipAot]
    public async Task TestNullable()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        int? obj = 10;
        Serializer.SerializeDynamically(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = Serializer.DeserializeDynamically<int?>(data);
        await Assert.That(parsed).IsEquivalentTo(obj);
    }

    [Test]
    [SkipAot]
    public async Task TestLazy()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Lazy<int> obj = new Lazy<int>(() => 10);
        Serializer.SerializeDynamically(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = Serializer.DeserializeDynamically<Lazy<int>>(data);
        await Assert.That(parsed.Value).IsEquivalentTo(obj.Value);
    }

    [Test]
    public async Task TestBufferWriter()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        var obj = CreateTestContract();
        Serializer.SerializeDynamically(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = Serializer.DeserializeDynamically<TestContract>(data);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestStream()
    {
        using var ms = new MemoryStream();
        var obj = CreateTestContract();

        Serializer.SerializeDynamically(ms, obj);

        using var ms2 = new MemoryStream(ms.ToArray());
        var parsed = Serializer.DeserializeDynamically<TestContract>(ms2);
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
    public async Task TestReadOnlySequence2()
    {
        var obj = CreateTestContract();
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, obj);
        var bytes = ms.ToArray();
        byte[][] bytesArray = [bytes];
        var sequence = GetReadonlySequence(bytesArray);

        var parsed = Serializer.DeserializeDynamically<TestContract>(sequence);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    [SkipAot]
    public async Task Int32CollectionTest()
    {
        using var ms = new MemoryStream();
        var original = new List<int>() { 1, 2, 3 };
        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;
        var parsed = Serializer.DeserializeDynamically<List<int>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task MessageCollectionTest()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;
        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest3()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.SerializeDynamically(ms, original);
        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(ms.ToArray());
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task DeepCloneTest()
    {
        var original = CreateTestContract();
        var parsed = Serializer.DeepCloneDynamically(original);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task SmallDeepCloneTest()
    {
        var original = new TestContract()
        {
            Name = Guid.NewGuid().ToString(),
            Bytes = Enumerable.Range(0, 10).Select(i => (byte)(i % 256)).ToArray(),
            Doubles = Enumerable.Range(0, 10).Select(i => (double)(i % 256)).ToArray(),
            Floats = Enumerable.Range(0, 10).Select(i => (float)(i % 256)).ToArray(),
        };

        var parsed = Serializer.DeepCloneDynamically(original);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest4()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        Serializer.SerializeDynamically(bufferWriter, original);
        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray())
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest5()
    {
        string[] original = ["", "123"];
        var bytes = original.ToByteArray(StringProtoParser.ProtoWriter.GetCollectionWriter());
        var parsed = Serializer.DeserializeDynamically<List<string>>(bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest6()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        string[] original = ["", "123"];
        Serializer.SerializeDynamically(bufferWriter, original);

        var parsed = Serializer.DeserializeDynamically<string[]>(bufferWriter.WrittenMemory.Span);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest8()
    {
        using var ms = new MemoryStream();
        string[] original = ["", "123"];

        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<List<string>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest81()
    {
        using var ms = new MemoryStream();
        HttpStatusCode[] original =
        [
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError,
        ];

        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<List<HttpStatusCode>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest82()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeDynamically(ms, TestContext.Current);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            Serializer.DeserializeDynamically<TestContext>(new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest83()
    {
        using var ms = new MemoryStream();
        List<HttpStatusCode> original =
        [
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError,
        ];

        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<IEnumerable<HttpStatusCode>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest84()
    {
        using var ms = new MemoryStream();
        ICollection<HttpStatusCode> original =
        [
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError,
        ];

        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<ICollection<HttpStatusCode>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest85()
    {
        using var ms = new MemoryStream();
        ValueTuple<HttpStatusCode> original = new ValueTuple<HttpStatusCode>(
            HttpStatusCode.Accepted
        );

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeDynamically(ms, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            Serializer.DeserializeDynamically<ValueTuple<HttpStatusCode>>(new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest86()
    {
        using var ms = new MemoryStream();
        ValueTuple<int, HttpStatusCode> original = new ValueTuple<int, HttpStatusCode>(
            123,
            HttpStatusCode.Accepted
        );
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeDynamically(ms, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            Serializer.DeserializeDynamically<ValueTuple<int, HttpStatusCode>>(new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest87()
    {
        using var ms = new MemoryStream();
        ValueTuple<int, string, HttpStatusCode> original = new ValueTuple<
            int,
            string,
            HttpStatusCode
        >(123, "hello", HttpStatusCode.Accepted);
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeDynamically(ms, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            Serializer.DeserializeDynamically<ValueTuple<int, string, HttpStatusCode>>(
                new MemoryStream()
            );
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest7()
    {
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        using var ms = new MemoryStream();

        Serializer.SerializeDynamically(ms, original);

        var bytes = ms.ToArray();
        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest9()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest11()
    {
        TestContract[] original = null!;
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, original);
        var bytes = ms.ToArray();
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest12()
    {
        byte[] bytes = [];
        var parsed = Serializer.DeserializeDynamically<List<TestContract>>(bytes);
        await Assert.That(parsed.Count).IsEqualTo(0);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        Serializer.SerializeDynamically(bufferWriter, original);
        var parsed = Serializer.DeserializeDynamically<Dictionary<int, TestContract>>(
            GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray())
        );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest2()
    {
        using var ms = new MemoryStream();
        Dictionary<int, TestContract> original = new()
        {
            [1] = CreateTestContract(),
            [2] = CreateTestContract(),
        };
        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;
        var parsed = Serializer.DeserializeDynamically<Dictionary<int, TestContract>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest3()
    {
        using var ms = new MemoryStream();
        Dictionary<List<int>, TestContract> original = new()
        {
            [[1, 3]] = CreateTestContract(),
            [[2, 3]] = CreateTestContract(),
        };
        Serializer.SerializeDynamically(ms, original);
        ms.Position = 0;

        var parsed = Serializer.DeserializeDynamically<Dictionary<List<int>, TestContract>>(ms);
        await Assert.That(parsed.Count).IsEquivalentTo(original.Count);
        foreach (var kv in original)
        {
            await Assert
                .That(parsed.FirstOrDefault(o => o.Key.SequenceEqual(kv.Key)).Value)
                .IsEquivalentTo(kv.Value);
        }
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest4()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, map);
        var bytes = ms.ToArray();
        var clone = Serializer.DeserializeDynamically<Dictionary<string, int>>(bytes.AsSpan());
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest5()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, map);
        var bytes = ms.ToArray();
        var clone = Serializer.DeserializeDynamically<IEnumerable<KeyValuePair<string, int>>>(
            bytes.AsSpan()
        );
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest6()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeDynamically<IReadOnlyDictionary<string, int>>(ms, map);
        var bytes = ms.ToArray();
        var clone = Serializer.DeserializeDynamically<IReadOnlyDictionary<string, int>>(
            bytes.AsSpan()
        );
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    public async Task Test_TriggersLargeSizeSlowPath()
    {
        var original = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            Serializer.SerializeDynamically(gzip, original);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.DeserializeDynamically<byte[]>(deZip);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReaderTest()
    {
        var original = Enumerable.Range(0, 100).ToList();

        using var ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, original);

        ms.Position = 0;
        var parsed = Serializer.DeserializeDynamically<List<int>>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReader2Test()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        Serializer.SerializeDynamically(ms, original);

        ms.Position = 0;
        var parsed = Serializer.DeserializeDynamically<int[]>(ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReaderWithGzipTest()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            Serializer.SerializeDynamically(gzip, original);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.DeserializeDynamically<int[]>(deZip);
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
