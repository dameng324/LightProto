using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
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
    private Random random = new Random(31);

    private CsTestMessage NewCsMessage() =>
        new CsTestMessage
        {
            RequiredIntField = 10,
            StringField = "hello",
            Int32Field = 20,
            Int32ArrayFields = new List<int>() { 0, 13123 },
            StringArrayFields = { Guid.NewGuid().ToString() },
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
            NestedMessageField = new CsTestMessage() { RequiredIntField = 20, StringField = "hello" },
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
#if NET6_0_OR_GREATER
            DateOnlyField = DateOnly.FromDateTime(DateTime.Now.Date),
            TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
#else
            DateOnlyField = (int)((ulong)DateTime.Now.Date.Ticks / 864000000000UL),
            TimeOnlyField = DateTime.Now.TimeOfDay.Ticks,
#endif
            GuidField = Guid.NewGuid(),
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
    [Repeat(100)]
    public async Task GoogleProtobuf_Should_Compatible()
    {
        await RunGoogleProtobuf<CsTestMessage, TestMessage>(NewCsMessage(), TestMessage.Parser.ParseFrom, t2 => t2.ToByteArray());
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic, ImplicitFirstTag = 10)]
    [DataContract]
    public class TestProtobufContract
    {
        public Guid guid3 { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level300)]
        public Guid guid { get; set; }
        internal Guid guid2 { get; set; }
    }

    // [ProtoContract(SkipConstructor = true)]
    // [ProtoInclude(3, typeof(Base2))]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3, typeof(Base2))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    // [ProtoContract(SkipConstructor = true)]
    // [ProtoInclude(3, typeof(Base3))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(3, typeof(Base3))]
    public partial record Base2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue2 { get; set; } = "";
    }

    //[ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract()]
    public partial record Base3 : Base2
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
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
        Console.WriteLine(ProtoBuf.Serializer.GetProto<Base3>());
        //Console.WriteLine( ProtoBuf.Serializer.GetProto<InheritanceTests.Container>());
    }

    async Task<T2> RunGoogleProtobuf<T1, T2>(T1 origin, Func<byte[], T2> parserFunc, Func<T2, byte[]> t2ToByteArray)
        where T1 : IProtoParser<T1>
    {
        var bytes = origin.ToByteArray(ProtoParser<T1>.ProtoWriter);
        var parsed = parserFunc(bytes);

        var originalBytes = Convert.ToBase64String(origin.ToByteArray(ProtoParser<T1>.ProtoWriter));
        var t2Array = t2ToByteArray(parsed);
        var parsedBytes = Convert.ToBase64String(t2Array);
        try
        {
            await Assert.That(originalBytes).IsEqualTo(parsedBytes);

#if NET6_0_OR_GREATER
            var calculatedSize = Serializer.CalculateSize(origin);
            var calculatedLongSize = Serializer.CalculateLongSize(origin);
#else
            var calculatedSize = ProtoParser<T1>.ProtoWriter.CalculateSize(origin);
            var calculatedLongSize = ProtoParser<T1>.ProtoWriter.CalculateLongSize(origin);
#endif
            await Assert.That(t2Array.Length).IsEqualTo(calculatedSize);
            await Assert.That((long)t2Array.Length).IsEqualTo(calculatedLongSize);
        }
        catch (Exception)
        {
            Console.WriteLine($"original: {JsonSerializer.Serialize(origin)}");
            Console.WriteLine($"parsed: {JsonSerializer.Serialize(parsed)}");
            throw;
        }

        var parseBack = LightProto.Serializer.Deserialize<T1>(bytes, ProtoParser<T1>.ProtoReader);
        var bytes2 = parseBack.ToByteArray(ProtoParser<T1>.ProtoWriter);

        var parseBackBytes = Convert.ToBase64String(bytes2);
        await Assert.That(parseBackBytes).IsEqualTo(originalBytes);
#if NET6_0_OR_GREATER
        await Assert.That(Serializer.CalculateSize(parseBack)).IsEqualTo(Serializer.CalculateSize(origin));
#else
        await Assert
            .That(ProtoParser<T1>.ProtoWriter.CalculateSize(parseBack))
            .IsEqualTo(ProtoParser<T1>.ProtoWriter.CalculateSize(origin));

#endif
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
#if NET6_0_OR_GREATER
        var bytes = obj.ToByteArray();
        var parsed = LightProto.Serializer.Deserialize<CsTestMessage>(bytes);
#else
        var bytes = obj.ToByteArray(ProtoParser<CsTestMessage>.ProtoWriter);
        var parsed = LightProto.Serializer.Deserialize<CsTestMessage>(bytes, ProtoParser<CsTestMessage>.ProtoReader);
#endif
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
