using System.Collections.Concurrent;
using AwesomeAssertions;
using Dameng.Protobuf.Extension;
using Google.Protobuf;
using Google.Protobuf.Collections;
using TestPackage;

namespace Dameng.Protobuf.Extension.Tests;

public class Tests
{
    [Test]
    public void Test()
    {
        Run<CsTestMessage,TestMessage>(
            new CsTestMessage
            {
                StringField = RandomString(),
                Int32Field = RandomInt(),
                Int32ArrayField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomInt())
                    .ToArray(),
                StringArrayField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomString()!)
                    .ToArray(),
                BytesField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => (byte)RandomInt())
                    .ToArray(),
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
                MapField = new Dictionary<string, string>()
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
                OneofStringField = "1111",
                OneofInt32Field = null,
                OneofNestedMessage1 = new CsTestMessage() { StringField = RandomString() },
                OneofNestedMessage2 = null,
                TimestampField = DateTime.Now,
                DurationField = DateTime.Now.TimeOfDay,
                MapField2 = new Dictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                MapField3 = new ConcurrentDictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
            }
        );
        Run<TestMessage,CsTestMessage>(
            new TestMessage
            {
                StringField = RandomString(),
                Int32Field = RandomInt(),
                Int32ArrayField = Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomInt())
                    .ToArray(),
                StringArrayField = new RepeatedField<string>(Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => RandomString()!)
                    .ToArray()) ,
                BytesField = ByteString.CopyFrom(Enumerable
                    .Range(0, Random.Shared.Next(10))
                    .Select(_ => (byte)RandomInt())
                    .ToArray()) ,
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
                MapField = new Dictionary<string, string>()
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
                OneofStringField = "1111",
                OneofInt32Field = null,
                OneofNestedMessage1 = new CsTestMessage() { StringField = RandomString() },
                OneofNestedMessage2 = null,
                TimestampField = DateTime.Now,
                DurationField = DateTime.Now.TimeOfDay,
                MapField2 = new Dictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
                MapField3 = new ConcurrentDictionary<string, string>()
                {
                    ["key1"] = "value1",
                    ["key2"] = "value2",
                },
            }
        );
        
    }

    string? RandomString()
    {
        return Random.Shared.Next(3) switch
        {
            0 => null,
            1 => string.Empty,
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

    void Run<T1, T2>(T1 obj)
        where T1 : IPbMessageParser<T1>
        where T2 : IPbMessageParser<T2>
    {
        var bytes = obj.ToByteArray();
        var parsed = T2.Parser.ParseFrom(bytes);

        var originalJson = JsonFormatter.Default.Format(obj);
        var parsedJson = JsonFormatter.Default.Format(parsed);
        originalJson.Should().Be(parsedJson);
    }

    T Deserialize<T>(byte[] bytes)
        where T : IPbMessageParser<T>
    {
        return T.Parser.ParseFrom(bytes);
    }
}
