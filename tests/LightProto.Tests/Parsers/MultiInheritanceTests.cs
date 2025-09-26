namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MultiInheritanceTests : BaseProtoBufTests<MultiInheritanceTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Base2))]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3, typeof(Base2))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Base3))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(3, typeof(Base3))]
    public partial record Base2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base2Value { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Base4))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(3, typeof(Base4))]
    public partial record Base3 : Base2
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base3Value { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract()]
    public partial record Base4 : Base3
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base4Value { get; set; } = "";
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Base4
        {
            BaseValue = "base",
            Base2Value = "base2",
            Base3Value = "base3",
            Base4Value = "value",
        };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);
        await Assert.That(clone is Base4).IsTrue();
        await Assert.That(message is Base4).IsTrue();
        await Assert.That((clone as Base4)!.Base4Value).IsEqualTo((message as Base4)!.Base4Value);
    }
}
