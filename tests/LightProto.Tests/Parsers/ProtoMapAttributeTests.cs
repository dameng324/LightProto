using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ProtoMapAttributeTests
    : BaseTests<ProtoMapAttributeTests.Message, ProtoMapAttributeTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoMap(KeyFormat = DataFormat.ZigZag, ValueFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(1)]
        [ProtoBuf.ProtoMap(
            KeyFormat = ProtoBuf.DataFormat.ZigZag,
            ValueFormat = ProtoBuf.DataFormat.FixedSize
        )]
        public Dictionary<int, long> Property { get; set; } = [];

        public override string ToString()
        {
            return string.Join(",", Property.Select(x => $"{x.Key}:{x.Value}"));
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Dictionary<int, long>() };
        yield return new()
        {
            Property = new Dictionary<int, long>()
            {
                [1] = 10,
                [2] = 20,
                [3] = 30,
            },
        };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ProtoMapAttributeTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ProtoMapAttributeTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(
        ProtoMapAttributeTestsMessage clone,
        Message message
    )
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
