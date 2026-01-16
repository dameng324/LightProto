using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class FixedSizePackedListTests : BaseTests<FixedSizePackedListTests.Message, FixedSizeArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, DataFormat = ProtoBuf.DataFormat.FixedSize, IsPacked = true)]
        public List<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new()
        {
            Property = new() { 1, 2, 3, 4, 5 },
        };
        yield return new()
        {
            Property = new() { -1, -2, -3, -4, -5 },
        };
        yield return new()
        {
            Property = new() { 0, 0, 0, 0, 0 },
        };
        //yield return new() { Property = new int[] { 0 } }; // protobuf-net is wrong here
        yield return new() { Property = [] };
    }

    public override IEnumerable<FixedSizeArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new FixedSizeArrayTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(FixedSizeArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
