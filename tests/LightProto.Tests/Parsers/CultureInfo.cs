using System.Globalization;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class CultureInfoTests : BaseTests<CultureInfoTests.Message, CultureInfoTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public CultureInfo Property { get; set; } = System.Globalization.CultureInfo.InvariantCulture;
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = CultureInfo.InvariantCulture };
        yield return new() { Property = new CultureInfo("en-US") };
        yield return new() { Property = new CultureInfo("zh-CN") };
    }

    public override IEnumerable<CultureInfoTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new CultureInfoTestsMessage() { Property = o.Property.Name });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property);
    }

    public override async Task AssertGoogleResult(CultureInfoTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property.Name);
    }
}
