namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceAbstractTests : BaseProtoBufTests<InheritanceAbstractTests.Base>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(Message1))]
    [ProtoBuf.ProtoInclude(3, typeof(Message1))]
    [ProtoInclude(4, typeof(Message2))]
    [ProtoBuf.ProtoInclude(4, typeof(Message2))]
    public abstract partial class Base { }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message1 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    [ProtoContract()]
    [ProtoBuf.ProtoContract()]
    public partial class Message2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public int IntValue { get; set; }
    }

    public override IEnumerable<Base> GetMessages()
    {
        yield return new Message1 { Value = Guid.NewGuid().ToString() };
        yield return new Message2 { Value = Guid.NewGuid().ToString() };
    }

    public override async Task AssertResult(Base clone, Base message)
    {
        await Assert.That(clone.GetType()).IsEqualTo(message.GetType());
        if (message is Message1 message1)
        {
            await Assert.That(clone is Message1).IsTrue();
            var cloneMessage = (clone as Message1)!;
            await Assert.That(message1.Value).IsEqualTo(cloneMessage.Value);
        }
        if (message is Message2 message2)
        {
            await Assert.That(clone is Message2).IsTrue();
            var cloneMessage = (Message2)(clone);
            await Assert.That(message2.Value).IsEqualTo(cloneMessage.Value);
            await Assert.That(message2.IntValue).IsEqualTo(cloneMessage.IntValue);
        }
    }
}
