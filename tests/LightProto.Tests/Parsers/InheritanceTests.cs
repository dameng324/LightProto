namespace LightProto.Tests.Parsers;

[InheritsTests]
[Explicit]
//TODO re-enable when inheritance is supported
public partial class InheritanceTests: BaseTests<InheritanceTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2, typeof(Message))]
    public partial record Base
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message : Base
    {
        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public string Value { get; set; } = "";
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message
        {
            BaseValue = "base",
            Value = "value"
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.BaseValue).IsEqualTo(message.BaseValue);
        //await Assert.That(clone.Value).IsEqualTo(message.Value);
    }
}