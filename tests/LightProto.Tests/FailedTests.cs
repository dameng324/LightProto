using System.Buffers;
using LightProto.Parser;

namespace LightProto.Tests;

public partial class FailedTests
{
    [Test]
    public async Task Sequence_contained_null_element_WhenSerializing()
    {
        List<string> strings = new() { "one", null!, "three" };
        var ex = await Assert.ThrowsAsync<Exception>(async () =>
        {
            var bytes = strings.ToByteArray(StringProtoParser.ProtoWriter);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).IsEqualTo("Sequence contained null element");
    }

    [Test]
    public async Task Sequence_contained_null_element_WhenSerializing2()
    {
        HashSet<string> strings = new() { "one", null!, "three" };
        var ex = await Assert.ThrowsAsync<Exception>(async () =>
        {
            var bytes = strings.ToByteArray(StringProtoParser.ProtoWriter);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).IsEqualTo("Sequence contained null element");
    }

    [Test]
    public async Task InvalidTag_WhenDeserializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[] { 0, 1, 0 };
            var strings = Serializer.Deserialize<List<int>, int>(
                bytes,
                Int32ProtoParser.ProtoReader
            );
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("invalid tag");
    }

    [ProtoContract]
    public partial class TestContract
    {
        [ProtoMember(1)]
        public TestContract? Object { get; set; }

        [ProtoMember(2)]
        public int Value { get; set; }
    }

    [Test]
    public async Task NegativeSize_WhenDeserializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[] { 10, 255, 255, 255, 255, 255, 255, 255, 255, 1, 16, 1 };
            var strings = Serializer.Deserialize<TestContract>(bytes);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("have negative size");
    }

    [Test]
    public async Task TruncatedMessage_WhenDeserializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[] { 8, 1, 8 };
            var strings = Serializer.Deserialize<List<int>, int>(
                bytes,
                Int32ProtoParser.ProtoReader
            );
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("input has been truncated");
    }

    [Test]
    public async Task InvalidUtf8_WhenDeserializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            // normal bytes is new byte[] { 10,3,111,110,111,10,5,116,104,114,101,101 };
            var bytes = new byte[] { 10, 3, 0xE0, 0x80, 0x80, 10, 5, 116, 104, 114, 101, 101 };
            var strings = Serializer.Deserialize<List<string>, string>(
                bytes,
                StringProtoParser.ProtoReader
            );
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("is invalid UTF-8");
    }

    [Test]
    public async Task RecursionLimitExceeded_WhenDeserializing()
    {
        var bytes = new TestContract()
        {
            Object = new TestContract()
            {
                Value = 1,
                Object = new TestContract() { Object = new TestContract() { Value = 1 } },
            },
        }.ToByteArray(); //10,8,10,4,10,2,16,1,16,1
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[] { 10, 8, 10, 4, 10, 2, 16, 1, 16, 1 };
            var obj = Parse(bytes);
            static TestContract Parse(byte[] bytes)
            {
                ReaderContext.Initialize(bytes, out var ctx);
                ctx.state.recursionLimit = 2;
                return TestContract.ProtoReader.ParseFrom(ref ctx);
            }
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("too many levels");
    }

    [Test]
    public async Task SizeLimitExceeded_WhenSerializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = SerializerTests.GetReadonlySequence(
                new byte[] { 10, 8, 10, 4, 10, 2, 16, 1, 16, 1 }
                    .Chunk(2)
                    .ToArray()
            );
            var obj = Parse(bytes);
            static TestContract Parse(ReadOnlySequence<byte> bytes)
            {
                ReaderContext.Initialize(bytes, out var ctx);
                ctx.state.sizeLimit = 3;
                return TestContract.ProtoReader.ParseFrom(ref ctx);
            }
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("message was too large");
    }

    [ProtoContract]
    public partial class MalformedVarint
    {
        [ProtoMember(1)]
        public int Value { get; set; }
    }

    [Test]
    public async Task MalformedVarint_WhenDeserializing()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[]
            {
                0x08,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
            };
            var strings = Serializer.Deserialize<MalformedVarint>(bytes);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("malformed varint");
    }
    // [Test]
    // public async Task MoreDataAvailable_WhenDeserializing()
    // {
    //     var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
    //     {
    //         var bytes = new byte[] { 0x08, 0x96, 0x01, 0xFF, 0xFF};
    //         var strings = Serializer.Deserialize<MalformedVarint>(
    //             bytes
    //         );
    //         await Task.CompletedTask;
    //     });
    //     await Assert.That(ex!.Message).Contains("more data");
    // }
}
