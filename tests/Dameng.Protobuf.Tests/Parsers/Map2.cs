namespace Dameng.Protobuf.Tests.Parsers;

public partial class Map2Tests
{
    [Dameng.Protobuf.ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [Dameng.Protobuf.ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Dictionary<int, Dictionary<int, string>> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {Property.Count}";
        }
    }

    public static IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = new () };
        yield return new Message() { Property = new  ()
        {
            [10] = new Dictionary<int, string>()
            {
                [1] = "one",
                // [2] = "two",
                // [3] = "three"
            },
            // [2] =new Dictionary<int, string>()
            // {
            //     [1] = "one",
            //     [2] = "two",
            //     [3] = "three"
            // },
            // [3] = new Dictionary<int, string>()
            // {
            //     [1] = "one",
            //     [2] = "two",
            //     [3] = "three"
            // }
        } };
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task SerializeAndDeserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = Dameng.Protobuf.Serializer.Deserialize<Message>(data);
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Serialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = Dameng.Protobuf.Serializer.Deserialize<Message>(ms.ToArray());
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Deserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = ProtoBuf.Serializer.Deserialize<Message>(data);
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_SerializeAndDeserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = ProtoBuf.Serializer.Deserialize<Message>(ms.ToArray());
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}