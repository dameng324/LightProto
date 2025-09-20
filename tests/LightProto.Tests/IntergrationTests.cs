using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;
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
            Int32ArrayFields = new List<int>() { 0, 13123 },
            StringArrayFields = { string.Empty, Guid.NewGuid().ToString() },
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
            EnumArrayFields = { CsTestEnum.OptionB, CsTestEnum.None, CsTestEnum.OptionA },
            NestedMessageField = new CsTestMessage()
            {
                RequiredIntField = 20,
                StringField = "hello",
            },
            NestedMessageArrayFields =
            [
                new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            ],
            TimestampField = DateTime.UtcNow,
            DurationField = DateTime.UtcNow.TimeOfDay,
            MapField2s = { ["hello"] = "hello", ["hello1"] = "hello" },
            IntArrayFieldTests = [20, 20, 20, 20],
            StringListFieldTests = { "hello", "hello", "hello", "hello" },
            StringArrayFieldTests = { "hello", "hello" },
            IntListFieldTests = [20, 20, 20],
            StringSetFieldTests = { "hello", "hello2" },
            StringQueueFieldTests = new(["hello", "hello"]),
            StringStackFieldTests = new(["hello", "hello"]),
            ConcurrentStringQueueFieldTests = new(["hello", "hello"]),
            ConcurrentStringStackFieldTests = new(["hello", "hello"]),
            NullableIntField = 10,
            IntLists = [20, 20],
            StringISets = { "hello", "hello2" },
            TimeSpanField = DateTime.Now.TimeOfDay,
            DateOnlyField = DateOnly.FromDateTime(DateTime.Now.Date),
            GuidField = Guid.NewGuid(),
            TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
            StringBuilderField = new StringBuilder("hello"),
            DateTimeField = DateTime.UtcNow,
            MapField4s = { [20] = 20 },
            MapFields =
            {
                ["key1"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                ["key2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            },

            MapField5s = { ["hello"] = "hello", ["hello2"] = "hello" },
            MapField6s = { ["hello"] = "hello", ["hello2"] = "hello" },
            MapField7s =
            {
                ["hello"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
                ["hello2"] = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
            },
        };

    [Test]
    public async Task GoogleProtobuf_Should_Compatible()
    {
        await RunGoogleProtobuf<CsTestMessage, TestMessage>(
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

    async Task<T2> RunGoogleProtobuf<T1, T2>(
        T1 origin,
        Func<byte[], T2> parserFunc,
        Func<T2, byte[]> t2ToByteArray
    )
        where T1 : IProtoParser<T1>
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

        var parseBack = T1.ProtoReader.ParseFrom(bytes);
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
        await Assert.That(obj.Int32ArrayFields).IsNotNull();
        await Assert.That(obj.StringArrayFields).IsNotNull();
        await Assert.That(obj.IntListFieldTests).IsNotNull();
        await Assert.That(obj.StringListFieldTests).IsNotNull();
        await Assert.That(obj.IntArrayFieldTests).IsNotNull();
        await Assert.That(obj.StringSetFieldTests).IsNotNull();
        await Assert.That(obj.StringQueueFieldTests).IsNotNull();
        await Assert.That(obj.StringStackFieldTests).IsNotNull();
        await Assert.That(obj.ConcurrentStringQueueFieldTests).IsNotNull();
        await Assert.That(obj.ConcurrentStringStackFieldTests).IsNotNull();
        var bytes = obj.ToByteArray();
        var parsed = CsTestMessage.ProtoReader.ParseFrom(bytes);
        await Assert.That(parsed.Int32ArrayFields).IsNotNull();
        await Assert.That(parsed.StringArrayFields).IsNotNull();
        await Assert.That(parsed.IntListFieldTests).IsNotNull();
        await Assert.That(parsed.StringListFieldTests).IsNotNull();
        await Assert.That(parsed.IntArrayFieldTests).IsNotNull();
        await Assert.That(parsed.StringSetFieldTests).IsNotNull();
        await Assert.That(parsed.StringQueueFieldTests).IsNotNull();
        await Assert.That(parsed.StringStackFieldTests).IsNotNull();
        await Assert.That(parsed.ConcurrentStringQueueFieldTests).IsNotNull();
        await Assert.That(parsed.ConcurrentStringStackFieldTests).IsNotNull();
    }
}
