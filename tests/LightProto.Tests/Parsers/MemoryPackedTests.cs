using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class MemoryPackedTests : BaseTests<MemoryPackedTests.Message, ListPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, IsPacked = true)]
        public Memory<int> Property { get; set; } = Memory<int>.Empty;

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property.ToArray())}";
        }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new int[] { 1, 2, 3, 4, 5 } };
        yield return new() { Property = new int[] { -1, -2, -3, -4, -5 } };
        yield return new() { Property = new int[] { 0, 0, 0, 0, 0 } };
        yield return new() { Property = new int[] { 0 } };
        yield return new() { Property = Memory<int>.Empty };
    }

    public override IEnumerable<ListPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ListPackedTestsMessage() { Property = { o.Property.ToArray() } });
    }

    public override async Task AssertGoogleResult(ListPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property.ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
