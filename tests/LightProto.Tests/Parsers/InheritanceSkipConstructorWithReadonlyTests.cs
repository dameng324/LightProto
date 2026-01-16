namespace LightProto.Tests.Parsers;

#if NET8_0_OR_GREATER
[InheritsTests]
public partial class InheritanceSkipConstructorWithReadonlyTests : BaseProtoBufTests<InheritanceSkipConstructorWithReadonlyTests.Base>
{
    // protobuf-net doesn't support SkipConstructor on abstract classes with readonly members
    protected override bool ProtoBuf_net_Serialize_Disabled => true;
    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Message1))]
    [ProtoBuf.ProtoInclude(3, typeof(Message1))]
    [ProtoInclude(4, typeof(Message2))]
    [ProtoBuf.ProtoInclude(4, typeof(Message2))]
    public abstract partial class Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message1 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; } = "InitialValue";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public readonly int ReadonlyField = 100;
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; } = "InitialValue2";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public readonly int ReadonlyField = 200;
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message1 { BaseValue = "base1" };
        yield return new Message2 { BaseValue = "base2" };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.GetType()).IsEqualTo(message.GetType());
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);

        if (message is Message1 message1)
        {
            await Assert.That(clone is Message1).IsTrue();
            var cloneMessage = (clone as Message1)!;
            await Assert.That(message1.Value).IsEqualTo(cloneMessage.Value);
            await Assert.That(message1.ReadonlyField).IsEqualTo(cloneMessage.ReadonlyField);
        }

        if (message is Message2 message2)
        {
            await Assert.That(clone is Message2).IsTrue();
            var cloneMessage = (clone as Message2)!;
            await Assert.That(message2.Value).IsEqualTo(cloneMessage.Value);
            await Assert.That(message2.ReadonlyField).IsEqualTo(cloneMessage.ReadonlyField);
        }
    }
}
#endif
