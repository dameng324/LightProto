namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceTests2 : BaseProtoBufTests<InheritanceTests2.Base>
{
    [ProtoContract()]
    [ProtoInclude(3, typeof(Message))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(3, typeof(Message))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    public record Base2 : Base { }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial record Message : Base2
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
