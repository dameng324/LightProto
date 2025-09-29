namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InterfaceInheritanceTests : BaseProtoBufTests<InterfaceInheritanceTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Message))]
    [ProtoBuf.ProtoInclude(3, typeof(Message))]
    [ProtoInclude(4, typeof(Message))]
    [ProtoBuf.ProtoInclude(4, typeof(Message))]
    public partial interface Base { }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial record Message : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial struct StructMessage : Base
    {
        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public string Value { get; set; }
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message { Value = "value" };
        yield return new StructMessage { Value = "value" };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.GetType()).IsEqualTo(message.GetType());
        await Assert.That(clone is Message).IsTrue();
        await Assert.That(message is Message).IsTrue();
        await Assert.That((clone as Message)!.Value).IsEqualTo((message as Message)!.Value);
    }
}
