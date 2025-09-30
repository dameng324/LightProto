using System.Diagnostics.CodeAnalysis;
using Google.Protobuf;

namespace LightProto.Tests.Parsers;

public static class BaseTestsConfig
{
    public static bool WriteDebugInfo = false;
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
[InheritsTests]
public abstract class BaseTests<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Message,
    GoogleProtobufMessage
> : BaseProtoBufTests<Message>
    where Message : IProtoParser<Message>
    where GoogleProtobufMessage : IMessage<GoogleProtobufMessage>, new()
{
    public abstract IEnumerable<GoogleProtobufMessage> GetGoogleMessages();

    public abstract Task AssertGoogleResult(GoogleProtobufMessage clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_LightProto_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"GoogleProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertGoogleResult(google, clone);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_GoogleProto_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        var googleClone = new GoogleProtobufMessage();
        googleClone.MergeFrom(bytes);
        await AssertGoogleResult(googleClone, message);
    }

    [Test]
    [SkipAot]
    [MethodDataSource(nameof(GetMessages))]
    public async Task ProtoBuf_net_Serialize_GoogleProto_Deserialize(Message message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        var googleClone = new GoogleProtobufMessage();
        googleClone.MergeFrom(bytes);
        await AssertGoogleResult(googleClone, message);
    }

    [Test]
    [SkipAot]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_ProtoBuf_net_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"GoogleProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertGoogleResult(google, clone);
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
[InheritsTests]
public abstract class BaseGoogleProtobufTests<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Message,
    GoogleProtobufMessage
>
    where Message : IProtoParser<Message>
    where GoogleProtobufMessage : IMessage<GoogleProtobufMessage>, new()
{
    public abstract IEnumerable<GoogleProtobufMessage> GetGoogleMessages();

    public abstract IEnumerable<Message> GetMessages();
    public abstract Task AssertGoogleResult(GoogleProtobufMessage clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_LightProto_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertGoogleResult(google, clone);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_GoogleProto_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"bytes: {string.Join(",", bytes)}");
        var googleClone = new GoogleProtobufMessage();
        googleClone.MergeFrom(bytes);
        await AssertGoogleResult(googleClone, message);
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
[InheritsTests]
public abstract class BaseProtoBufTests<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Message
> : BaseProtoBufTestsWithParser<Message, Message>
    where Message : IProtoParser<Message> { }

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
public abstract class BaseProtoBufTestsWithParser<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Message,
    Parser
>
    where Parser : IProtoParser<Message>
{
    public abstract IEnumerable<Message> GetMessages();

    public abstract Task AssertResult(Message clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray(Parser.ProtoWriter);
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes, Parser.ProtoReader);
        await AssertResult(clone, message);
    }

    protected virtual bool ProtoBuf_net_Serialize_LightProto_Deserialize_Disabled => false;

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_LightProto_Deserialize(Message message)
    {
        if (ProtoBuf_net_Serialize_LightProto_Deserialize_Disabled)
        {
            return;
        }
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes, Parser.ProtoReader);
        await AssertResult(clone, message);
    }

    protected virtual bool LightProto_Serialize_ProtoBuf_net_Deserialize_Disabled => false;

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task LightProto_Serialize_ProtoBuf_net_Deserialize(Message message)
    {
        if (LightProto_Serialize_ProtoBuf_net_Deserialize_Disabled)
            return;
        byte[] bytes = message.ToByteArray(Parser.ProtoWriter);
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertResult(clone, message);
    }

    protected virtual bool ProtoBuf_net_Serialize_Deserialize_Disabled => false;

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_Deserialize(Message message)
    {
        if (ProtoBuf_net_Serialize_Deserialize_Disabled)
            return;
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertResult(clone, message);
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
public abstract class BaseEquivalentTypeTests<
    LightProtoMessage,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] ProtoNetMessage
>
    where LightProtoMessage : IProtoParser<LightProtoMessage>
{
    public abstract IEnumerable<LightProtoMessage> GetLightProtoMessages();
    public abstract IEnumerable<ProtoNetMessage> GetProtoNetMessages();

    public abstract Task AssertResult(
        LightProtoMessage lightProto,
        ProtoNetMessage protobuf,
        bool lightProtoToProtoBuf
    );

    [Test]
    [MethodDataSource(nameof(GetProtoNetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_LightProto_Deserialize(ProtoNetMessage message)
    {
        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var clone = Serializer.Deserialize<LightProtoMessage>(ms.ToArray());
        await AssertResult(clone, message, false);
    }

    [Test]
    [MethodDataSource(nameof(GetLightProtoMessages))]
    [SkipAot]
    public async Task LightProto_Serialize_ProtoBuf_net_Deserialize(LightProtoMessage message)
    {
        byte[] data = message.ToByteArray();
        var clone = ProtoBuf.Serializer.Deserialize<ProtoNetMessage>(data.AsSpan());
        await AssertResult(message, clone, true);
    }
}
