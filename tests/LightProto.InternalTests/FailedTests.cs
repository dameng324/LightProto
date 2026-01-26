using System.Buffers;
using LightProto.InternalTests;
using LightProto.Parser;

namespace LightProto.Tests;

public partial class FailedTests
{
    [Test]
    public async Task OutOfSpaceExceptionTest()
    {
        var ex = Assert.Throws<CodedOutputStream.OutOfSpaceException>(() =>
        {
            var message = "1234567";
            var writer = StringProtoParser.ProtoWriter;
            var buffer = new byte[writer.CalculateLongSize(message) - 1];
            using CodedOutputStream output = new CodedOutputStream(buffer);
            WriterContext.Initialize(output, out var ctx);
            writer.WriteTo(ref ctx, message);
            ctx.Flush();
        });
        await Assert.That(ex!.Message).Contains("ran out of space");
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
}
