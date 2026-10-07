using LightProto;

namespace LightProto.Tests.Parsers;

public partial class OptimizedListCollectionTests
{
    [ProtoContract]
    public partial class Message
    {
        [ProtoMember(1, IsPacked = true)]
        public List<bool> PackedBooleans { get; set; } = [];

        [ProtoMember(2, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        public List<byte> PackedFixedBytes { get; set; } = [];

        [ProtoMember(3)]
        public List<string> UnpackedStrings { get; set; } = [];
    }

    [Test]
    public async Task ListFastPathsRoundTrip()
    {
        var message = new Message
        {
            PackedBooleans = [true, false, true],
            PackedFixedBytes = [1, 255],
            UnpackedStrings = ["first", "second"],
        };

        var parsed = Serializer.Deserialize<Message>(message.ToByteArray());

        await Assert.That(parsed.PackedBooleans).IsEquivalentTo(message.PackedBooleans);
        await Assert.That(parsed.PackedFixedBytes).IsEquivalentTo(message.PackedFixedBytes);
        await Assert.That(parsed.UnpackedStrings).IsEquivalentTo(message.UnpackedStrings);
    }
}
