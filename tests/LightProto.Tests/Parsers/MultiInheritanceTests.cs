namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MultiInheritanceTests : BaseProtoBufTests<MultiInheritanceTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(Base2))]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(100, typeof(Base2))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public required string BaseValue2 { get; set; } = "";

#if NET8_0_OR_GREATER
        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public string BaseValue3 { get; init; } = "";

        [ProtoMember(4)]
        [ProtoBuf.ProtoMember(4)]
        public int BaseValue4 { get; init; }
#endif
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(200, typeof(Base3))]
    [ProtoBuf.ProtoContract()]
    [ProtoBuf.ProtoInclude(200, typeof(Base3))]
    public partial record Base2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base2Value { get; set; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public required string Base2Value2 { get; set; } = "";

#if NET8_0_OR_GREATER
        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public string Base2Value3 { get; init; } = "";

        [ProtoMember(4)]
        [ProtoBuf.ProtoMember(4)]
        public int Base2Value4 { get; init; }
#endif
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(300, typeof(Base4))]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoInclude(300, typeof(Base4))]
    public partial record Base3 : Base2
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base3Value { get; set; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public required string Base3Value2 { get; set; } = "";

#if NET8_0_OR_GREATER
        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public string Base3Value3 { get; init; } = "";

        [ProtoMember(4)]
        [ProtoBuf.ProtoMember(4)]
        public int Base3Value4 { get; init; }
#endif
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract()]
    public partial record Base4 : Base3
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Base4Value { get; set; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public required string Base4Value2 { get; set; } = "";

#if NET8_0_OR_GREATER
        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public string Base4Value3 { get; init; } = "";

        [ProtoMember(4)]
        [ProtoBuf.ProtoMember(4)]
        public int Base4Value4 { get; init; }
#endif
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Base
        {
            BaseValue = "base",
            BaseValue2 = Guid.NewGuid().ToString(),
#if NET8_0_OR_GREATER
            BaseValue3 = Guid.NewGuid().ToString(),
            BaseValue4 = 42,
#endif
        };
        yield return new Base2
        {
            BaseValue = "base",
            BaseValue2 = Guid.NewGuid().ToString(),
            Base2Value = "base2",
            Base2Value2 = Guid.NewGuid().ToString(),
#if NET8_0_OR_GREATER
            BaseValue3 = Guid.NewGuid().ToString(),
            BaseValue4 = 42,
            Base2Value3 = Guid.NewGuid().ToString(),
            Base2Value4 = 84,
#endif
        };
        yield return new Base3
        {
            BaseValue = "base",
            BaseValue2 = Guid.NewGuid().ToString(),
            Base2Value = "base2",
            Base2Value2 = Guid.NewGuid().ToString(),
            Base3Value = "base3",
            Base3Value2 = Guid.NewGuid().ToString(),
#if NET8_0_OR_GREATER
            BaseValue3 = Guid.NewGuid().ToString(),
            BaseValue4 = 42,
            Base2Value3 = Guid.NewGuid().ToString(),
            Base2Value4 = 84,
            Base3Value3 = Guid.NewGuid().ToString(),
            Base3Value4 = 168,
#endif
        };
        yield return new Base4
        {
            BaseValue = "base",
            BaseValue2 = Guid.NewGuid().ToString(),
            Base2Value = "base2",
            Base2Value2 = Guid.NewGuid().ToString(),
            Base3Value = "base3",
            Base3Value2 = Guid.NewGuid().ToString(),
            Base4Value = "base4",
            Base4Value2 = Guid.NewGuid().ToString(),
#if NET8_0_OR_GREATER
            BaseValue3 = Guid.NewGuid().ToString(),
            BaseValue4 = 42,
            Base2Value3 = Guid.NewGuid().ToString(),
            Base2Value4 = 84,
            Base3Value3 = Guid.NewGuid().ToString(),
            Base3Value4 = 168,
            Base4Value3 = Guid.NewGuid().ToString(),
            Base4Value4 = 336,
#endif
        };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);
        await Assert.That(clone.GetType() == message.GetType()).IsTrue();

        if (clone is Base4 clone4)
        {
            var message4 = (message as Base4)!;
            await Assert.That(clone4.Base4Value).IsEqualTo(message4.Base4Value);
            await Assert.That(clone4.Base4Value2).IsEqualTo(message4.Base4Value2);

#if NET8_0_OR_GREATER
            await Assert.That(clone4.Base4Value3).IsEqualTo(message4.Base4Value3);
            await Assert.That(clone4.Base4Value4).IsEqualTo(message4.Base4Value4);
#endif
        }

        if (clone is Base3 clone3)
        {
            var message3 = (message as Base3)!;
            await Assert.That(clone3.Base3Value).IsEqualTo(message3.Base3Value);
            await Assert.That(clone3.Base3Value2).IsEqualTo(message3.Base3Value2);
#if NET8_0_OR_GREATER
            await Assert.That(clone3.Base3Value3).IsEqualTo(message3.Base3Value3);
            await Assert.That(clone3.Base3Value4).IsEqualTo(message3.Base3Value4);
#endif
        }

        if (clone is Base2 clone2)
        {
            var message2 = (message as Base2)!;
            await Assert.That(clone2.Base2Value).IsEqualTo(message2.Base2Value);
            await Assert.That(clone2.Base2Value2).IsEqualTo(message2.Base2Value2);
#if NET8_0_OR_GREATER
            await Assert.That(clone2.Base2Value3).IsEqualTo(message2.Base2Value3);
            await Assert.That(clone2.Base2Value4).IsEqualTo(message2.Base2Value4);
#endif
        }

        if (clone is Base2 clone1)
        {
            var message1 = (message as Base)!;
            await Assert.That(clone1.BaseValue).IsEqualTo(message1.BaseValue);
            await Assert.That(clone1.BaseValue2).IsEqualTo(message1.BaseValue2);
#if NET8_0_OR_GREATER
            await Assert.That(clone1.BaseValue3).IsEqualTo(message1.BaseValue3);
            await Assert.That(clone1.BaseValue4).IsEqualTo(message1.BaseValue4);
#endif
        }
    }
}
