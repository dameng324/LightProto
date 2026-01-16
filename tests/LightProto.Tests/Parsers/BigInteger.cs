using System.Numerics;
using Google.Protobuf;
using LightProto;

namespace LightProto.Tests.Parsers;

#if NET5_0_OR_GREATER
[InheritsTests]
public partial class BigIntegerTests : BaseTests<BigIntegerTests.Message, BigIntegerTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public BigInteger Property { get; set; }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = BigInteger.Zero };
        yield return new() { Property = BigInteger.Parse("1111111111111111111111111111111111111111111111111111111111111111") };
        yield return new() { Property = BigInteger.Parse("-1111111111111111111111111111111111111111111111111111111111111111") };
    }

    public override IEnumerable<BigIntegerTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(m => new BigIntegerTestsMessage { Property = ByteString.CopyFrom(m.Property.ToByteArray()) });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property);
    }

    public override async Task AssertGoogleResult(BigIntegerTestsMessage clone, Message message)
    {
        await Assert.That(new BigInteger(clone.Property.ToByteArray())).IsEqualTo(message.Property);
    }
}
#endif
