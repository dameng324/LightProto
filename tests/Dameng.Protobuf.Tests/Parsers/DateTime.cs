namespace Dameng.Protobuf.Tests.Parsers;
[SkipAot]
public partial class DateTimeTests
{
    [Dameng.Protobuf.ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [Dameng.Protobuf.ProtoMember(1, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, IsPacked = true)]
        public DateTime Property { get; set; }
    }

    public static IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = DateTime.MinValue };
        yield return new Message() { Property = DateTime.MaxValue };
        yield return new Message() { Property = DateTime.UtcNow };
        yield return new Message() { Property = DateTime.Now };
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task SerializeAndDeserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = Dameng.Protobuf.Serializer.Deserialize<Message>(data);
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Unspecified);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Serialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = Dameng.Protobuf.Serializer.Deserialize<Message>(ms.ToArray());
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Unspecified);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Deserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = ProtoBuf.Serializer.Deserialize<Message>(data);
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Unspecified);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_SerializeAndDeserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = ProtoBuf.Serializer.Deserialize<Message>(ms.ToArray());
        await Assert.That(clone.Property.Ticks).IsEquivalentTo(message.Property.Ticks);
        await Assert.That(clone.Property.Kind).IsEquivalentTo(DateTimeKind.Unspecified);
    }
}