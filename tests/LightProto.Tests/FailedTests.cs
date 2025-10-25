#if NET6_0_OR_GREATER
using System.Buffers;
using System.IO.Compression;
using System.Runtime.Intrinsics.X86;
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
            var bytes = strings.ToByteArray(StringProtoParser.ProtoWriter.GetCollectionWriter());
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
            var bytes = strings.ToByteArray(StringProtoParser.ProtoWriter.GetCollectionWriter());
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).IsEqualTo("Sequence contained null element");
    }

    [Test]
    public async Task Sequence_contained_null_element_WhenSerializing3()
    {
        List<string> strings = new() { "one", null!, "three" };
        var ex = await Assert.ThrowsAsync<Exception>(async () =>
        {
            using var ms = new MemoryStream();
            strings.SerializeTo(ms, StringProtoParser.ProtoWriter.GetCollectionWriter());
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).IsEqualTo("Sequence contained null element");
    }

    [Test]
    public async Task Sequence_contained_null_element_WhenSerializing4()
    {
        HashSet<string> strings = new() { "one", null!, "three" };
        var ex = await Assert.ThrowsAsync<Exception>(async () =>
        {
            using var ms = new MemoryStream();
            strings.SerializeTo(ms, StringProtoParser.ProtoWriter.GetCollectionWriter());
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
            var strings = Serializer.Deserialize(
                bytes,
                Int32ProtoParser.ProtoReader.GetCollectionReader<List<int>, int>()
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
            var strings = Serializer.Deserialize(
                bytes,
                Int32ProtoParser.ProtoReader.GetCollectionReader<List<int>, int>()
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
            var strings = Serializer.Deserialize(
                bytes,
                StringProtoParser.ProtoReader.GetCollectionReader<List<string>, string>()
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

    [Test]
    public async Task MalformedVarint_WhenDeserializingWithLengthPrefix()
    {
        var ex = await Assert.ThrowsAsync<InvalidProtocolBufferException>(async () =>
        {
            var bytes = new byte[]
            {
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
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
            };
            var ms = new MemoryStream(bytes);
            var strings = Serializer.DeserializeWithLengthPrefix<MalformedVarint>(
                ms,
                PrefixStyle.Base128
            );
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("malformed varint");
    }

    [ProtoContract]
    public partial class MalformedVarint64
    {
        [ProtoMember(1)]
        public long Value { get; set; }
    }

    [Test]
    public async Task MalformedVarint64_WhenDeserializing()
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
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
            };
            var strings = Serializer.Deserialize<MalformedVarint64>(bytes);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("malformed varint");
    }

    [ProtoContract]
    public partial class MalformedVarUint64
    {
        [ProtoMember(1)]
        public ulong Value { get; set; }
    }

    [Test]
    public async Task MalformedVarUint64_WhenDeserializing()
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
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
            };
            var strings = Serializer.Deserialize<MalformedVarUint64>(bytes);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("malformed varint");
    }

    [Test]
    public async Task Invalid_ticks_for_MINMAX_scale_WhenDeserializingDateTime()
    {
        //var bytes =DateTime.MaxValue.ToByteArray(DateTimeProtoParser.ProtoWriter);//8,2,16,15
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            var bytes = new byte[] { 8, 3, 16, 15 };
            Serializer.Deserialize<DateTime>(bytes, DateTimeProtoParser.ProtoReader);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("Invalid ticks for MINMAX scale");
    }

    [Test]
    public async Task Unknown_scale_WhenDeserializingDateTime()
    {
        //var bytes =DateTime.MaxValue.ToByteArray(DateTimeProtoParser.ProtoWriter);//8,2,16,15
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            var bytes = new byte[] { 8, 2, 16, 14 };
            Serializer.Deserialize<DateTime>(bytes, DateTimeProtoParser.ProtoReader);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("Unknown scale");
    }

    [Test]
    public async Task Invalid_ticks_for_MINMAX_scale_WhenDeserializingTimeSpan()
    {
        //var bytes =TimeSpan.MaxValue.ToByteArray(TimeSpanProtoParser.ProtoWriter);//8,2,16,15
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            var bytes = new byte[] { 8, 3, 16, 15 };
            Serializer.Deserialize<TimeSpan>(bytes, TimeSpanProtoParser.ProtoReader);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("Invalid ticks for MINMAX scale");
    }

    [Test]
    public async Task Unknown_scale_WhenDeserializingTimeSpan()
    {
        //var bytes =TimeSpan.MaxValue.ToByteArray(TimeSpanProtoParser.ProtoWriter);//8,2,16,15
        var ex = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            var bytes = new byte[] { 8, 2, 16, 14 };
            Serializer.Deserialize<TimeSpan>(bytes, TimeSpanProtoParser.ProtoReader);
            await Task.CompletedTask;
        });
        await Assert.That(ex!.Message).Contains("Unknown scale");
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

    [Test]
    public async Task Test_TriggersLargeSizeSlowPath()
    {
        var original = Enumerable.Range(0, 1000000).Select(i => (byte)i).ToArray();

        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, mode: CompressionMode.Compress, leaveOpen: true))
            original.SerializeTo(gzip, ByteArrayProtoParser.ProtoWriter);

        ms.Position = 0;
        using var deZip = new GZipStream(ms, mode: CompressionMode.Decompress, leaveOpen: true);
        var parsed = Serializer.Deserialize<byte[]>(deZip, ByteArrayProtoParser.ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }

    [Test]
    public async Task OutOfSpaceExceptionTest()
    {
        var ex = Assert.Throws<CodedOutputStream.OutOfSpaceException>(() =>
        {
            var message = "1234567";
            var writer = StringProtoParser.ProtoWriter;
            var buffer = new byte[writer.CalculateSize(message) - 1];
            using CodedOutputStream output = new CodedOutputStream(buffer);
            WriterContext.Initialize(output, out var ctx);
            writer.WriteTo(ref ctx, message);
            ctx.Flush();
        });
        await Assert.That(ex!.Message).Contains("ran out of space");
    }
}
#endif
