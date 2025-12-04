using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class VersionTests : BaseTests<VersionTests.Message, VersionTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required Version Property { get; set; }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Version() };
        yield return new() { Property = new Version(1, 2) };
        yield return new() { Property = new Version(1, 2, 3) };
        yield return new() { Property = new Version(1, 2, 3, 4) };
    }

    public override IEnumerable<VersionTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new VersionTestsMessage
            {
                Property = new BclVersion()
                {
                    Major = o.Property.Major,
                    Minor = o.Property.Minor,
                    Build = o.Property.Build,
                    Revision = o.Property.Revision,
                },
            });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(VersionTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.Major).IsEquivalentTo(message.Property.Major);
        await Assert.That(clone.Property.Minor).IsEquivalentTo(message.Property.Minor);
        await Assert.That(clone.Property.Build).IsEquivalentTo(message.Property.Build);
        await Assert.That(clone.Property.Revision).IsEquivalentTo(message.Property.Revision);
    }
}
