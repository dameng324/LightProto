using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using Google.Protobuf;
using LightProto;
using LightProto.Tests;
using LightProto.Tests.Parsers;
using TestPackage;

// using Google.Protobuf.WellKnownTypes;

namespace LightProto.Tests;

public class IntergrationTests
{
    private Random random = Random.Shared; // new Random(31);

    private CsTestMessage NewCsMessage() =>
        new CsTestMessage
        {
            RequiredIntField = 10,
            StringField = "hello",
            Int32Field = 20,
            Int32ArrayField = new List<int>() { 0, 13123 },
            StringArrayField = new List<string>() { string.Empty, Guid.NewGuid().ToString() },
            BytesField = Enumerable.Range(0, random.Next(100)).Select(_ => (byte)20).ToArray(),
            BoolField = random.Next() % 2 == 0,
            DoubleField = random.NextDouble(),
            FloatField = (float)random.NextDouble(),
            Int64Field = random.Next(),
            UInt32Field = (uint)random.Next(),
            UInt64Field = (ulong)random.Next(),
            SInt32Field = random.Next(),
            SInt64Field = random.Next(),
            Fixed32Field = (uint)random.Next(),
            Fixed64Field = (ulong)random.Next(),
            SFixed32Field = random.Next(),
            SFixed64Field = random.Next(),
            EnumField = CsTestEnum.OptionB,
            EnumArrayField = [CsTestEnum.OptionB, CsTestEnum.None, CsTestEnum.OptionA],
            NestedField = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            NestedMessageArrayField =
            [
                new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            ],
            TimestampField2 = DateTime.UtcNow,
            DurationField2 = DateTime.UtcNow.TimeOfDay,
            MapField2 = new() { ["hello"] = "hello", ["hello1"] = "hello" },
            IntArrayFieldTest = [20, 20, 20, 20],
            StringListFieldTest = new List<string>() { "hello", "hello", "hello", "hello" },
            StringArrayFieldTest = ["hello", "hello"],
            IntListFieldTest = [20, 20, 20],
            StringSetFieldTest = ["hello", "hello2"],
            StringQueueFieldTest = new(["hello", "hello"]),
            StringStackFieldTest = new(["hello", "hello"]),
            ConcurrentStringQueueFieldTest = new(["hello", "hello"]),
            ConcurrentStringStackFieldTest = new(["hello", "hello"]),
            NullableIntField = 10,
            IntList = [20, 20],
            StringISet = new HashSet<string>(["hello", "hello"]),
            TimeSpanField = DateTime.Now.TimeOfDay,
            DateOnlyField = DateOnly.FromDateTime(DateTime.Now.Date),
            GuidField = Guid.NewGuid(),
            TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
            StringBuilderField = new StringBuilder("hello"),
            DateTimeField = DateTime.UtcNow,
            MapField4 = new Dictionary<int, long>() { [20] = 20 },
            MapField = new Dictionary<string, CsTestMessage>()
            {
                ["key1"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                ["key2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            },

            MapField5 = new Dictionary<string, string>()
            {
                ["hello"] = "hello",
                ["hello2"] = "hello",
            },
            MapField6 = new ConcurrentDictionary<string, string>()
            {
                ["hello"] = "hello",
                ["hello2"] = "hello",
            },
            MapField7 = new ConcurrentDictionary<string, CsTestMessage>()
            {
                ["hello"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                ["hello2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            },
            NestDictionary = new Dictionary<string, Dictionary<int, string>>()
            {
                ["hello"] = new Dictionary<int, string>() { [10] = "hello", [20] = "hello" },
                ["hello2"] = new Dictionary<int, string>() { [10] = "hello", [20] = "hello" },
            },
        };

    [Test]
    public void GoogleProtobuf_Should_Compatible()
    {
        _ = RunGoogleProtobuf<CsTestMessage, TestMessage>(
            NewCsMessage(),
            TestMessage.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [ProtoBuf.ProtoContract(
        ImplicitFields = ProtoBuf.ImplicitFields.AllPublic,
        ImplicitFirstTag = 10
    )]
    [DataContract]
    public class TestProtobufContract
    {
        public Guid guid3 { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level300)]
        public Guid guid { get; set; }

        internal Guid guid2 { get; set; }
    }

    [Test]
    [Explicit]
    public void GenProto()
    {
        // {
        //     TestArrayMessage message = new();
        //     message.Items.AddRange([1, 2, 3, 4, 5]);
        //     var bytes = message.ToByteArray();
        // }
        // {
        //     var message = new ArrayTests.Message() { Property = [1, 2, 3, 4, 5] };
        //     var ms = new MemoryStream();
        //     ProtoBuf.Serializer.Serialize<ArrayTests.Message>(ms, message);
        //     var bytes = ms.ToArray();
        // }
        //Console.WriteLine( ProtoBuf.Serializer.GetProto<CsTestMessage>());
        Console.WriteLine(ProtoBuf.Serializer.GetProto<TestProtobufContract>());
        //Console.WriteLine( ProtoBuf.Serializer.GetProto<InheritanceTests.Container>());
    }

    async Task RunProtobuf_net<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T1>(
        T1 origin
    )
        where T1 : IProtoMessage<T1>
    {
        var bytes = origin.ToByteArray();
        byte[] byte2;
        var parsed = ProtoBuf.Serializer.Deserialize<T1>(bytes.AsSpan());
        await Assert.That(parsed.ToByteArray()).IsEquivalentTo(bytes);
        {
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, parsed);
                byte2 = ms.ToArray();
            }
            byte[] originalBytes;
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, origin);
                originalBytes = ms.ToArray();
            }
            await Assert.That(originalBytes).IsEquivalentTo(byte2);
        }

        var parseBack = T1.Reader.ParseFrom(byte2);
        var bytes2 = parseBack.ToByteArray();

        await Assert.That(bytes).IsEquivalentTo(bytes2);
    }

    async Task<T2> RunGoogleProtobuf<T1, T2>(
        T1 origin,
        Func<byte[], T2> parserFunc,
        Func<T2, byte[]> t2ToByteArray
    )
        where T1 : IProtoMessage<T1>
    {
        var bytes = origin.ToByteArray();
        var parsed = parserFunc(bytes);

        // Compare the binary serialization instead of JSON for now
        var originalBytes = Convert.ToHexString(origin.ToByteArray());
        var t2Array = t2ToByteArray(parsed);
        var parsedBytes = Convert.ToHexString(t2Array);
        // For now, just check that parsing doesn't throw and produces a result
        await Assert.That(originalBytes).IsEqualTo(parsedBytes);
        await Assert.That(t2Array.Length).IsEqualTo(origin.CalculateSize());

        var parseBack = T1.Reader.ParseFrom(bytes);
        var bytes2 = parseBack.ToByteArray();

        var parseBackBytes = Convert.ToHexString(bytes2);
        await Assert.That(parseBackBytes).IsEqualTo(originalBytes);
        await Assert.That(parseBack.CalculateSize()).IsEqualTo(origin.CalculateSize());

        return parsed;
    }

    [Test]
    public async Task Collection_ShouldNotBeNull_WhenDefaultSizeIsSet_WhenDeserializing()
    {
        var obj = new CsTestMessage { };
        await Assert.That(obj.Int32ArrayField).IsNotNull();
        await Assert.That(obj.StringArrayField).IsNotNull();
        await Assert.That(obj.IntListFieldTest).IsNotNull();
        await Assert.That(obj.StringListFieldTest).IsNotNull();
        await Assert.That(obj.IntArrayFieldTest).IsNotNull();
        await Assert.That(obj.StringSetFieldTest).IsNotNull();
        await Assert.That(obj.StringQueueFieldTest).IsNotNull();
        await Assert.That(obj.StringStackFieldTest).IsNotNull();
        await Assert.That(obj.ConcurrentStringQueueFieldTest).IsNotNull();
        await Assert.That(obj.ConcurrentStringStackFieldTest).IsNotNull();
        var bytes = obj.ToByteArray();
        var parsed = CsTestMessage.Reader.ParseFrom(bytes);
        await Assert.That(parsed.Int32ArrayField).IsNotNull();
        await Assert.That(parsed.StringArrayField).IsNotNull();
        await Assert.That(parsed.IntListFieldTest).IsNotNull();
        await Assert.That(parsed.StringListFieldTest).IsNotNull();
        await Assert.That(parsed.IntArrayFieldTest).IsNotNull();
        await Assert.That(parsed.StringSetFieldTest).IsNotNull();
        await Assert.That(parsed.StringQueueFieldTest).IsNotNull();
        await Assert.That(parsed.StringStackFieldTest).IsNotNull();
        await Assert.That(parsed.ConcurrentStringQueueFieldTest).IsNotNull();
        await Assert.That(parsed.ConcurrentStringStackFieldTest).IsNotNull();
    }
}
