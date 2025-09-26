using LightProto.Tests.Parsers;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MultiLevelInheritanceTests : BaseProtoBufTests<MultiLevelInheritanceTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(2, typeof(Base2))]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2, typeof(Base2))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(2, typeof(Message))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(2, typeof(Message))]
    public partial record Base2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base2Value { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract()]
    public partial record Message : Base2
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string MessageValue { get; set; } = "";
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message { BaseValue = "base", Base2Value = "base2", MessageValue = "message" };
        yield return new Base2 { BaseValue = "base", Base2Value = "base2" };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);

        if (message is Message originalMessage && clone is Message clonedMessage)
        {
            await Assert.That(clonedMessage.Base2Value).IsEqualTo(originalMessage.Base2Value);
            await Assert.That(clonedMessage.MessageValue).IsEqualTo(originalMessage.MessageValue);
        }
        else if (message is Base2 originalBase2 && clone is Base2 clonedBase2)
        {
            await Assert.That(clonedBase2.Base2Value).IsEqualTo(originalBase2.Base2Value);
        }
    }
}