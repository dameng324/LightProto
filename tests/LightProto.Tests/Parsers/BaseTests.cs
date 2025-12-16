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
    where GoogleProtobufMessage : IMessage<GoogleProtobufMessage>, new()
{
    public abstract IEnumerable<GoogleProtobufMessage> GetGoogleMessages();

    public abstract Task AssertGoogleResult(GoogleProtobufMessage clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_LightProto_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteString().ToByteArray();
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
        if (ProtoBuf_net_Serialize_Disabled)
            return;

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
        if (ProtoBuf_net_Deserialize_Disabled)
            return;

        var bytes = google.ToByteString().ToByteArray();
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
    where GoogleProtobufMessage : IMessage<GoogleProtobufMessage>, new()
{
    public abstract IEnumerable<GoogleProtobufMessage> GetGoogleMessages();

    public abstract IEnumerable<Message> GetMessages();
    public abstract Task AssertGoogleResult(GoogleProtobufMessage clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetGoogleMessages))]
    public async Task GoogleProto_Serialize_LightProto_Deserialize(GoogleProtobufMessage google)
    {
        var bytes = google.ToByteString().ToByteArray();
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
public abstract class BaseProtoBufTests<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Message
>
{
    public abstract IEnumerable<Message> GetMessages();

    public abstract Task AssertResult(Message clone, Message message);

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    public async Task LightProto_Serialize_Deserialize(Message message)
    {
        byte[] bytes = message.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertResult(clone, message);
    }

    protected virtual bool ProtoBuf_net_Serialize_Disabled => false;
    protected virtual bool ProtoBuf_net_Deserialize_Disabled => false;

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_LightProto_Deserialize(Message message)
    {
        if (ProtoBuf_net_Serialize_Disabled)
        {
            return;
        }

        var ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, message);
        ms.Position = 0;
        var bytes = ms.ToArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        var clone = Serializer.Deserialize<Message>(bytes);
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task LightProto_Serialize_ProtoBuf_net_Deserialize(Message message)
    {
        if (ProtoBuf_net_Deserialize_Disabled)
            return;
        byte[] bytes = message.ToByteArray();
        if (BaseTestsConfig.WriteDebugInfo)
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        var clone = ProtoBuf.Serializer.Deserialize<Message>(bytes.AsSpan());
        await AssertResult(clone, message);
    }

    [Test]
    [MethodDataSource(nameof(GetMessages))]
    [SkipAot]
    public async Task ProtoBuf_net_Serialize_Deserialize(Message message)
    {
        if (ProtoBuf_net_Deserialize_Disabled || ProtoBuf_net_Serialize_Disabled)
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

    public IEnumerable<
        Func<(PrefixStyle style, int fieldNumber, Message[] messages)>
    > GetLengthPrefixMessages()
    {
        foreach (
            var style in new[]
            {
                PrefixStyle.Base128,
                PrefixStyle.Fixed32,
                PrefixStyle.Fixed32BigEndian,
            }
        )
        {
            var messages = GetMessages().ToArray();
            foreach (var fieldNumber in new[] { 0, 1, 1000, 1000_000, 100_000_000 })
            {
                yield return () => (style, fieldNumber, messages);
            }
        }
    }

    [Test]
    [MethodDataSource(nameof(GetLengthPrefixMessages))]
    [SkipAot]
    public async Task LengthPrefix_ProtoBuf_net_Serialize_LightProto_Deserialize(
        PrefixStyle style,
        int fieldNumber,
        Message[] messages
    )
    {
        if (ProtoBuf_net_Serialize_Disabled)
        {
            return;
        }

        using var ms = new MemoryStream();
        foreach (var message in messages)
        {
            ProtoBuf.Serializer.SerializeWithLengthPrefix(
                ms,
                message,
                (ProtoBuf.PrefixStyle)style,
                fieldNumber
            );
        }
        if (BaseTestsConfig.WriteDebugInfo)
        {
            var bytes = ms.ToArray();
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        }

        ms.Position = 0;
        List<Message> clones = Serializer
            .DeserializeItems<Message>(ms, style, fieldNumber)
            .ToList();
        await Assert.That(clones.Count).IsEqualTo(messages.Length);
        for (int i = 0; i < clones.Count; i++)
        {
            await AssertResult(clones[i], messages[i]);
        }
    }

    [Test]
    [MethodDataSource(nameof(GetLengthPrefixMessages))]
    [SkipAot]
    public async Task LengthPrefix_LightProto_Serialize_ProtoBuf_net_Deserialize(
        PrefixStyle style,
        int fieldNumber,
        Message[] messages
    )
    {
        if (ProtoBuf_net_Deserialize_Disabled)
            return;
        using var ms = new MemoryStream();
        foreach (var message in messages)
            Serializer.SerializeWithLengthPrefix(ms, message, style, fieldNumber);

        if (BaseTestsConfig.WriteDebugInfo)
        {
            var bytes = ms.ToArray();
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        }

        ms.Position = 0;

        List<Message> clones = ProtoBuf
            .Serializer.DeserializeItems<Message>(ms, (ProtoBuf.PrefixStyle)style, fieldNumber)
            .ToList();
        await Assert.That(clones.Count).IsEqualTo(messages.Length);
        for (int i = 0; i < clones.Count; i++)
        {
            await AssertResult(clones[i], messages[i]);
        }
    }

    [Test]
    [MethodDataSource(nameof(GetLengthPrefixMessages))]
    public async Task LengthPrefix_LightProto_Serialize_LightProto_Deserialize(
        PrefixStyle style,
        int fieldNumber,
        Message[] messages
    )
    {
        using var ms = new MemoryStream();
        foreach (var message in messages)
            Serializer.SerializeWithLengthPrefix<Message>(ms, message, style, fieldNumber);
        if (BaseTestsConfig.WriteDebugInfo)
        {
            var bytes = ms.ToArray();
            Console.WriteLine($"ProtoBuf_net_Serialize bytes: {string.Join(",", bytes)}");
        }

        ms.Position = 0;
        List<Message> clones = Serializer
            .DeserializeItems<Message>(ms, style, fieldNumber)
            .ToList();
        await Assert.That(clones.Count).IsEqualTo(messages.Length);
        for (int i = 0; i < clones.Count; i++)
        {
            await AssertResult(clones[i], messages[i]);
        }
    }

    [Test]
    [MethodDataSource(nameof(GetLengthPrefixMessages))]
    [SkipAot]
    public async Task LengthPrefix_ProtoBuf_net_Serialize_ProtoBuf_net_Deserialize(
        PrefixStyle style,
        int fieldNumber,
        Message[] messages
    )
    {
        if (ProtoBuf_net_Serialize_Disabled || ProtoBuf_net_Deserialize_Disabled)
            return;
        using var ms = new MemoryStream();
        foreach (var message in messages)
        {
            ProtoBuf.Serializer.SerializeWithLengthPrefix(
                ms,
                message,
                (ProtoBuf.PrefixStyle)style,
                fieldNumber
            );
        }

        if (BaseTestsConfig.WriteDebugInfo)
        {
            var bytes = ms.ToArray();
            Console.WriteLine($"LightProto_Serialize bytes: {string.Join(",", bytes)}");
        }

        ms.Position = 0;
        List<Message> clones = ProtoBuf
            .Serializer.DeserializeItems<Message>(ms, (ProtoBuf.PrefixStyle)style, fieldNumber)
            .ToList();

        await Assert.That(clones.Count).IsEqualTo(messages.Length);
        for (int i = 0; i < clones.Count; i++)
        {
            await AssertResult(clones[i], messages[i]);
        }
    }
}

[SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
public abstract class BaseEquivalentTypeTests<
    LightProtoMessage,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] ProtoNetMessage
>
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
