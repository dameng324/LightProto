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
        var random=Random.Shared; // new Random(31);
        var parsed = Run<CsTestMessage, TestMessage>(
            new CsTestMessage
            {
                // RequiredIntField = RandomInt(random),
                // StringField = RandomString(random),
                // Int32Field = RandomInt(random),
                // Int32ArrayField = 
                //     new RepeatedField<int>()
                //     {
                //         0,
                //         13123,
                //     },
                // StringArrayField = 
                //     new RepeatedField<string>()
                //         {
                //             string.Empty,
                //             Guid.NewGuid().ToString(),
                //         },
                // BytesField = ByteString.CopyFrom(
                //     Enumerable
                //         .Range(0, random.Next(100))
                //         .Select(_ => (byte)RandomInt(random))
                //         .ToArray()
                // ),
                // BoolField = random.Next() % 2 == 0,
                // DoubleField = random.NextDouble(),
                // FloatField = (float)random.NextDouble(),
                // Int64Field = random.Next(),
                // UInt32Field = (uint)random.Next(),
                // UInt64Field = (ulong)random.Next(),
                // SInt32Field = random.Next(),
                // SInt64Field = random.Next(),
                // Fixed32Field = (uint)random.Next(),
                // Fixed64Field = (ulong)random.Next(),
                // SFixed32Field = random.Next(),
                // SFixed64Field = random.Next(),
                // MapField = new MapField<string, string>()
                // {
                //     ["key1"] = "value1",
                //     ["key2"] = "value2",
                // },
                // EnumField = CsTestEnum.OptionB,
                // EnumArrayField = [CsTestEnum.OptionB,CsTestEnum.None, CsTestEnum.OptionA],
                NestedMessageField = new CsTestMessage()
                {
                    RequiredIntField = RandomInt(random),
                    StringField = RandomString(random)
                },
                NestedMessageArrayField =
                [
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random)
                    },
                    new CsTestMessage()
                    {
                        RequiredIntField = RandomInt(random),
                        StringField = RandomString(random)
                    },
                ],
                TimestampField = DateTime.UtcNow.ToTimestamp(),
                DurationField = DateTime.UtcNow.TimeOfDay.ToDuration(),
                // MapField2 = new()
                // {
                //     [RandomString()] = RandomString(random)
                //     [RandomString()] = RandomString(random)
                // },
                // MapField3 = new()
                // {
                //     [RandomString()] = RandomString(random)
                //     [RandomString()] = RandomString(random)
                // },
                // MapField4 = new MapField<int, long>() { [1111] = 2222 },
                // DateTimeField = DateTime.UtcNow,
                // IntArrayFieldTest = [10, 20, 30],
                // StringListFieldTest = ["array", "list", "test"],
                // StringArrayFieldTest = ["hello", "world"],
                // IntListFieldTest = [100, 200, 300],
                // MapField5 = new Dictionary<string, string>()
                // {
                //     [RandomString()] = RandomString(random)
                //     [RandomString()] = RandomString(random)
                // },
                // MapField6 = new ConcurrentDictionary<string, string>()
                // {
                //     [RandomString()] = RandomString(random)
                //     [RandomString()] = RandomString(random)
                // },
                // RequiredIntField = RandomInt(random),
            }
        );
        //parsed.NullableIntField.Should().Be(0);
    }
    //
    // [Test]
    // public void GoogleToLocal()
    // {
    //     var testMessage = new TestMessage
    //     {
    //         StringField = RandomString(random)
    //         Int32Field = RandomInt(random),
    //         BytesField = ByteString.CopyFrom(
    //             Enumerable.Range(0, random.Next(10)).Select(_ => (byte)RandomInt(random)).ToArray()
    //         ),
    //         BoolField = random.Next() % 2 == 0,
    //         DoubleField = random.NextDouble(),
    //         FloatField = (float)random.NextDouble(),
    //         Int64Field = random.Next(),
    //         UInt32Field = (uint)random.Next(),
    //         UInt64Field = (ulong)random.Next(),
    //         SInt32Field = random.Next(),
    //         SInt64Field = random.Next(),
    //         Fixed32Field = (uint)random.Next(),
    //         Fixed64Field = (ulong)random.Next(),
    //         SFixed32Field = random.Next(),
    //         SFixed64Field = random.Next(),
    //         // EnumField = (TestEnum)CsTestEnum.None,
    //         // NestedMessageField = new TestMessage() { StringField = RandomString() },
    //         // TimestampField = Timestamp.FromDateTime(DateTime.UtcNow),
    //         // DurationField = Duration.FromTimeSpan(DateTime.Now.TimeOfDay),
    //         // DateTimeField = Timestamp.FromDateTime(DateTime.UtcNow),
    //     };
    //
    //     // Initialize collections after construction
    //     testMessage.Int32ArrayField.AddRange(
    //         [0, 13123]
    //     );
    //     testMessage.StringArrayField.AddRange(
    //         [string.Empty,Guid.NewGuid().ToString()]
    //     );
    //     // testMessage.MapField["key1"] = "value1";
    //     // testMessage.MapField["key2"] = "value2";
    //     // testMessage.EnumArrayField.AddRange(
    //     //     new[] { (TestEnum)CsTestEnum.None, (TestEnum)CsTestEnum.OptionA }
    //     // );
    //     // testMessage.NestedMessageArrayField.Add(new TestMessage() { StringField = RandomString() });
    //     // testMessage.NestedMessageArrayField.Add(new TestMessage() { StringField = RandomString() });
    //     // testMessage.MapField2["key1"] = "value1";
    //     // testMessage.MapField2["key2"] = "value2";
    //     // testMessage.MapField3["key1"] = "value1";
    //     // testMessage.MapField3["key2"] = "value2";
    //     // testMessage.MapField4[1111] = 2222;
    //     //
    //     // // Add array/list test data
    //     // testMessage.IntArrayFieldTest.AddRange([10, 20, 30]);
    //     // testMessage.StringListFieldTest.AddRange(["array", "list", "test"]);
    //     // testMessage.StringArrayFieldTest.AddRange(["hello", "world"]);
    //     // testMessage.IntListFieldTest.AddRange([100, 200, 300]);
    //
    //     var parsed = Run2<TestMessage, CsTestMessage>(testMessage);
    //     //parsed.NullableIntField.Should().BeNull();
    // }

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

    T2 Run2<T1, T2>(T1 obj)
        where T1 : IPbMessageParser<T1>
        where T2 : IProtoBufMessage<T2>
    {
        var bytes = obj.ToByteArray();
        var parsed1= T1.Parser.ParseFrom(bytes);
        var parsed = ProtoBuf.ParseFromBytes<T2>(bytes);

        // Compare the binary serialization instead of JSON for now
        var originalBytes = Convert.ToHexString(obj.ToByteArray());
        var parsedBytes = Convert.ToHexString(parsed.ToByteArray());

        // For now, just check that parsing doesn't throw and produces a result
        originalBytes.Should().Be(parsedBytes);

        parsed.CalculateSize().Should().Be(obj.CalculateSize());
        return parsed;
    }
    T2 Run<T1, T2>(T1 obj)
        where T1 : IProtoBufMessage<T1>
        where T2 : IPbMessageParser<T2>
    {
        var bytes = obj.ToByteArray();
        var parsed = T2.Parser.ParseFrom(bytes);

        // Compare the binary serialization instead of JSON for now
        var originalBytes = Convert.ToHexString(obj.ToByteArray());
        var parsedBytes = Convert.ToHexString(parsed.ToByteArray());
        // For now, just check that parsing doesn't throw and produces a result
        originalBytes.Should().Be(parsedBytes);
        parsed.CalculateSize().Should().Be(obj.CalculateSize());
        return parsed;
    }
    //
    // [Test]
    // public void TestStruct()
    // {
    //     Run<TestStruct, TestType>(new TestStruct { Name = RandomString(random) Value = RandomInt(random) });
    // }
    //
    // [Test]
    // public void TestRecord()
    // {
    //     Run<TestRecord, TestType>(new TestRecord { Name = RandomString(random) Value = RandomInt(random) });
    // }
    //
    // [Test]
    // public void TestRecordStruct()
    // {
    //     Run<TestRecordStruct, TestType>(
    //         new TestRecordStruct { Name = RandomString(random) Value = RandomInt(random) }
    //     );
    // }
    //
    // [Test]
    // public void TestSimpleNewTypesSupport()
    // {
    //     var testObj = new TestSimpleNewTypes
    //     {
    //         TimeSpanField = DateTime.Now.TimeOfDay,
    //         DateOnlyField = DateOnly.FromDateTime(DateTime.Today),
    //         GuidField = Guid.NewGuid(),
    //         TimeOnlyField = TimeOnly.FromDateTime(DateTime.Now),
    //         StringBuilderField = new StringBuilder(RandomString()),
    //     };
    //
    //     var serialized = testObj.ToByteArray();
    //     var deserialized = TestSimpleNewTypes.Parser.ParseFrom(serialized);
    //
    //     // Verify round-trip serialization works
    //     deserialized.TimeSpanField.Should().Be(testObj.TimeSpanField);
    //     deserialized.DateOnlyField.Should().Be(testObj.DateOnlyField);
    //     deserialized.GuidField.Should().Be(testObj.GuidField);
    //     deserialized.TimeOnlyField.Ticks.Should().Be(testObj.TimeOnlyField.Ticks);
    //     deserialized
    //         .StringBuilderField.ToString()
    //         .Should()
    //         .Be(testObj.StringBuilderField.ToString());
    // }
    //
    // [Test]
    // public void TestHashSetSupport()
    // {
    //     var testObj = new TestHashSet
    //     {
    //         Name = RandomString(random)
    //         IntSet = new HashSet<int> { 1, 2, 3, RandomInt(random) },
    //         StringSet = new HashSet<string> { "hello", "world", RandomString() },
    //     };
    //
    //     // Test serialization/deserialization through protobuf
    //     var bytes = testObj.ToByteArray();
    //     var parsed = TestHashSet.Parser.ParseFrom(bytes);
    //
    //     // Verify the sets are equal
    //     parsed.Name.Should().Be(testObj.Name);
    //     parsed.IntSet.Should().BeEquivalentTo(testObj.IntSet);
    //     parsed.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    //
    //     // Test equality
    //     parsed.Should().Be(testObj);
    //
    //     // Test cloning
    //     var cloned = testObj.Clone();
    //     cloned.Should().Be(testObj);
    //     cloned.IntSet.Should().BeEquivalentTo(testObj.IntSet);
    //     cloned.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    // }
    //
    // [Test]
    // public void TestISetSupport()
    // {
    //     var testObj = new TestISet
    //     {
    //         Name = RandomString(random)
    //         IntSet = new HashSet<int> { 4, 5, 6, RandomInt(random) },
    //         StringSet = new HashSet<string> { "foo", "bar", RandomString() },
    //     };
    //
    //     // Test serialization/deserialization through protobuf
    //     var bytes = testObj.ToByteArray();
    //     var parsed = TestISet.Parser.ParseFrom(bytes);
    //
    //     // Verify the sets are equal
    //     parsed.Name.Should().Be(testObj.Name);
    //     parsed.IntSet.Should().BeEquivalentTo(testObj.IntSet);
    //     parsed.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    //
    //     // Test equality
    //     parsed.Should().Be(testObj);
    //
    //     // Test cloning
    //     var cloned = testObj.Clone();
    //     cloned.Should().Be(testObj);
    //     cloned.IntSet.Should().BeEquivalentTo(testObj.IntSet);
    //     cloned.StringSet.Should().BeEquivalentTo(testObj.StringSet);
    // }
    //
    // [Test]
    // public void TestEmptyAndNullSets()
    // {
    //     // Test with explicitly initialized empty sets
    //     var emptyObj = new TestHashSet
    //     {
    //         Name = "empty",
    //         IntSet = new HashSet<int>(),
    //         StringSet = new HashSet<string>(),
    //     };
    //
    //     var bytes = emptyObj.ToByteArray();
    //     var parsed = TestHashSet.Parser.ParseFrom(bytes);
    //
    //     parsed.Name.Should().Be("empty");
    //
    //     // For empty sets, just verify they serialize/deserialize without errors
    //     // and that the basic functionality works
    //     bytes.Length.Should().BeGreaterThan(0);
    //     parsed.Should().NotBeNull();
    //
    //     // Test that we can add items to parsed sets
    //     if (parsed.IntSet != null)
    //     {
    //         parsed.IntSet.Add(1);
    //         parsed.IntSet.Count.Should().Be(1);
    //     }
    // }
    //
    // [Test]
    // public void MergeFromTest()
    // {
    //     TestHashSet set1 = new TestHashSet() { IntSet = [1, 2, 3] };
    //     TestHashSet set2 = new TestHashSet() { IntSet = [3, 4, 5] };
    //
    //     var bytes = set2.ToByteArray();
    //
    //     set1.MergeFrom(bytes);
    //
    //     set1.IntSet.Count.Should().Be(5);
    // }
    //
    // [Test]
    // public void ConcurrentCollectionTest()
    // {
    //     TestConcurrentCollection testObj = new TestConcurrentCollection
    //     {
    //         Name = RandomString(random)
    //         IntBag = [RandomInt(random), RandomInt(random)],
    //         ConcurrentQueue = new ConcurrentQueue<string>([RandomString(random) RandomString()]),
    //         ConcurrentStack = new ConcurrentStack<string>([RandomString(random) RandomString()]),
    //         IntList = [RandomInt(random), RandomInt(random)],
    //         IntIList = [RandomInt(random), RandomInt(random)],
    //         IntImmutableArray = [RandomInt(random), RandomInt(random)],
    //     };
    //
    //     var bytes = testObj.ToByteArray();
    //     var parsed = TestConcurrentCollection.ParseFrom(bytes);
    //     parsed.Name.Should().Be(testObj.Name);
    //     parsed.IntBag.Should().BeEquivalentTo(testObj.IntBag);
    //     parsed.ConcurrentQueue.Should().BeEquivalentTo(testObj.ConcurrentQueue);
    //     parsed.ConcurrentStack.Should().BeEquivalentTo(testObj.ConcurrentStack);
    //     parsed.IntList.Should().BeEquivalentTo(testObj.IntList);
    //     parsed.IntIList.Should().BeEquivalentTo(testObj.IntIList);
    //     parsed.IntImmutableArray.Should().BeEquivalentTo(testObj.IntImmutableArray);
    //
    //     parsed.GetHashCode().Should().Be(testObj.GetHashCode());
    // }
    //
    // [Test]
    // public void ProxyTest()
    // {
    //     TestOrder testObj = new()
    //     {
    //         Instrument = Instrument.FromNameValue(RandomString(random) RandomInt(random)),
    //     };
    //
    //     var bytes = testObj.ToByteArray();
    //     var parsed = TestOrder.Parser.ParseFrom(bytes);
    //     parsed.Instrument.Name.Should().Be(testObj.Instrument.Name);
    //     parsed.Instrument.Value.Should().Be(testObj.Instrument.Value);
    //
    //     parsed.GetHashCode().Should().Be(testObj.GetHashCode());
    // }
}

public static class Extensions
{
    public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> source)
    {
        return new RepeatedField<T> { source };
    }
}
