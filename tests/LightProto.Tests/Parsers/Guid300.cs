using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Guid300Tests: BaseTests<Guid300Tests.Message,Guid300TestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        [CompatibilityLevel(CompatibilityLevel.Level300)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level300)]
        public Guid Property { get; set; }
    }

    public override IEnumerable<Guid300TestsMessage> GetGoogleMessages()
    {
        yield return new () { Property = Guid.Empty.ToString() };
        yield return new () { Property = Guid.NewGuid().ToString() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(Guid300TestsMessage clone, Message message)
    {
        await Assert.That(Guid.Parse( clone.Property)).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new () { Property = Guid.Empty };
        yield return new () { Property = Guid.NewGuid() };
    }
}