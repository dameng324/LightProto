namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class InheritanceInterfaceTests : BaseProtoBufTests<InheritanceInterfaceTests.Base>
{
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

    public override IEnumerable<Base> GetMessages()
    {
        yield return new ClassMessage { Value = Guid.NewGuid().ToString() };
        yield return new StructMessage { Value = Guid.NewGuid().ToString() };
    }

    protected override bool ProtoBuf_net_Serialize_Disabled => true;
    protected override bool ProtoBuf_net_Deserialize_Disabled => true;

    public override async Task AssertResult(Base clone, Base message)
    {
        if (message is ClassMessage classMessage)
        {
            await Assert.That(clone is ClassMessage).IsTrue();
            var cloneMessage = (clone as ClassMessage)!;
            await Assert.That(classMessage.Value).IsEqualTo(cloneMessage.Value);
        }
        if (message is StructMessage structMessage)
        {
            await Assert.That(clone is StructMessage).IsTrue();
            var cloneMessage = (StructMessage)(clone);
            await Assert.That(structMessage.Value).IsEqualTo(cloneMessage.Value);
        }
    }
}
