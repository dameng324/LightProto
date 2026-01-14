namespace LightProto.Tests;

public partial class RuntimeParserTests
{
    [ProtoContract]
    public partial class TestMessage
    {
        [ProtoMember(1)]
        public int Value { get; set; }

        [ProtoMember(2)]
        public string StringValue { get; set; }

        [ProtoMember(3)]
        public int[] IntArray { get; set; }
    }

    [Test]
    public async Task Test()
    {
        var runtimeParser = new RuntimeProtoParser<TestMessage>(() => new());
        runtimeParser.AddMember(
            1,
            message => message.Value,
            (message, value) => message.Value = value
        );
        runtimeParser.AddMember(
            2,
            message => message.StringValue,
            (message, value) => message.StringValue = value
        );
        runtimeParser.AddMember(
            3,
            message => message.IntArray,
            (message, value) => message.IntArray = value
        );

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
        public string StringValue { get; set; }
        public int[] IntArray { get; set; }
    }

    [Test]
    public async Task Test2()
    {
        var runtimeParser = new RuntimeProtoParser<TestMessage2<int>>(() => new());
        runtimeParser.AddMember(
            1,
            message => message.Value,
            (message, value) => message.Value = value
        );
        runtimeParser.AddMember(
            2,
            message => message.StringValue,
            (message, value) => message.StringValue = value
        );
        runtimeParser.AddMember(
            3,
            message => message.IntArray,
            (message, value) => message.IntArray = value
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
        var runtimeBytes = message2.ToByteArray<TestMessage2<int>>(runtimeParser);
        await Assert.That(runtimeBytes).IsEquivalentTo(bytes);

        var cloned = Serializer.Deserialize<TestMessage2<int>>(bytes, runtimeParser);

        await Assert.That(cloned.Value).IsEqualTo(message.Value);
        await Assert.That(cloned.StringValue).IsEqualTo(message.StringValue);
        await Assert.That(cloned.IntArray).IsEquivalentTo(message.IntArray);
    }
}
