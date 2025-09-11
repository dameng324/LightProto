using System.Collections.Concurrent;
using System.Text;
using Dameng.Protobuf.WellKnownTypes;
using Google.Protobuf;
using TestPackage;
// using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Tests;

public class Tests
{
    private Random random = Random.Shared; // new Random(31);

    [Test]
    public void LocalToGoogle()
    {
        var parsed = Run<CsTestMessage, TestMessage>(
            new CsTestMessage
            {
                RequiredIntField = RandomInt(),
                StringField = RandomString(),
                Int32Field = RandomInt(),
                Int32ArrayField = new List<int>()
                {
                    0,
                    13123
                },
                StringArrayField = new List<string>()
                {
                    string.Empty,
                    Guid.NewGuid()
                        .ToString()
                },
                BytesField = Enumerable
                    .Range(0,
                        random.Next(100))
                    .Select(_ => (byte)RandomInt())
                    .ToList(),
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
                MapField = new Dictionary<string, CsTestMessage>()
                {
                    ["key1"] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                    ["key2"] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                },
                EnumField = CsTestEnum.OptionB,
                EnumArrayField =
                [
                    CsTestEnum.OptionB,
                    CsTestEnum.None,
                    CsTestEnum.OptionA
                ],
                NestedField = new CsTestMessage()
                {
                    RequiredIntField = RandomInt(),
                    StringField = RandomString(),
                },
                NestedMessageArrayField =
                [
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                ],
                TimestampField = DateTime.UtcNow.ToTimestamp(),
                DurationField = DateTime.UtcNow.TimeOfDay.ToDuration(),
                MapField2 = new()
                {
                    [RandomString()] = RandomString(),
                    [RandomString()] = RandomString(),
                },
                MapField4 = new Dictionary<int, long>()
                {
                    [1111] = 2222
                },
                DateTimeField = DateTime.UtcNow,
                IntArrayFieldTest =
                [
                    10,
                    20,
                    30
                ],
                StringListFieldTest =
                [
                    "array",
                    "list",
                    "test"
                ],
                StringArrayFieldTest =
                [
                    "hello",
                    "world"
                ],
                IntListFieldTest =
                [
                    100,
                    200,
                    300
                ],
                NullableIntField = 10,
                MapField5 = new Dictionary<string, string>()
                {
                    [RandomString()] = RandomString(),
                    [RandomString()] = RandomString(),
                },
                MapField6 = new ConcurrentDictionary<string, string>()
                {
                    [RandomString()] = RandomString(),
                    [RandomString()] = RandomString(),
                },
                MapField7 = new ConcurrentDictionary<string, CsTestMessage>()
                {
                    [RandomString()] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                    [RandomString()] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(),
                        StringField = RandomString(),
                    },
                },
                StringSetFieldTest =
                [
                    RandomString(),
                    RandomString()
                ],
                StringQueueFieldTest = new([
                    RandomString(),
                    RandomString()
                ]),
                StringStackFieldTest = new([
                    RandomString(),
                    RandomString()
                ]),
                ConcurrentStringQueueFieldTest = new([
                    RandomString(),
                    RandomString()
                ]),
                ConcurrentStringStackFieldTest = new([
                    RandomString(),
                    RandomString()
                ]),
                IntBag = [RandomInt(), RandomInt()],
                StringISet = new HashSet<string>([
                    RandomString(),
                    RandomString()
                ]),
                TimeSpanField = DateTime.Now.TimeOfDay,
                DateOnlyField = DateOnly.FromDateTime(DateTime.Now.Date),
                GuidField = Guid.NewGuid(),
                TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
                StringBuilderField = new StringBuilder(RandomString()),
            },
            TestMessage.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    string RandomString()
    {
        return random.Next(2) switch
        {
            //0 => string.Empty,
            _ => Guid.NewGuid().ToString(),
        };
    }

    int RandomInt() => random.Next(10);

    async Task<T2> Run<T1, T2>(
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
    public void TestStruct()
    {
        Run<TestStruct, TestType>(
            new TestStruct { Name = RandomString(), Value = RandomInt() },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public void TestRecord()
    {
        Run<TestRecord, TestType>(
            new TestRecord { Name = RandomString(), Value = RandomInt() },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public void TestRecordStruct()
    {
        Run<TestRecordStruct, TestType>(
            new TestRecordStruct { Name = RandomString(), Value = RandomInt() },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public async Task ProxyTest()
    {
        ProtoProxy testObj = new()
        {
            Instrument = Instrument.FromNameValue(RandomString(), RandomInt()),
        };

        var bytes = testObj.ToByteArray();
        var parsed = ProtoProxy.Reader.ParseFrom(bytes);
        await Assert.That(parsed.Instrument.Name).IsEqualTo(testObj.Instrument.Name);
        await Assert.That(parsed.Instrument.Value).IsEqualTo(testObj.Instrument.Value);

        //parsed.GetHashCode()await Assert.That().IsEqualTo(testObj.GetHashCode());
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
