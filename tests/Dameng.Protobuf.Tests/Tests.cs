using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Dameng.Protobuf.Tests;
using Dameng.Protobuf.WellKnownTypes;
using Google.Protobuf;
// using Google.Protobuf.WellKnownTypes;
using TestPackage;

namespace Dameng.Protobuf.Extension.Tests;

public class Tests
{
    private Random random = Random.Shared; // new Random(31);

    [Test]
    public void LocalToGoogle()
    {
        var parsed = Run<CsTestMessage, TestMessage>(
            new CsTestMessage
            {
                RequiredIntField = RandomInt(random),
                StringField = RandomString(random),
                Int32Field = RandomInt(random),
                Int32ArrayField = new List<int>() { 0, 13123 },
                StringArrayField = new List<string>() { string.Empty, Guid.NewGuid().ToString() },
                BytesField = Enumerable
                    .Range(0, random.Next(100))
                    .Select(_ => (byte)RandomInt(random))
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
                MapField = new Dictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                EnumField = CsTestEnum.OptionB,
                EnumArrayField = [CsTestEnum.OptionB, CsTestEnum.None, CsTestEnum.OptionA],
                NestedField = new CsTestMessage()
                {
                    RequiredIntField = RandomInt(random),
                    StringField = RandomString(random),
                },
                NestedMessageArrayField =
                [
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random),
                    },
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random),
                    },
                ],
                TimestampField = DateTime.UtcNow.ToTimestamp(),
                DurationField = DateTime.UtcNow.TimeOfDay.ToDuration(),
                MapField2 = new()
                {
                    [RandomString(random)] = RandomString(random),
                    [RandomString(random)] = RandomString(random),
                },
                MapField4 = new Dictionary<int, long>() { [1111] = 2222 },
                DateTimeField = DateTime.UtcNow,
                IntArrayFieldTest = [10, 20, 30],
                StringListFieldTest = ["array", "list", "test"],
                StringArrayFieldTest = ["hello", "world"],
                IntListFieldTest = [100, 200, 300],
                NullableIntField = 10,
                MapField5 = new Dictionary<string, string>()
                {
                    [RandomString(random)] = RandomString(random),
                    [RandomString(random)] = RandomString(random),
                },
                MapField6 = new ConcurrentDictionary<string, string>()
                {
                    [RandomString(random)] = RandomString(random),
                    [RandomString(random)] = RandomString(random),
                },
                MapField7 = new ConcurrentDictionary<string, CsTestMessage>()
                {
                    [RandomString(random)] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random),
                    },
                    [RandomString(random)] = new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random),
                    },
                },
                StringSetFieldTest = [RandomString(random), RandomString(random)],
                StringQueueFieldTest = new([RandomString(random), RandomString(random)]),
                StringStackFieldTest = new([RandomString(random), RandomString(random)]),
                ConcurrentStringQueueFieldTest = new([RandomString(random), RandomString(random)]),
                ConcurrentStringStackFieldTest = new([RandomString(random), RandomString(random)]),
            },
            TestMessage.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
        //await Assert.That(parsed.NullableIntField).IsEqualTo(0);
    }

    string RandomString(Random random)
    {
        return random.Next(2) switch
        {
            //0 => string.Empty,
            _ => Guid.NewGuid().ToString(),
        };
    }

    int? RandomNullableInt(Random random)
    {
        return random.Next(2) switch
        {
            0 => null,
            _ => 1,
        };
    }

    int RandomInt(Random random) => random.Next(10);

    async Task<T2> Run<T1, T2>(T1 origin, Func<byte[], T2> parserFunc, Func<T2, byte[]> t2ToByteArray)
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
            new TestStruct { Name = RandomString(random), Value = RandomInt(random) },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public void TestRecord()
    {
        Run<TestRecord, TestType>(
            new TestRecord { Name = RandomString(random), Value = RandomInt(random) },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public void TestRecordStruct()
    {
        Run<TestRecordStruct, TestType>(
            new TestRecordStruct { Name = RandomString(random), Value = RandomInt(random) },
            TestType.Parser.ParseFrom,
            t2 => t2.ToByteArray()
        );
    }

    [Test]
    public async Task TestSimpleNewTypesSupport()
    {
        var testObj = new TestSimpleNewTypes
        {
            TimeSpanField = DateTime.Now.TimeOfDay,
            DateOnlyField = DateOnly.FromDateTime(DateTime.Today),
            GuidField = Guid.NewGuid(),
            TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
            StringBuilderField = new StringBuilder(RandomString(random)),
        };

        var serialized = testObj.ToByteArray();
        var deserialized = TestSimpleNewTypes.Reader.ParseFrom(serialized);

        // Verify round-trip serialization works
        await Assert.That(deserialized.TimeSpanField).IsEqualTo(testObj.TimeSpanField);
        await Assert.That(deserialized.DateOnlyField).IsEqualTo(testObj.DateOnlyField);
        await Assert.That(deserialized.GuidField).IsEqualTo(testObj.GuidField);
        await Assert.That(deserialized.TimeOnlyField.Ticks).IsEqualTo(testObj.TimeOnlyField.Ticks);
        await Assert.That(deserialized.StringBuilderField.ToString()).IsEqualTo(testObj.StringBuilderField.ToString());
    }

    [Test]
    public async Task TestHashSetSupport()
    {
        var testObj = new TestHashSet
        {
            Name = RandomString(random),
            IntSet = new HashSet<int> { 1, 2, 3, RandomInt(random) },
            StringSet = new HashSet<string> { "hello", "world", RandomString(random) },
        };

        // Test serialization/deserialization through protobuf
        var bytes = testObj.ToByteArray();
        var parsed = TestHashSet.Reader.ParseFrom(bytes);

        // Verify the sets are equal
        await Assert.That(parsed.Name).IsEqualTo(testObj.Name);
        await Assert.That(parsed.IntSet).IsEquivalentTo(testObj.IntSet);
        await Assert.That(parsed.StringSet).IsEquivalentTo(testObj.StringSet);

        // Test equality
        //await Assert.That(parsed).IsEqualTo(testObj);
    }

    [Test]
    public async Task TestISetSupport()
    {
        var testObj = new TestISet
        {
            Name = RandomString(random),
            IntSet = new HashSet<int> { 4, 5, 6, RandomInt(random) },
            StringSet = new HashSet<string> { "foo", "bar", RandomString(random) },
        };

        // Test serialization/deserialization through protobuf
        var bytes = testObj.ToByteArray();
        var parsed = TestISet.Reader.ParseFrom(bytes);

        // Verify the sets are equal
        await Assert.That(parsed.Name).IsEqualTo(testObj.Name);
        await Assert.That(parsed.IntSet).IsEquivalentTo(testObj.IntSet);
        await Assert.That(parsed.StringSet).IsEquivalentTo(testObj.StringSet);

        // Test equality
        //await Assert.That(parsed).IsEqualTo(testObj);
    }

    [Test]
    public async Task TestEmptyAndNullSets()
    {
        // Test with explicitly initialized empty sets
        var emptyObj = new TestHashSet
        {
            Name = "empty",
            IntSet = new HashSet<int>(),
            StringSet = new HashSet<string>(),
        };

        var bytes = emptyObj.ToByteArray();
        var parsed = TestHashSet.Reader.ParseFrom(bytes);

        await Assert.That(parsed.Name).IsEqualTo("empty");

        // For empty sets, just verify they serialize/deserialize without errors
        // and that the basic functionality works

        await Assert.That(bytes.Length).IsGreaterThan(0);
        await Assert.That(parsed).IsNotNull();

        // Test that we can add items to parsed sets
        if (parsed.IntSet != null)
        {
            parsed.IntSet.Add(1);
            await Assert.That(parsed.IntSet.Count).IsEqualTo(1);
        }
    }

    [Test]
    public async Task ConcurrentCollectionTest()
    {
        TestConcurrentCollection testObj = new TestConcurrentCollection
        {
            Name = RandomString(random),
            IntBag = [RandomInt(random), RandomInt(random)],
            ConcurrentQueue = new ConcurrentQueue<string>(
                [RandomString(random), RandomString(random)]
            ),
            ConcurrentStack = new ConcurrentStack<string>(
                [RandomString(random), RandomString(random)]
            ),
            IntList = [RandomInt(random), RandomInt(random)],
            IntIList = [RandomInt(random), RandomInt(random)],
        };

        var bytes = testObj.ToByteArray();
        var parsed = TestConcurrentCollection.Reader.ParseFrom(bytes);
        await Assert.That(parsed.Name).IsEqualTo(testObj.Name);
        await Assert.That(parsed.IntBag.Order().ToArray()).IsEquivalentTo(testObj.IntBag.Order().ToArray());
        await Assert.That(parsed.ConcurrentQueue).IsEquivalentTo(testObj.ConcurrentQueue);
        await Assert.That(parsed.ConcurrentStack).IsEquivalentTo(testObj.ConcurrentStack);
        await Assert.That(parsed.IntList).IsEquivalentTo(testObj.IntList);
        await Assert.That(parsed.IntIList).IsEquivalentTo(testObj.IntIList);

        //parsed.GetHashCode()await Assert.That().IsEqualTo(testObj.GetHashCode());
    }

    [Test]
    public async Task ProxyTest()
    {
        TestOrder testObj = new()
        {
            Instrument = Instrument.FromNameValue(RandomString(random), RandomInt(random)),
        };

        var bytes = testObj.ToByteArray();
        var parsed = TestOrder.Reader.ParseFrom(bytes);
        await Assert.That(parsed.Instrument.Name).IsEqualTo(testObj.Instrument.Name);
        await Assert.That(parsed.Instrument.Value).IsEqualTo(testObj.Instrument.Value);

        //parsed.GetHashCode()await Assert.That().IsEqualTo(testObj.GetHashCode());
    }
}

// public static class Extensions
// {
//     public static List<T> ToList<T>(this IEnumerable<T> source)
//     {
//         return new List<T> { source };
//     }
// }
