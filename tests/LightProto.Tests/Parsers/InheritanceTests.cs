namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceTests : BaseProtoBufTests<InheritanceTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Message))]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoInclude(3, typeof(Message))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial record Message : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message { BaseValue = "base", Value = "value" };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);

        await Assert.That(clone is Message).IsTrue();
        await Assert.That(message is Message).IsTrue();

        await Assert.That((clone as Message)!.Value).IsEqualTo((message as Message)!.Value);
    }
}
