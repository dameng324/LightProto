using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using AwesomeAssertions;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using TestPackage;

namespace Dameng.Protobuf.Extension.Tests;

public class Tests
{
    [Test]
    public void LocalToGoogle()
    {
        var parsed = Run<CsTestMessage, TestMessage>(
            new CsTestMessage
            {
                RequiredIntField = RandomInt(),
                StringField = RandomString(),
                Int32Field = RandomInt(),
                Int32ArrayField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomInt())
                    .ToRepeatedField(),
                StringArrayField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomString())
                    .ToRepeatedField(),
                BytesField = ByteString.CopyFrom(
                    Enumerable
                        .Range(0, Random.Shared.Next(10))
                        .Select(_ => (byte)RandomInt())
                        .ToArray()
                ),
                BoolField = Random.Shared.Next() % 2 == 0,
                DoubleField = Random.Shared.NextDouble(),
                FloatField = (float)Random.Shared.NextDouble(),
                Int64Field = Random.Shared.Next(),
                UInt32Field = (uint)Random.Shared.Next(),
                UInt64Field = (ulong)Random.Shared.Next(),
                SInt32Field = Random.Shared.Next(),
                SInt64Field = Random.Shared.Next(),
                Fixed32Field = (uint)Random.Shared.Next(),
                Fixed64Field = (ulong)Random.Shared.Next(),
                SFixed32Field = Random.Shared.Next(),
                SFixed64Field = Random.Shared.Next(),
                MapField = new MapField<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                EnumField = CsTestEnum.None,
                EnumArrayField = [CsTestEnum.None, CsTestEnum.OptionA],
                NestedMessageField = new CsTestMessage()
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
                MapField3 = new()
                {
                    [RandomString()] = RandomString(),
                    [RandomString()] = RandomString(),
                },
                MapField4 = new MapField<int, long>() { [1111] = 2222 },
                DateTimeField = DateTime.UtcNow,
                IntArrayFieldTest = [10, 20, 30],
                StringListFieldTest = ["array", "list", "test"],
                StringArrayFieldTest = ["hello", "world"],
                IntListFieldTest = [100, 200, 300],
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
            }
        );
        parsed.NullableIntField.Should().Be(0);
    }

    [Test]
    public void GoogleToLocal()
    {
        var testMessage = new TestMessage
        {
            StringField = RandomString(),
            Int32Field = RandomInt(),
            BytesField = ByteString.CopyFrom(
                Enumerable.Range(0, Random.Shared.Next(10)).Select(_ => (byte)RandomInt()).ToArray()
            ),
            BoolField = Random.Shared.Next() % 2 == 0,
            DoubleField = Random.Shared.NextDouble(),
            FloatField = (float)Random.Shared.NextDouble(),
            Int64Field = Random.Shared.Next(),
            UInt32Field = (uint)Random.Shared.Next(),
            UInt64Field = (ulong)Random.Shared.Next(),
            SInt32Field = Random.Shared.Next(),
            SInt64Field = Random.Shared.Next(),
            Fixed32Field = (uint)Random.Shared.Next(),
            Fixed64Field = (ulong)Random.Shared.Next(),
            SFixed32Field = Random.Shared.Next(),
            SFixed64Field = Random.Shared.Next(),
            EnumField = (TestEnum)CsTestEnum.None,
            NestedMessageField = new TestMessage() { StringField = RandomString() },
            TimestampField = Timestamp.FromDateTime(DateTime.UtcNow),
            DurationField = Duration.FromTimeSpan(DateTime.Now.TimeOfDay),
            DateTimeField = Timestamp.FromDateTime(DateTime.UtcNow),
        };

        // Initialize collections after construction
        testMessage.Int32ArrayField.AddRange(
            Enumerable.Range(0, Random.Shared.Next(10)).Select(_ => RandomInt())
        );
        testMessage.StringArrayField.AddRange(
            Enumerable.Range(0, Random.Shared.Next(10)).Select(_ => RandomString()!)
        );
        testMessage.MapField["key1"] = "value1";
        testMessage.MapField["key2"] = "value2";
        testMessage.EnumArrayField.AddRange(
            new[] { (TestEnum)CsTestEnum.None, (TestEnum)CsTestEnum.OptionA }
        );
        testMessage.NestedMessageArrayField.Add(new TestMessage() { StringField = RandomString() });
        testMessage.NestedMessageArrayField.Add(new TestMessage() { StringField = RandomString() });
        testMessage.MapField2["key1"] = "value1";
        testMessage.MapField2["key2"] = "value2";
        testMessage.MapField3["key1"] = "value1";
        testMessage.MapField3["key2"] = "value2";
        testMessage.MapField4[1111] = 2222;

        // Add array/list test data
        testMessage.IntArrayFieldTest.AddRange([10, 20, 30]);
        testMessage.StringListFieldTest.AddRange(["array", "list", "test"]);
        testMessage.StringArrayFieldTest.AddRange(["hello", "world"]);
        testMessage.IntListFieldTest.AddRange([100, 200, 300]);

        var parsed = Run<TestMessage, CsTestMessage>(testMessage);
        parsed.NullableIntField.Should().BeNull();
    }

    string RandomString()
    {
        return Random.Shared.Next(2) switch
        {
            0 => string.Empty,
            _ => Guid.NewGuid().ToString(),
        };
    }

    int? RandomNullableInt()
    {
        return Random.Shared.Next(2) switch
        {
            0 => null,
            _ => 1,
        };
    }

    int RandomInt() => Random.Shared.Next(10);

    T2 Run<T1, T2>(T1 obj)
        where T1 : IPbMessageParser<T1>
        where T2 : IPbMessageParser<T2>
    {
        var bytes = obj.ToByteArray();
        var parsed = T2.Parser.ParseFrom(bytes);

        // Compare the binary serialization instead of JSON for now
        var originalBytes = Convert.ToBase64String(obj.ToByteArray());
        var parsedBytes = Convert.ToBase64String(parsed.ToByteArray());

        // For now, just check that parsing doesn't throw and produces a result
        originalBytes.Should().Be(parsedBytes);

        parsed.CalculateSize().Should().Be(obj.CalculateSize());
        return parsed;
    }

    [Test]
    public void TestStruct()
    {
        Run<TestStruct, TestType>(new TestStruct { Name = RandomString(), Value = RandomInt() });
    }

    [Test]
    public void TestRecord()
    {
        Run<TestRecord, TestType>(new TestRecord { Name = RandomString(), Value = RandomInt() });
    }

    [Test]
    public void TestRecordStruct()
    {
        Run<TestRecordStruct, TestType>(
            new TestRecordStruct { Name = RandomString(), Value = RandomInt() }
        );
    }

    [Test]
    public void TestSimpleNewTypesSupport()
    {
        var testObj = new TestSimpleNewTypes
        {
            TimeSpanField = DateTime.Now.TimeOfDay,
            DateOnlyField = DateOnly.FromDateTime(DateTime.Today),
            GuidField = Guid.NewGuid(),
            TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
            StringBuilderField = new StringBuilder(RandomString()),
        };

        var serialized = testObj.ToByteArray();
        var deserialized = TestSimpleNewTypes.Parser.ParseFrom(serialized);

        // Verify round-trip serialization works
        deserialized.TimeSpanField.Should().Be(testObj.TimeSpanField);
        deserialized.DateOnlyField.Should().Be(testObj.DateOnlyField);
        deserialized.GuidField.Should().Be(testObj.GuidField);
        deserialized.TimeOnlyField.Ticks.Should().Be(testObj.TimeOnlyField.Ticks);
        deserialized
            .StringBuilderField.ToString()
            .Should()
            .Be(testObj.StringBuilderField.ToString());
    }

    [Test]
    public void TestHashSetSupport()
    {
        var testObj = new TestHashSet
        {
            Name = RandomString(),
            IntSet = new HashSet<int> { 1, 2, 3, RandomInt() },
            StringSet = new HashSet<string> { "hello", "world", RandomString() },
        };

        // Test serialization/deserialization through protobuf
        var bytes = testObj.ToByteArray();
        var parsed = TestHashSet.Parser.ParseFrom(bytes);

        // Verify the sets are equal
        parsed.Name.Should().Be(testObj.Name);
        parsed.IntSet.Should().BeEquivalentTo(testObj.IntSet);
        parsed.StringSet.Should().BeEquivalentTo(testObj.StringSet);

        // Test equality
        parsed.Should().Be(testObj);

        // Test cloning
        var cloned = testObj.Clone();
        cloned.Should().Be(testObj);
        cloned.IntSet.Should().BeEquivalentTo(testObj.IntSet);
        cloned.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    }

    [Test]
    public void TestISetSupport()
    {
        var testObj = new TestISet
        {
            Name = RandomString(),
            IntSet = new HashSet<int> { 4, 5, 6, RandomInt() },
            StringSet = new HashSet<string> { "foo", "bar", RandomString() },
        };

        // Test serialization/deserialization through protobuf
        var bytes = testObj.ToByteArray();
        var parsed = TestISet.Parser.ParseFrom(bytes);

        // Verify the sets are equal
        parsed.Name.Should().Be(testObj.Name);
        parsed.IntSet.Should().BeEquivalentTo(testObj.IntSet);
        parsed.StringSet.Should().BeEquivalentTo(testObj.StringSet);

        // Test equality
        parsed.Should().Be(testObj);

        // Test cloning
        var cloned = testObj.Clone();
        cloned.Should().Be(testObj);
        cloned.IntSet.Should().BeEquivalentTo(testObj.IntSet);
        cloned.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    }

    [Test]
    public void TestEmptyAndNullSets()
    {
        // Test with explicitly initialized empty sets
        var emptyObj = new TestHashSet
        {
            Name = "empty",
            IntSet = new HashSet<int>(),
            StringSet = new HashSet<string>(),
        };

        var bytes = emptyObj.ToByteArray();
        var parsed = TestHashSet.Parser.ParseFrom(bytes);

        parsed.Name.Should().Be("empty");

        // For empty sets, just verify they serialize/deserialize without errors
        // and that the basic functionality works
        bytes.Length.Should().BeGreaterThan(0);
        parsed.Should().NotBeNull();

        // Test that we can add items to parsed sets
        if (parsed.IntSet != null)
        {
            parsed.IntSet.Add(1);
            parsed.IntSet.Count.Should().Be(1);
        }
    }

    [Test]
    public void MergeFromTest()
    {
        TestHashSet set1 = new TestHashSet() { IntSet = [1, 2, 3] };
        TestHashSet set2 = new TestHashSet() { IntSet = [3, 4, 5] };

        var bytes = set2.ToByteArray();

        set1.MergeFrom(bytes);

        set1.IntSet.Count.Should().Be(5);
    }

    [Test]
    public void ConcurrentCollectionTest()
    {
        TestConcurrentCollection testObj = new TestConcurrentCollection
        {
            Name = RandomString(),
            IntBag = [RandomInt(), RandomInt()],
            ConcurrentQueue = new ConcurrentQueue<string>([RandomString(), RandomString()]),
            ConcurrentStack = new ConcurrentStack<string>([RandomString(), RandomString()]),
            IntList = [RandomInt(), RandomInt()],
            IntIList = [RandomInt(), RandomInt()],
            IntImmutableArray = [RandomInt(), RandomInt()],
        };

        var bytes = testObj.ToByteArray();
        var parsed = TestConcurrentCollection.Parser.ParseFrom(bytes);
        parsed.Name.Should().Be(testObj.Name);
        parsed.IntBag.Should().BeEquivalentTo(testObj.IntBag);
        parsed.ConcurrentQueue.Should().BeEquivalentTo(testObj.ConcurrentQueue);
        parsed.ConcurrentStack.Should().BeEquivalentTo(testObj.ConcurrentStack);
        parsed.IntList.Should().BeEquivalentTo(testObj.IntList);
        parsed.IntIList.Should().BeEquivalentTo(testObj.IntIList);
        parsed.IntImmutableArray.Should().BeEquivalentTo(testObj.IntImmutableArray);

        parsed.GetHashCode().Should().Be(testObj.GetHashCode());
    }

    [Test]
    public void ProxyTest()
    {
        TestOrder testObj = new()
        {
            Instrument = Instrument.FromNameValue(RandomString(), RandomInt()),
        };

        var bytes = testObj.ToByteArray();
        var parsed = TestOrder.Parser.ParseFrom(bytes);
        parsed.Instrument.Name.Should().Be(testObj.Instrument.Name);
        parsed.Instrument.Value.Should().Be(testObj.Instrument.Value);

        parsed.GetHashCode().Should().Be(testObj.GetHashCode());
    }
}

public static class Extensions
{
    public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> source)
    {
        return new RepeatedField<T> { source };
    }
}
