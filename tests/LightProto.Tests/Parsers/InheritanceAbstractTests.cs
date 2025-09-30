namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceAbstractTests : BaseProtoBufTests<InheritanceAbstractTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Message))]
    [ProtoBuf.ProtoInclude(3, typeof(Message))]
    [ProtoInclude(4, typeof(Message2))]
    [ProtoBuf.ProtoInclude(4, typeof(Message2))]
    public abstract partial class Base { }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message { Value = "value" };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.GetType()).IsEqualTo(message.GetType());
        await Assert.That(clone is Message).IsTrue();
        await Assert.That(message is Message).IsTrue();
        await Assert.That((clone as Message)!.Value).IsEqualTo((message as Message)!.Value);
    }
}
