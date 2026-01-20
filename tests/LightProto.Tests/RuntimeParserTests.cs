namespace LightProto.Tests;

public partial class RuntimeParserTests
{
    [ProtoContract]
    public partial class TestMessage
    {
        [ProtoMember(1)]
        public int Value { get; set; }

        [ProtoMember(2)]
        public string StringValue { get; set; } = string.Empty;

        [ProtoMember(3)]
        public int[] IntArray { get; set; } = [];
    }

    [Test]
    public async Task Test()
    {
        var runtimeParser = new RuntimeProtoParser<TestMessage>(() => new());
        runtimeParser.AddMember(1, message => message.Value, (message, value) => message.Value = value);
        runtimeParser.AddMember(2, message => message.StringValue, (message, value) => message.StringValue = value);
        runtimeParser.AddMember(3, message => message.IntArray, (message, value) => message.IntArray = value);

        var message = new TestMessage
        {
            Value = 42,
            StringValue = "Hello, Runtime Parser!",
            IntArray = new int[] { 1, 2, 3, 4, 5 },
        };
        var bytes = message.ToByteArray(TestMessage.ProtoWriter);
        var runtimeBytes = message.ToByteArray(runtimeParser);
        await Assert.That(runtimeBytes).IsEquivalentTo(bytes);

        var cloned = Serializer.Deserialize<TestMessage>(bytes, runtimeParser);

        await Assert.That(cloned.Value).IsEqualTo(message.Value);
        await Assert.That(cloned.StringValue).IsEqualTo(message.StringValue);
        await Assert.That(cloned.IntArray).IsEquivalentTo(message.IntArray);
    }

    public class TestMessage2<T>
    {
        public T Value { get; set; }
        public string StringValue { get; set; } = string.Empty;
        public int[] IntArray { get; set; } = [];
    }

    internal class TestMessage2RuntimeProtoParser<T1> : RuntimeProtoParser<TestMessage2<T1>>
    {
        public TestMessage2RuntimeProtoParser()
            : base(() => new())
        {
            AddMember(1, x => x.Value, (x, v) => x.Value = v);
            AddMember(2, x => x.StringValue, (x, v) => x.StringValue = v);
            AddMember(3, x => x.IntArray, (x, v) => x.IntArray = v);
        }
    }

    [Test]
    public async Task Test2()
    {
        Serializer.RegisterGenericParser(
            typeof(TestMessage2<>),
            typeof(TestMessage2RuntimeProtoParser<>),
            typeof(TestMessage2RuntimeProtoParser<>)
        );
        var message = new TestMessage
        {
            Value = 42,
            StringValue = "Hello, Runtime Parser!",
            IntArray = new int[] { 1, 2, 3, 4, 5 },
        };
        var bytes = message.ToByteArray(TestMessage.ProtoWriter);
        var message2 = new TestMessage2<int>
        {
            Value = 42,
            StringValue = "Hello, Runtime Parser!",
            IntArray = new int[] { 1, 2, 3, 4, 5 },
        };
        var runtimeBytes = Serializer.SerializeToArrayDynamically(message2);
        await Assert.That(runtimeBytes).IsEquivalentTo(bytes);

        var cloned = Serializer.DeserializeDynamically<TestMessage2<int>>(bytes);

        await Assert.That(cloned.Value).IsEqualTo(message.Value);
        await Assert.That(cloned.StringValue).IsEqualTo(message.StringValue);
        await Assert.That(cloned.IntArray).IsEquivalentTo(message.IntArray);
    }
}
