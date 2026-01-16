namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceAbstractTests2 : BaseProtoBufTests<InheritanceAbstractTests2.Message>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required Base Content { get; set; }
    }

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

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message2 : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message { Content = new Message1 { Value = Guid.NewGuid().ToString() } };
        yield return new Message { Content = new Message2 { Value = Guid.NewGuid().ToString() } };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Content.GetType()).IsEqualTo(message.Content.GetType());
        if (message.Content is Message1 message1)
        {
            await Assert.That(clone.Content is Message1).IsTrue();
            var cloneMessage = (clone.Content as Message1)!;
            await Assert.That(message1.Value).IsEqualTo(cloneMessage.Value);
        }
        if (message.Content is Message2 message2)
        {
            await Assert.That(clone.Content is Message2).IsTrue();
            var cloneMessage = (Message2)(clone.Content);
            await Assert.That(message2.Value).IsEqualTo(cloneMessage.Value);
        }
    }
}
