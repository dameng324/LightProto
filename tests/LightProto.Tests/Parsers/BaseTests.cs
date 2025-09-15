using System.Diagnostics.CodeAnalysis;
using Google.Protobuf;

namespace LightProto.Tests.Parsers;

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
[InheritsTests]
public abstract class BaseTests<Message, GoogleProtobufMessage> : BaseProtoBufTests<Message>
    where Message : IProtoMessage<Message>
    where GoogleProtobufMessage : IMessage<GoogleProtobufMessage>, new()
{
    public abstract IEnumerable<GoogleProtobufMessage> GetGoogleMessages();

    public abstract Task AssertGoogleResult(GoogleProtobufMessage clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_LightProto_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertGoogleResult(google, clone);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_GoogleProto_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var googleClone = new GoogleProtobufMessage();
        googleClone.MergeFrom(bytes);
        await AssertGoogleResult(googleClone, message);
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
public abstract class BaseProtoBufTests<Message>
    where Message : IProtoMessage<Message>
{
    public abstract IEnumerable<Message> GetMessages();

    public abstract Task AssertResult(Message clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_LightProto_Deserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task LightProto_Serialize_ProtoBuf_net_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_Deserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        TestContext.Current!.WriteLine($"bytes: {string.Join(",",bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertResult(clone, message);
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
public abstract class BaseEquivalentTypeTests<LightProtoMessage, ProtoNetMessage>
    where LightProtoMessage : IProtoMessage<LightProtoMessage>
{
    public abstract IEnumerable<LightProtoMessage> GetLightProtoMessages();
    public abstract IEnumerable<ProtoNetMessage> GetProtoNetMessages();

    public abstract Task AssertResult(LightProtoMessage clone, ProtoNetMessage message);

    [Test]
    [MethodDataSource(nameof(GetProtoNetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_LightProto_Deserialize(ProtoNetMessage message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var clone = Serializer.Deserialize<LightProtoMessage>(ms.ToArray());
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetLightProtoMessages))]
    [SkipAot]
    public async Task LightProto_Serialize_ProtoBuf_net_Deserialize(LightProtoMessage message)
    {
        byte[] data = message.ToByteArray();
        var clone = ProtoBuf.Serializer.Deserialize<ProtoNetMessage>(data.AsSpan());
        await AssertResult(message, clone);
    }
}
