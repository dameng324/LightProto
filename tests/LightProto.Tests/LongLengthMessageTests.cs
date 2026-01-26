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

    [Test]
    public async Task LongLengthMessageTest()
    {
        var message = new TestMessage { Items = new List<InternalMessage>() };
        for (int i = 0; i < 500; i++)
        {
            message.Items.Add(new InternalMessage { Data = new byte[10_000_000] });
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
            var size = lazyWriter.CalculateLongSize(new Lazy<TestMessage>(() => message));
        });
        Assert.Throws<OverflowException>(() =>
        {
            var lazyWriter = new NullableProtoWriter<TestMessage>(TestMessage.ProtoWriter);
            var size = lazyWriter.CalculateLongSize(message);
        });

        var tempFileName = Path.GetTempFileName();
        try
        {
            using (var fs = File.OpenWrite(tempFileName))
            {
                using var gzip = new GZipStream(fs, CompressionLevel.Fastest, leaveOpen: true);
                Serializer.Serialize(gzip, message, TestMessage.ProtoWriter);
            }

            using (var fs = File.OpenRead(tempFileName))
            {
                using var gzip = new GZipStream(fs, CompressionMode.Decompress, leaveOpen: true);
                var deserialized = Serializer.Deserialize<TestMessage>(gzip, TestMessage.ProtoReader);
                await Assert.That(deserialized.Items.Count).IsEqualTo(message.Items.Count);
            }
        }
        finally
        {
            if (File.Exists(tempFileName))
                File.Delete(tempFileName);
        }
    }
}
