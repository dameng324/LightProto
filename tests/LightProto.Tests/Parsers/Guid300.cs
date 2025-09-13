using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Guid300Tests: BaseTests<Guid300Tests.Message>
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
    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = Guid.Empty };
        yield return new Message() { Property = Guid.NewGuid() };
    }
}