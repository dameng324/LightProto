#if NET5_0_OR_GREATER
using System.Buffers.Binary;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class HalfTests : BaseTests<HalfTests.Message, HalfTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Half Property { get; set; }
    }

    public override IEnumerable<HalfTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(x => new HalfTestsMessage() { Property = (float)x.Property });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = (Half)0.0f };
        yield return new() { Property = (Half)1.0f };
        yield return new() { Property = (Half)(-1.0f) };
        yield return new() { Property = (Half)3.14159f };
        yield return new() { Property = (Half)100.5f };
        yield return new() { Property = (Half)(-42.75f) };
        yield return new() { Property = Half.MaxValue };
        yield return new() { Property = Half.MinValue };
        yield return new() { Property = Half.Epsilon };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(HalfTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo((float)message.Property);
    }
}
#endif
