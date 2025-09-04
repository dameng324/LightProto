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
                NestedMessageField = new CsTestMessage() { StringField = RandomString() },
                NestedMessageArrayField =
                [
                    new CsTestMessage() { StringField = RandomString() },
                    new CsTestMessage() { StringField = RandomString() },
                ],
                TimestampField = DateTime.UtcNow.ToTimestamp(),
                DurationField = DateTime.UtcNow.TimeOfDay.ToDuration(),
                MapField2 = new MapField<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                MapField3 = new MapField<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                MapField4 = new MapField<int, long>() { [1111] = 2222 },
                DateTimeField = DateTime.UtcNow,
                IntArrayFieldTest = [10, 20, 30],
                StringListFieldTest = ["array", "list", "test"],
                StringArrayFieldTest = ["hello", "world"],
                IntListFieldTest = [100, 200, 300]
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

    int RandomInt() => Random.Shared.Next(2);

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
        return parsed;
    }

    T Deserialize<T>(byte[] bytes)
        where T : IPbMessageParser<T>
    {
        return T.Parser.ParseFrom(bytes);
    }
}

public static class Extensions
{
    public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> source)
    {
        return new RepeatedField<T> { source };
    }
}
