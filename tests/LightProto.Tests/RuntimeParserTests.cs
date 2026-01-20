using System.Diagnostics.CodeAnalysis;
using LightProto.Parser;

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
    public async Task RuntimeParser_Serialization_Deserialization_ProducesEquivalentResults()
    {
        var runtimeParser = new RuntimeProtoParser<TestMessage>(() => new());
        runtimeParser.AddMember(1, message => message.Value, (message, value) => message.Value = value);
        runtimeParser.AddMember(2, message => message.StringValue, (message, value) => message.StringValue = value);
        runtimeParser.AddMember<int[]>(
            3,
            message => message.IntArray,
            (message, value) => message.IntArray = value,
            Int32ProtoParser.ProtoReader.GetArrayReader(),
            Int32ProtoParser.ProtoWriter.GetCollectionWriter()
        );

        await Assert.That(runtimeParser.ProtoReader.IsMessage).IsEqualTo(true);
        await Assert.That(runtimeParser.ProtoReader.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);
        await Assert.That(runtimeParser.ProtoWriter.IsMessage).IsEqualTo(true);
        await Assert.That(runtimeParser.ProtoWriter.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);

        var message = new TestMessage
        {
            Value = 42,
            StringValue = "Hello, Runtime Parser!",
            IntArray = new int[] { 1, 2, 3, 4, 5 },
        };
        var bytes = message.ToByteArray(TestMessage.ProtoWriter);
        var runtimeBytes = message.ToByteArray(runtimeParser.ProtoWriter);
        await Assert.That(runtimeBytes).IsEquivalentTo(bytes);

        var cloned = Serializer.Deserialize<TestMessage>(bytes, runtimeParser.ProtoReader);

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

    internal class TestMessage2RuntimeProtoReader<T1> : RuntimeProtoReader<TestMessage2<T1>>
    {
        public TestMessage2RuntimeProtoReader()
            : base(() => new())
        {
            AddMember<T1>(1, (x, v) => x.Value = v);
            AddMember<string>(2, (x, v) => x.StringValue = v);
            AddMember<int[]>(3, (x, v) => x.IntArray = v);
        }
    }

    internal class TestMessage2RuntimeProtoWriter<T1> : RuntimeProtoWriter<TestMessage2<T1>>
    {
        public TestMessage2RuntimeProtoWriter()
        {
            AddMember<T1>(1, x => x.Value);
            AddMember<string>(2, x => x.StringValue);
            AddMember<int[]>(3, x => x.IntArray);
        }
    }

    [Test]
    [SkipAot]
    public async Task GenericRuntimeParser_WithRegistration_WorksCorrectly()
    {
        Serializer.RegisterGenericParser(
            typeof(TestMessage2<>),
            typeof(TestMessage2RuntimeProtoReader<>),
            typeof(TestMessage2RuntimeProtoWriter<>)
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

    [Test]
    [SkipAot]
    public async Task GenericRuntimeParser_WithRegistration_WorksCorrectly2()
    {
        Serializer.RegisterGenericParser(
            typeof(TestMessage2<>),
            typeof(TestMessage2RuntimeProtoReader<>),
            typeof(TestMessage2RuntimeProtoWriter<>)
        );

        var message = new TestMessage2<TestMessage>
        {
            Value = new TestMessage()
            {
                Value = 42,
                StringValue = "Hello, Runtime Parser!",
                IntArray = new int[] { 1, 2, 3, 4, 5 },
            },
            StringValue = "Hello, Runtime Parser!",
            IntArray = new int[] { 1, 2, 3, 4, 5 },
        };
        var bytes = Serializer.SerializeToArrayDynamically(message);

        var cloned = Serializer.DeserializeDynamically<TestMessage2<TestMessage>>(bytes);

        await Assert.That(cloned.Value.Value).IsEqualTo(message.Value.Value);
        await Assert.That(cloned.Value.StringValue).IsEqualTo(message.Value.StringValue);
        await Assert.That(cloned.Value.IntArray).IsEquivalentTo(message.Value.IntArray);
        await Assert.That(cloned.StringValue).IsEqualTo(message.StringValue);
        await Assert.That(cloned.IntArray).IsEquivalentTo(message.IntArray);
    }
}
