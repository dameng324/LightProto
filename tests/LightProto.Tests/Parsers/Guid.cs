using System.Buffers.Binary;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class GuidTests : BaseTests<GuidTests.Message, GuidTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Guid Property { get; set; }
    }

    public override IEnumerable<GuidTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = Guid.Empty.ToProtobuf() };
        yield return new() { Property = Guid.NewGuid().ToProtobuf() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(GuidTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToGuid()).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = Guid.Empty };
        yield return new() { Property = Guid.NewGuid() };
    }
}

file static class BclExtension
{
    public static ProtoBuf.Bcl.Guid ToProtobuf(this Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
#if NET7_0_OR_GREATER
        value.TryWriteBytes(bytes);
#else
        value.ToByteArray().AsSpan().CopyTo(bytes);
#endif
        return new ProtoBuf.Bcl.Guid()
        {
            Lo = BinaryPrimitives.ReadUInt64LittleEndian(bytes),
            Hi = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(8)),
        };
    }

    public static Guid ToGuid(this ProtoBuf.Bcl.Guid proxy)
    {
        if (proxy is null)
        {
            return Guid.Empty;
        }
        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, proxy.Lo);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8), proxy.Hi);

#if NET7_0_OR_GREATER
        return new Guid(bytes);
#else
        return new Guid(bytes.ToArray());
#endif
    }
}
