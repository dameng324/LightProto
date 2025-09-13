namespace LightProto.Tests.Parsers;

public abstract class BaseTests<Message>
    where Message : IProtoMessage<Message>
{
    public abstract IEnumerable<Message> GetMessages();

    public abstract Task AssertResult(Message clone, Message message);
    
    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task SerializeAndDeserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = Serializer.Deserialize<Message>(data);
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = Serializer.Deserialize<Message>(ms.ToArray());
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Deserialize(Message message)
    {
        byte[] data = message.ToByteArray();
        var clone = ProtoBuf.Serializer.Deserialize<Message>(data.AsSpan());
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_SerializeAndDeserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize<Message>(ms, message);
        ms.Position = 0;
        var clone = ProtoBuf.Serializer.Deserialize<Message>(ms.ToArray().AsSpan());
        await AssertResult(clone, message);
    }
}
