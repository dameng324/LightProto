using System.IO.Compression;
using LightProto.Parser;

namespace LightProto.Tests;

public partial class LongLengthMessageTests
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public partial struct TestMessage
    {
        public List<InternalMessage> Items { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public partial class InternalMessage
    {
        public byte[] Data { get; set; } = [];
    }

#if NET10_0 // only for .NET 10 to avoid out-of-memory in CI
    [Test]
    public async Task LongLengthMessageTest()
    {
        int count = 1100_000;
        var message = new TestMessage { Items = new List<InternalMessage>() };
        for (int i = 0; i < count; i++)
        {
            message.Items.Add(new InternalMessage { Data = new byte[4096] });
        }

        var longSize = TestMessage.ProtoWriter.CalculateLongSize(message);
        await Assert.That(longSize).IsGreaterThan(int.MaxValue);

        Assert.Throws<OverflowException>(() =>
        {
            var bytes = message.ToByteArray(TestMessage.ProtoWriter);
        });

        Assert.Throws<OverflowException>(() =>
        {
            var cloned = Serializer.DeepClone(message, TestMessage.ProtoReader, TestMessage.ProtoWriter);
        });

        Assert.Throws<OverflowException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix(ms, message, PrefixStyle.Fixed32, TestMessage.ProtoWriter);
        });
        Assert.Throws<OverflowException>(() =>
        {
            var ms = new MemoryStream();
            Serializer.SerializeWithLengthPrefix(ms, message, PrefixStyle.Fixed32BigEndian, TestMessage.ProtoWriter);
        });
        Assert.Throws<OverflowException>(() =>
        {
            var lazyWriter = new LazyProtoWriter<TestMessage>(TestMessage.ProtoWriter);
            var size = lazyWriter.CalculateSize(new Lazy<TestMessage>(() => message));
        });
        Assert.Throws<OverflowException>(() =>
        {
            var lazyWriter = new NullableProtoWriter<TestMessage>(TestMessage.ProtoWriter);
            var size = lazyWriter.CalculateSize(message);
        });
        Assert.Throws<OverflowException>(() =>
        {
            var runtimeProtoWriter = new RuntimeProtoWriter<TestMessage>();
            runtimeProtoWriter.AddMember(1, x => x.Items, InternalMessage.ProtoWriter.GetCollectionWriter());
            var runtimeSize = runtimeProtoWriter.CalculateSize(message);
        });
        Assert.Throws<OverflowException>(() =>
        {
            var size = TestMessage.ProtoWriter.GetCollectionWriter().CalculateSize([message]);
        });
        Assert.Throws<OverflowException>(() =>
        {
            var keyValuePairWriter = new KeyValuePairProtoWriter<int, TestMessage>(Int32ProtoParser.ProtoWriter, TestMessage.ProtoWriter);
            var size = keyValuePairWriter.CalculateSize(new KeyValuePair<int, TestMessage>(1, message));
        });

        var tempFileName = Path.GetTempFileName();
        try
        {
            using (var fs = File.OpenWrite(tempFileName))
            {
                using var gzip = new GZipStream(fs, CompressionLevel.Fastest, leaveOpen: true);
                Serializer.Serialize(gzip, message, TestMessage.ProtoWriter);
            }

            GC.Collect();
            using (var fs = File.OpenRead(tempFileName))
            {
                using var gzip = new GZipStream(fs, CompressionMode.Decompress, leaveOpen: true);
                var deserialized = Serializer.Deserialize<TestMessage>(gzip, TestMessage.ProtoReader);
                await Assert.That(deserialized.Items.Count).IsEqualTo(count);
            }
        }
        finally
        {
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);
        }
    }
#endif
}
