using LightProto;

namespace LightProto.Tests.Parsers;
[SkipAot]
public partial class ArrayTests
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial class Message
    {
        [ProtoMember(1, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, IsPacked = true)]
        public int[] Property { get; set; } = [];
    }
    public static IEnumerable<Message> GetMessages()
    {
        yield return new Message(){ Property = new int[] { 1, 2, 3, 4, 5 } };
        yield return new Message(){ Property = new int[] { -1, -2, -3, -4, -5 } };
        yield return new Message(){ Property = new int[] { 0, 0, 0, 0, 0 } };
        yield return new Message(){ Property = [] };
    }
    
    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task SerializeAndDeserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = Serializer.Deserialize<Message>(data);
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Serialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = Serializer.Deserialize<Message>(ms.ToArray());
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
}
