namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceInterfaceTests2
    : BaseProtoBufTests<InheritanceInterfaceTests2.Message>
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial class Message
    {
        [ProtoMember(1)]
        public required Base Content { get; set; }
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    [ProtoInclude(3, typeof(StructMessage))]
    [ProtoBuf.ProtoInclude(3, typeof(StructMessage))]
    [ProtoInclude(4, typeof(ClassMessage))]
    [ProtoBuf.ProtoInclude(4, typeof(ClassMessage))]
    public partial interface Base { }

    public partial class BaseProtoParser { }

    [ProtoContract(SkipConstructor = true)]
    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public partial record ClassMessage : Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string Value { get; set; } = "";
    }

    [ProtoContract()]
    [ProtoBuf.ProtoContract()]
    public partial record struct StructMessage : Base
    {
        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public string Value { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message()
        {
            Content = new ClassMessage { Value = Guid.NewGuid().ToString() },
        };
        yield return new Message()
        {
            Content = new StructMessage { Value = Guid.NewGuid().ToString() },
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        if (message.Content is ClassMessage classMessage)
        {
            await Assert.That(clone.Content is ClassMessage).IsTrue();
            var cloneMessage = (clone.Content as ClassMessage)!;
            await Assert.That(classMessage.Value).IsEqualTo(cloneMessage.Value);
        }
        if (message.Content is StructMessage structMessage)
        {
            await Assert.That(clone.Content is StructMessage).IsTrue();
            var cloneMessage = (StructMessage)(clone.Content);
            await Assert.That(structMessage.Value).IsEqualTo(cloneMessage.Value);
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;
    protected override bool ProtoBuf_net_Deserialize_Disabled => true;
}
