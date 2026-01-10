namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MultiInheritanceTests2 : BaseProtoBufTests<MultiInheritanceTests2.Message>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required Base Content { get; set; }
    }

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

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message { Content = new Base { BaseValue = "base" } };
        yield return new Message
        {
            Content = new Base2 { BaseValue = "base", Base2Value = "base2" },
        };
        yield return new Message
        {
            Content = new Base3
            {
                BaseValue = "base",
                Base2Value = "base2",
                Base3Value = "base3",
            },
        };
        yield return new Message
        {
            Content = new Base4
            {
                BaseValue = "base",
                Base2Value = "base2",
                Base3Value = "base3",
                Base4Value = "value",
            },
        };
    }

    public override async Task AssertResult(Message cloneObj, Message messageObj)
    {
        var clone = cloneObj.Content;
        var message = messageObj.Content;

        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);
        await Assert.That(clone.GetType() == message.GetType()).IsTrue();

        if (clone is Base4)
        {
            await Assert
                .That((clone as Base4)!.Base4Value)
                .IsEqualTo((message as Base4)!.Base4Value);
        }

        if (clone is Base3)
        {
            await Assert
                .That((clone as Base3)!.Base3Value)
                .IsEqualTo((message as Base3)!.Base3Value);
        }

        if (clone is Base2)
        {
            await Assert
                .That((clone as Base2)!.Base2Value)
                .IsEqualTo((message as Base2)!.Base2Value);
        }
    }
}
