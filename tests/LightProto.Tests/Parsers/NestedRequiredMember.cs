using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class NestedRequiredMember
    : BaseTests<NestedRequiredMember.Message, NestedRequiredMemberMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required NestedMessage Property { get; set; }
    }

    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record NestedMessage
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required int Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new NestedMessage() { Property = 1 } };
        yield return new() { Property = new NestedMessage() { Property = 0 } };
        yield return new() { Property = new NestedMessage() { Property = -1 } };
        yield return new() { Property = new NestedMessage() { Property = int.MaxValue } };
        yield return new() { Property = new NestedMessage() { Property = int.MinValue } };
    }

    public override IEnumerable<NestedRequiredMemberMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new NestedRequiredMemberMessage()
            {
                Property = new NullableIntTestsMessage() { Property = o.Property.Property },
            });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Property).IsEquivalentTo(message.Property.Property);
    }

    public override async Task AssertGoogleResult(
        NestedRequiredMemberMessage clone,
        Message message
    )
    {
        await Assert.That(clone.Property.Property).IsEquivalentTo(message.Property.Property);
    }
}
