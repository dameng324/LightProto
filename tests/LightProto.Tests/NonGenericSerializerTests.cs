using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using System.Text;
using LightProto.Parser;

namespace LightProto.Tests;

[SuppressMessage(
    "AOT",
    "IL3050:Calling members annotated with \'RequiresDynamicCodeAttribute\' may break functionality when AOT compiling."
)]
[SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code"
)]
public partial class NonGenericSerializerTests
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
        Serializer.SerializeNonGeneric(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = (HttpStatusCode)Serializer.DeserializeNonGeneric(typeof(HttpStatusCode), data);
        await Assert.That(parsed).IsEquivalentTo(obj);
    }

    [Test]
    [SkipAot]
    public async Task TestNullable()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        int? obj = 10;
        Serializer.SerializeNonGeneric(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = (int?)Serializer.DeserializeNonGeneric(typeof(int?), data);
        await Assert.That(parsed).IsEquivalentTo(obj);
    }

    [Test]
    [SkipAot]
    public async Task TestLazy()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Lazy<int> obj = new Lazy<int>(() => 10);
        Serializer.SerializeNonGeneric(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = (Lazy<int>)Serializer.DeserializeNonGeneric(typeof(Lazy<int>), data);
        await Assert.That(parsed.Value).IsEquivalentTo(obj.Value);
    }

    [Test]
    public async Task TestBufferWriter()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        var obj = CreateTestContract();
        Serializer.SerializeNonGeneric(bufferWriter, obj);
        var data = bufferWriter.WrittenMemory.ToArray();
        var parsed = (TestContract)Serializer.DeserializeNonGeneric(typeof(TestContract), data);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    public async Task TestStream()
    {
        using var ms = new MemoryStream();
        var obj = CreateTestContract();

        Serializer.SerializeNonGeneric(ms, obj);

        using var ms2 = new MemoryStream(ms.ToArray());
        var parsed = (TestContract)Serializer.DeserializeNonGeneric(typeof(TestContract), ms2);
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
            var segment = new BufferSegment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = segment;
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
    public async Task TestReadOnlySequence2()
    {
        var obj = CreateTestContract();
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, obj);
        var bytes = ms.ToArray();
        byte[][] bytesArray = [bytes];
        var sequence = GetReadonlySequence(bytesArray);

        var parsed = (TestContract)Serializer.DeserializeNonGeneric(typeof(TestContract), sequence);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }

    [Test]
    [SkipAot]
    public async Task Int32CollectionTest()
    {
        using var ms = new MemoryStream();
        var original = new List<int>() { 1, 2, 3 };
        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;
        var parsed = (List<int>)Serializer.DeserializeNonGeneric(typeof(List<int>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task MessageCollectionTest()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;
        var parsed = (List<TestContract>)Serializer.DeserializeNonGeneric(typeof(List<TestContract>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest3()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        Serializer.SerializeNonGeneric(ms, original);
        var parsed = (List<TestContract>)Serializer.DeserializeNonGeneric(typeof(List<TestContract>), ms.ToArray());
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
        Serializer.SerializeNonGeneric(bufferWriter, original);
        var parsed =
            (List<TestContract>)
                Serializer.DeserializeNonGeneric(
                    typeof(List<TestContract>),
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
        var parsed = (List<string>)Serializer.DeserializeNonGeneric(typeof(List<string>), bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest6()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        string[] original = ["", "123"];
        Serializer.SerializeNonGeneric(bufferWriter, original);

        var parsed = (string[])Serializer.DeserializeNonGeneric(typeof(string[]), bufferWriter.WrittenMemory.Span);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest8()
    {
        using var ms = new MemoryStream();
        string[] original = ["", "123"];

        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (List<string>)Serializer.DeserializeNonGeneric(typeof(List<string>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest81()
    {
        using var ms = new MemoryStream();
        HttpStatusCode[] original = [HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError];

        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (List<HttpStatusCode>)Serializer.DeserializeNonGeneric(typeof(List<HttpStatusCode>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest82()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeNonGeneric(ms, TestContext.Current!);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = (TestContext)Serializer.DeserializeNonGeneric(typeof(TestContext), new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest83()
    {
        using var ms = new MemoryStream();
        List<HttpStatusCode> original = [HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError];

        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (IEnumerable<HttpStatusCode>)Serializer.DeserializeNonGeneric(typeof(IEnumerable<HttpStatusCode>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest84()
    {
        using var ms = new MemoryStream();
        ICollection<HttpStatusCode> original = [HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError];

        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (ICollection<HttpStatusCode>)Serializer.DeserializeNonGeneric(typeof(ICollection<HttpStatusCode>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest85()
    {
        using var ms = new MemoryStream();
        ValueTuple<HttpStatusCode> original = new ValueTuple<HttpStatusCode>(HttpStatusCode.Accepted);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var ms2 = new MemoryStream();
            Serializer.SerializeNonGeneric(ms2, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = (ValueTuple<HttpStatusCode>)Serializer.DeserializeNonGeneric(typeof(ValueTuple<HttpStatusCode>), new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest86()
    {
        using var ms = new MemoryStream();
        ValueTuple<int, HttpStatusCode> original = new ValueTuple<int, HttpStatusCode>(123, HttpStatusCode.Accepted);
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms2 = new MemoryStream();
            Serializer.SerializeNonGeneric(ms2, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            _ =
                (ValueTuple<int, HttpStatusCode>)
                    Serializer.DeserializeNonGeneric(typeof(ValueTuple<int, HttpStatusCode>), new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest87()
    {
        using var ms = new MemoryStream();
        ValueTuple<int, string, HttpStatusCode> original = new ValueTuple<int, string, HttpStatusCode>(
            123,
            "hello",
            HttpStatusCode.Accepted
        );
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var ms2 = new MemoryStream();
            Serializer.SerializeNonGeneric(ms2, original);
        });
        await Assert.That(ex.Message).Contains("No ProtoParser registered for type");

        var ex2 = Assert.Throws<InvalidOperationException>(() =>
        {
            _ =
                (ValueTuple<int, string, HttpStatusCode>)
                    Serializer.DeserializeNonGeneric(typeof(ValueTuple<int, string, HttpStatusCode>), new MemoryStream());
        });
        await Assert.That(ex2.Message).Contains("No ProtoParser registered for type");
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest7()
    {
        TestContract[] original = [CreateTestContract(), CreateTestContract()];

        using var ms = new MemoryStream();

        Serializer.SerializeNonGeneric(ms, original);

        var bytes = ms.ToArray();
        var parsed = (List<TestContract>)Serializer.DeserializeNonGeneric(typeof(List<TestContract>), bytes);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest9()
    {
        using var ms = new MemoryStream();
        TestContract[] original = [CreateTestContract(), CreateTestContract()];
        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (List<TestContract>)Serializer.DeserializeNonGeneric(typeof(List<TestContract>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest11()
    {
        TestContract[] original = null!;
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, original);
        var bytes = ms.ToArray();
        await Assert.That(bytes.Length).IsEqualTo(0);

        ArrayBufferWriter<byte> bufferWriter = new();
        Serializer.SerializeNonGeneric(bufferWriter, original);
        await Assert.That(bufferWriter.WrittenCount).IsEqualTo(0);

        bytes = Serializer.SerializeToArrayNonGeneric(original);
        await Assert.That(bytes.Length).IsEqualTo(0);
    }

    [Test]
    [SkipAot]
    public async Task CollectionTest12()
    {
        byte[] bytes = [];
        var parsed = (List<TestContract>)Serializer.DeserializeNonGeneric(typeof(List<TestContract>), bytes);
        await Assert.That(parsed.Count).IsEqualTo(0);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        Dictionary<int, TestContract> original = new() { [1] = CreateTestContract(), [2] = CreateTestContract() };
        Serializer.SerializeNonGeneric(bufferWriter, original);
        var parsed =
            (Dictionary<int, TestContract>)
                Serializer.DeserializeNonGeneric(
                    typeof(Dictionary<int, TestContract>),
                    GetReadonlySequence(bufferWriter.WrittenMemory.ToArray().Chunk(2).ToArray())
                );
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest2()
    {
        using var ms = new MemoryStream();
        Dictionary<int, TestContract> original = new() { [1] = CreateTestContract(), [2] = CreateTestContract() };
        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;
        var parsed = (Dictionary<int, TestContract>)Serializer.DeserializeNonGeneric(typeof(Dictionary<int, TestContract>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest3()
    {
        using var ms = new MemoryStream();
        Dictionary<List<int>, TestContract> original = new() { [[1, 3]] = CreateTestContract(), [[2, 3]] = CreateTestContract() };
        Serializer.SerializeNonGeneric(ms, original);
        ms.Position = 0;

        var parsed = (Dictionary<List<int>, TestContract>)Serializer.DeserializeNonGeneric(typeof(Dictionary<List<int>, TestContract>), ms);
        await Assert.That(parsed.Count).IsEquivalentTo(original.Count);
        foreach (var kv in original)
        {
            await Assert.That(parsed.FirstOrDefault(o => o.Key.SequenceEqual(kv.Key)).Value).IsEquivalentTo(kv.Value);
        }
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest4()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, map);
        var bytes = ms.ToArray();
        var clone = (Dictionary<string, int>)Serializer.DeserializeNonGeneric(typeof(Dictionary<string, int>), bytes.AsSpan());
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest5()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, map);
        var bytes = ms.ToArray();
        var clone =
            (IEnumerable<KeyValuePair<string, int>>)
                Serializer.DeserializeNonGeneric(typeof(IEnumerable<KeyValuePair<string, int>>), bytes.AsSpan());
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    [SkipAot]
    public async Task DictionaryTest6()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        MemoryStream ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, map);
        var bytes = ms.ToArray();
        var clone =
            (IReadOnlyDictionary<string, int>)Serializer.DeserializeNonGeneric(typeof(IReadOnlyDictionary<string, int>), bytes.AsSpan());
        await Assert.That(clone).IsEquivalentTo(map);
    }

    [Test]
    public async Task Test_TriggersLargeSizeSlowPath()
    {
        var original = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            Serializer.SerializeNonGeneric(gzip, original);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = (byte[])Serializer.DeserializeNonGeneric(typeof(byte[]), deZip);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReaderTest()
    {
        var original = Enumerable.Range(0, 100).ToList();

        using var ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, original);

        ms.Position = 0;
        var parsed = (List<int>)Serializer.DeserializeNonGeneric(typeof(List<int>), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReader2Test()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        Serializer.SerializeNonGeneric(ms, original);

        ms.Position = 0;
        var parsed = (int[])Serializer.DeserializeNonGeneric(typeof(int[]), ms);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    [SkipAot]
    public async Task GetArrayReaderWithGzipTest()
    {
        var original = Enumerable.Range(0, 100).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            Serializer.SerializeNonGeneric(gzip, original);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = (int[])Serializer.DeserializeNonGeneric(typeof(int[]), deZip);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    public static IEnumerable<Func<object>> GetAotUnsupportedValues()
    {
        yield return () => new List<byte>() { 1, 2, 3, 4, 5 };
        yield return () => new List<sbyte>() { -1, 2, -3, 4, -5 };
        yield return () => new Lazy<int>(() => 123);
        yield return () => new List<int>() { 1, 2, 3 };
        yield return () => new Queue<int>([1, 2, 3]);
        yield return () => new Stack<int>([1, 2, 3]);
        yield return () => new LinkedList<int>([1, 2, 3]);
        yield return () => new Collection<int>() { 1, 2, 3 };
        yield return () => new ReadOnlyCollection<int>([1, 2, 3]);
        yield return () => new ObservableCollection<int>() { 1, 2, 3 };
        yield return () => new ReadOnlyObservableCollection<int>([1, 2, 3]);
        yield return () => new BlockingCollection<int>() { 1, 2, 3 };
        yield return () => new ConcurrentBag<int>() { 1, 2, 3 };
        yield return () => new ConcurrentQueue<int>([1, 2, 3]);
        yield return () => new ConcurrentStack<int>([1, 2, 3]);
        yield return () => new HashSet<int>([1, 2, 3]);
        yield return () => new SortedSet<int>([1, 2, 3]);
        yield return () => ImmutableArray.Create(1, 2, 3);
        yield return () => ImmutableHashSet.Create(1, 2, 3);
        yield return () => ImmutableList.Create(1, 2, 3);
        yield return () => new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        yield return () => new ReadOnlyDictionary<string, int>(new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 });
        yield return () => new ConcurrentDictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        yield return () => ImmutableDictionary.CreateRange(new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 });
        yield return () => new SortedList<string, int>() { ["a"] = 1, ["b"] = 2 };
        yield return () => new SortedDictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        yield return () => new KeyValuePair<string, int>("key", 123);
    }

    [Test]
    [MethodDataSource(nameof(GetAotUnsupportedValues))]
    [SkipAot]
    public async Task LightProto_SerializeDeserialize_NonGeneric_NoAOT(object value) =>
        await LightProto_SerializeDeserialize_NonGeneric_AOT(value);

    public static IEnumerable<Func<object>> GetAotSupportedValues()
    {
        yield return () => 123;
        yield return () => -123;
        yield return () => true;
        yield return () => false;
        yield return () => (long)123;
        yield return () => (byte)123;
        yield return () => (sbyte)123;
        yield return () => (sbyte)-123;
        yield return () => (long)-123;
        yield return () => (uint)123;
        yield return () => (ulong)123;
        yield return () => (float)123;
        yield return () => (double)123;
        yield return () => (decimal)123;
        yield return () => (BigInteger)123;
        yield return () => HttpStatusCode.Accepted;
        yield return () => Guid.NewGuid();
        yield return () => 123;
        yield return () => 123;
        yield return () => "Hello, World!";
        yield return () => 'H';
        yield return () => new StringBuilder("Hello, StringBuilder!");
        yield return () => new TestContract() { Name = "Test" };
        yield return () => new byte[] { 1, 2, 3, 4, 5 };
        yield return () => new BitArray([1, 2, 3, 4, 5]);
        yield return () => new Complex(1, 2);
        yield return () => new CultureInfo("en-US");
        yield return () => DateTime.Now;
        yield return () => DateTime.Now.TimeOfDay;
        yield return () => DateTimeOffset.Now;
        yield return () => TimeZoneInfo.Utc;
        yield return () => new Uri("https://www.example.com/");
        yield return () => new Version(1, 2, 3, 4);
#if NET7_0_OR_GREATER
        yield return () => (Int128)123;
        yield return () => (Int128)(-123);
        yield return () => (UInt128)123;
        yield return () => new DateOnly(2022, 1, 1);
        yield return () => new TimeOnly(12, 0, 0);
        yield return () => (Half)123;
        yield return () => new Matrix3x2(1, 2, 3, 4, 5, 6);
        yield return () => new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
        yield return () => new Plane(1, 2, 3, 4);
        yield return () => new Quaternion(1, 2, 3, 4);
        yield return () => new Vector2(1, 2);
        yield return () => new Vector3(1, 2, 3);
        yield return () => new Rune('A');
#endif
    }

    [Test]
    [MethodDataSource(nameof(GetAotSupportedValues))]
    public async Task LightProto_SerializeDeserialize_NonGeneric_AOT(object value)
    {
        var type = value.GetType();
        var bytes1 = SerializeToArray();
        var bytes2 = SerializeToIBufferWriter();
        var bytes3 = SerializeToStream();
        // Assert serialized bytes are equal
        await Assert.That(bytes2).IsEquivalentTo(bytes1);
        await Assert.That(bytes3).IsEquivalentTo(bytes1);

        // Deserialize
        var deserialized = DeserializedFromSequence(bytes1);
        await AssertEquivalentTo(type, deserialized, value);

        deserialized = DeserializedFromStream(bytes1);
        await AssertEquivalentTo(type, deserialized, value);

        deserialized = DeserializedFromSpan(bytes1);
        await AssertEquivalentTo(type, deserialized, value);

        var reader = Serializer.GetProtoReader(type);

        static async Task AssertEquivalentTo(Type type, object deserialized, object original)
        {
            if (type == typeof(Lazy<int>))
            {
                var lazyDeserialized = (Lazy<int>)deserialized;
                var lazyOriginal = (Lazy<int>)original;
                await Assert.That(lazyDeserialized.Value).IsEquivalentTo(lazyOriginal.Value);
            }
            else if (type == typeof(ImmutableArray<int>))
            {
                var arrDeserialized = ((ImmutableArray<int>)deserialized).ToArray();
                var arrOriginal = ((ImmutableArray<int>)original).ToArray();
                await Assert.That(arrDeserialized).IsEquivalentTo(arrOriginal);
            }
            else if (type == typeof(ConcurrentBag<int>))
            {
                var arrDeserialized = ((ConcurrentBag<int>)deserialized).OrderBy(x => x).ToArray();
                var arrOriginal = ((ConcurrentBag<int>)original).OrderBy(x => x).ToArray();
                await Assert.That(arrDeserialized).IsEquivalentTo(arrOriginal);
            }
            else
            {
                await Assert.That(deserialized).IsEquivalentTo(original);
            }
        }

        byte[] SerializeToStream()
        {
            using var ms = new MemoryStream();
            Serializer.SerializeNonGeneric(ms, value);
            return ms.ToArray();
        }
        byte[] SerializeToIBufferWriter()
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            Serializer.SerializeNonGeneric(bufferWriter, value);
            return bufferWriter.WrittenMemory.ToArray();
        }
        byte[] SerializeToArray()
        {
            return Serializer.SerializeToArrayNonGeneric(value);
        }

        object DeserializedFromStream(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return Serializer.DeserializeNonGeneric(type, ms);
        }
        object DeserializedFromSpan(byte[] bytes)
        {
            return Serializer.DeserializeNonGeneric(type, bytes);
        }
        object DeserializedFromSequence(byte[] bytes)
        {
            var sequence = new ReadOnlySequence<byte>(bytes);
            return Serializer.DeserializeNonGeneric(type, sequence);
        }
    }

    public static IEnumerable<Func<(object, IProtoReader, IProtoWriter)>> GetValuesWithReadersWriters()
    {
        yield return () =>
            (
                (int?)456,
                new NullableProtoReader<int>(Int32ProtoParser.ProtoReader),
                new NullableProtoWriter<int>(Int32ProtoParser.ProtoWriter)
            );
        yield return () =>
            (DateTime.Now, (IProtoReader)DateTime240ProtoParser.ProtoReader, (IProtoWriter)DateTime240ProtoParser.ProtoWriter);
        yield return () =>
            (DateTime.Now.TimeOfDay, (IProtoReader)TimeSpan240ProtoParser.ProtoReader, (IProtoWriter)TimeSpan240ProtoParser.ProtoWriter);
        yield return () => ((decimal)123, (IProtoReader)Decimal300ProtoParser.ProtoReader, (IProtoWriter)Decimal300ProtoParser.ProtoWriter);
        yield return () => ((uint)123, (IProtoReader)Fixed32ProtoParser.ProtoReader, (IProtoWriter)Fixed32ProtoParser.ProtoWriter);
        yield return () => ((ulong)123, (IProtoReader)Fixed64ProtoParser.ProtoReader, (IProtoWriter)Fixed64ProtoParser.ProtoWriter);
        yield return () => ((int)123, (IProtoReader)SFixed32ProtoParser.ProtoReader, (IProtoWriter)SFixed32ProtoParser.ProtoWriter);
        yield return () => ((long)123, (IProtoReader)SFixed64ProtoParser.ProtoReader, (IProtoWriter)SFixed64ProtoParser.ProtoWriter);
        yield return () => ((int)123, (IProtoReader)SInt32ProtoParser.ProtoReader, (IProtoWriter)SInt32ProtoParser.ProtoWriter);
        yield return () => ((long)123, (IProtoReader)SInt64ProtoParser.ProtoReader, (IProtoWriter)SInt64ProtoParser.ProtoWriter);
        yield return () => ((byte)123, (IProtoReader)ByteProtoParser.ProtoReader, (IProtoWriter)ByteProtoParser.ProtoWriter);
        yield return () => ((byte)123, (IProtoReader)FixedByteProtoParser.ProtoReader, (IProtoWriter)FixedByteProtoParser.ProtoWriter);
        yield return () => ((sbyte)123, (IProtoReader)SSByteProtoParser.ProtoReader, (IProtoWriter)SSByteProtoParser.ProtoWriter);
        yield return () => ((sbyte)123, (IProtoReader)SFixedByteProtoParser.ProtoReader, (IProtoWriter)SFixedByteProtoParser.ProtoWriter);
        yield return () => ((sbyte)123, (IProtoReader)SByteProtoParser.ProtoReader, (IProtoWriter)SByteProtoParser.ProtoWriter);
        yield return () => ((char)'a', (IProtoReader)CharProtoParser.ProtoReader, (IProtoWriter)CharProtoParser.ProtoWriter);
        yield return () => (Guid.NewGuid(), (IProtoReader)Guid300ProtoParser.ProtoReader, (IProtoWriter)Guid300ProtoParser.ProtoWriter);
        yield return () =>
            ("Hello World!", (IProtoReader)InternedStringProtoParser.ProtoReader, (IProtoWriter)InternedStringProtoParser.ProtoWriter);
        yield return () => ((ushort)123, (IProtoReader)UInt16ProtoParser.ProtoReader, (IProtoWriter)UInt16ProtoParser.ProtoWriter);
        yield return () => ((ushort)123, (IProtoReader)Fixed16ProtoParser.ProtoReader, (IProtoWriter)Fixed16ProtoParser.ProtoWriter);
        yield return () => ((short)123, (IProtoReader)Int16ProtoParser.ProtoReader, (IProtoWriter)Int16ProtoParser.ProtoWriter);
        yield return () => ((short)123, (IProtoReader)SInt16ProtoParser.ProtoReader, (IProtoWriter)SInt16ProtoParser.ProtoWriter);
        yield return () => ((short)123, (IProtoReader)SFixed16ProtoParser.ProtoReader, (IProtoWriter)SFixed16ProtoParser.ProtoWriter);
    }

    [Test]
    [MethodDataSource(nameof(GetValuesWithReadersWriters))]
    public async Task LightProto_SerializeDeserialize_NonGeneric_With_Reader_Writer(object value, IProtoReader reader, IProtoWriter writer)
    {
        var type = value.GetType();
        await LightProto_SerializeDeserialize_NonGeneric_WithType_Reader_Writer(value, type, reader, writer);
    }

    public static IEnumerable<Func<(object?, Type, IProtoReader, IProtoWriter)>> GetValuesWithTypeReadersWriters()
    {
        yield return () =>
            (
                null,
                typeof(int?),
                new NullableProtoReader<int>(Int32ProtoParser.ProtoReader),
                new NullableProtoWriter<int>(Int32ProtoParser.ProtoWriter)
            );
    }

    [Test]
    [MethodDataSource(nameof(GetValuesWithTypeReadersWriters))]
    public async Task LightProto_SerializeDeserialize_NonGeneric_WithType_Reader_Writer(
        object? value,
        Type type,
        IProtoReader reader,
        IProtoWriter writer
    )
    {
        var bytes1 = SerializeToArray();
        var bytes2 = SerializeToIBufferWriter();
        var bytes3 = SerializeToStream();
        // Assert serialized bytes are equal
        await Assert.That(bytes2).IsEquivalentTo(bytes1);
        await Assert.That(bytes3).IsEquivalentTo(bytes1);

        // Deserialize
        var deserialized = DeserializedFromSequence(bytes1);
        await AssertEquivalentTo(type, deserialized, value!);

        deserialized = DeserializedFromStream(bytes1);
        await AssertEquivalentTo(type, deserialized, value!);

        deserialized = DeserializedFromSpan(bytes1);
        await AssertEquivalentTo(type, deserialized, value!);

        static async Task AssertEquivalentTo(Type type, object deserialized, object original)
        {
            if (type == typeof(Lazy<int>))
            {
                var lazyDeserialized = (Lazy<int>)deserialized;
                var lazyOriginal = (Lazy<int>)original;
                await Assert.That(lazyDeserialized.Value).IsEquivalentTo(lazyOriginal.Value);
            }
            else if (type == typeof(ImmutableArray<int>))
            {
                var arrDeserialized = ((ImmutableArray<int>)deserialized).ToArray();
                var arrOriginal = ((ImmutableArray<int>)original).ToArray();
                await Assert.That(arrDeserialized).IsEquivalentTo(arrOriginal);
            }
            else if (type == typeof(ConcurrentBag<int>))
            {
                var arrDeserialized = ((ConcurrentBag<int>)deserialized).OrderBy(x => x).ToArray();
                var arrOriginal = ((ConcurrentBag<int>)original).OrderBy(x => x).ToArray();
                await Assert.That(arrDeserialized).IsEquivalentTo(arrOriginal);
            }
            else
            {
                await Assert.That(deserialized).IsEquivalentTo(original);
            }
        }

        byte[] SerializeToStream()
        {
            using var ms = new MemoryStream();
            Serializer.SerializeNonGeneric(ms, value, writer);
            return ms.ToArray();
        }
        byte[] SerializeToIBufferWriter()
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            Serializer.SerializeNonGeneric(bufferWriter, value, writer);
            return bufferWriter.WrittenMemory.ToArray();
        }
        byte[] SerializeToArray()
        {
            return Serializer.SerializeToArrayNonGeneric(value, writer);
        }

        object DeserializedFromStream(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return Serializer.DeserializeNonGeneric(ms, reader);
        }
        object DeserializedFromSpan(byte[] bytes)
        {
            return Serializer.DeserializeNonGeneric(bytes, reader);
        }
        object DeserializedFromSequence(byte[] bytes)
        {
            var sequence = new ReadOnlySequence<byte>(bytes);
            return Serializer.DeserializeNonGeneric(sequence, reader);
        }
    }
}
