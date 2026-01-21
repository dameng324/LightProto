using System.Collections;
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
        runtimeParser.AddMember(typeof(string), 2, message => message.StringValue, (message, value) => message.StringValue = (string)value);
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
        public TestMessage2RuntimeProtoWriter(IProtoWriter<T1> writer)
        {
            AddMember<T1>(1, x => x.Value, writer);
            AddMember<string>(2, x => x.StringValue);
            AddMember<int[]>(3, x => x.IntArray);
        }
    }

    [Test]
    [SkipAot]
    public async Task GenericRuntimeParser_Int_WithRegistration_WorksCorrectly()
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
    public async Task GenericRuntimeParser_Message_WithRegistration_WorksCorrectly()
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

    [Test]
    [SkipAot]
    public async Task GenericRuntimeParser_Message_NonGeneric_WithRegistration_WorksCorrectly()
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
        var bytes = Serializer.SerializeToArrayNonGeneric(message);

        var cloned = (TestMessage2<TestMessage>)Serializer.DeserializeNonGeneric(typeof(TestMessage2<TestMessage>), bytes);

        await Assert.That(cloned.Value.Value).IsEqualTo(message.Value.Value);
        await Assert.That(cloned.Value.StringValue).IsEqualTo(message.Value.StringValue);
        await Assert.That(cloned.Value.IntArray).IsEquivalentTo(message.Value.IntArray);
        await Assert.That(cloned.StringValue).IsEqualTo(message.StringValue);
        await Assert.That(cloned.IntArray).IsEquivalentTo(message.IntArray);
    }

    public sealed class MyListProtoWriter<T> : IEnumerableProtoWriter<MyList<T>, T>
    {
        public MyListProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
            : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
    }

    public sealed class MyListProtoReader<T> : IEnumerableProtoReader<MyList<T>, T>
    {
        public MyListProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
            : base(
                itemReader,
                static capacity => new MyList<T>(capacity),
                static (collection, item) =>
                {
                    collection.Add(item);
                    return collection;
                },
                itemFixedSize
            ) { }
    }

    public class MyList<T> : IEnumerable<T>
    {
        private readonly List<T> _innerList;

        public MyList(int capacity)
        {
            _innerList = new List<T>(capacity);
        }

        public void Add(T item)
        {
            _innerList.Add(item);
        }

        public int Count => _innerList.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }
    }

    [Test]
    [SkipAot]
    public async Task GenericRuntimeParser_MyList_WorksCorrectly()
    {
        Serializer.RegisterGenericParser(typeof(MyList<>), typeof(MyListProtoReader<>), typeof(MyListProtoWriter<>));
        var message = new MyList<int>(10) { 1, 2, 3, 4, 5 };
        var bytes = Serializer.SerializeToArrayDynamically(message);
        var cloned = Serializer.DeserializeDynamically<MyList<int>>(bytes);
        await Assert.That(cloned).IsEquivalentTo(message);
    }
}
