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

        [ProtoMember(4, IsPacked = true)]
        public Queue<bool> PackedBooleanQueue { get; set; } = new();

        [ProtoMember(5, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        public Queue<byte> PackedFixedByteQueue { get; set; } = new();

        [ProtoMember(6)]
        public Queue<string> UnpackedStringQueue { get; set; } = new();

        [ProtoMember(7, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        public List<int> PackedFixedIntegers { get; set; } = [];

        [ProtoMember(8, IsPacked = true)]
        public List<int> PackedVarintIntegers { get; set; } = [];

        [ProtoMember(9, IsPacked = true)]
        public Queue<int> PackedVarintIntegerQueue { get; set; } = new();
    }

    [Test]
    public async Task ListAndQueueFastPathsRoundTrip()
    {
        var message = new Message
        {
            PackedBooleans = [true, false, true],
            PackedFixedBytes = [1, 255],
            UnpackedStrings = ["first", "second"],
            PackedBooleanQueue = new Queue<bool>([true, false, true]),
            PackedFixedByteQueue = new Queue<byte>([1, 255]),
            UnpackedStringQueue = new Queue<string>(["first", "second"]),
            PackedFixedIntegers = [1, -100, int.MaxValue],
            PackedVarintIntegers = [1, -100, int.MaxValue],
            PackedVarintIntegerQueue = new Queue<int>([1, -100, int.MaxValue]),
        };

        var parsed = Serializer.Deserialize(message.ToByteArray(Message.ProtoWriter), Message.ProtoReader);

        await Assert.That(parsed.PackedBooleans).IsEquivalentTo(message.PackedBooleans);
        await Assert.That(parsed.PackedFixedBytes).IsEquivalentTo(message.PackedFixedBytes);
        await Assert.That(parsed.UnpackedStrings).IsEquivalentTo(message.UnpackedStrings);
        await Assert.That(parsed.PackedBooleanQueue).IsEquivalentTo(message.PackedBooleanQueue);
        await Assert.That(parsed.PackedFixedByteQueue).IsEquivalentTo(message.PackedFixedByteQueue);
        await Assert.That(parsed.UnpackedStringQueue).IsEquivalentTo(message.UnpackedStringQueue);
        await Assert.That(parsed.PackedFixedIntegers).IsEquivalentTo(message.PackedFixedIntegers);
        await Assert.That(parsed.PackedVarintIntegers).IsEquivalentTo(message.PackedVarintIntegers);
        await Assert.That(parsed.PackedVarintIntegerQueue).IsEquivalentTo(message.PackedVarintIntegerQueue);
    }
}
