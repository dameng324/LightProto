using LightProto;

namespace LightProto.Tests.Parsers;

#if NET5_0_OR_GREATER
[InheritsTests]
public partial class Int128Tests : BaseTests<Int128Tests.Message, Int128TestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public Int128 Property { get; set; }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = Int128.MinValue };
        yield return new() { Property = Int128.MaxValue };
        yield return new() { Property = Int128.Zero };
        yield return new() { Property = 1111111111111111111 };
        yield return new() { Property = -1111111111111111111 };
    }

    public override IEnumerable<Int128TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(m => new Int128TestsMessage
            {
                Property = new Int128Message()
                {
                    Lower = (ulong)m.Property,
                    Upper = (ulong)(m.Property >> 64),
                },
            });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property);
    }

    public override async Task AssertGoogleResult(Int128TestsMessage clone, Message message)
    {
        clone.Property ??= new Int128Message();
        await Assert
            .That(new Int128(clone.Property.Upper, clone.Property.Lower))
            .IsEqualTo(message.Property);
    }
}
#endif
