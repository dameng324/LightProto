#if NET8_0_OR_GREATER
using System.Collections.Frozen;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class FrozenSetTests : BaseTests<FrozenSetTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public FrozenSet<int> Property { get; set; } = FrozenSet<int>.Empty;

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new[] { 1, 2, 3, 4, 5 }.ToFrozenSet() };
        yield return new() { Property = new[] { -1, -2, -3, -4, -5 }.ToFrozenSet() };
        yield return new() { Property = new[] { 0, 1, 2, 3, 4 }.ToFrozenSet() };
        yield return new() { Property = new[] { 0 }.ToFrozenSet() };
        yield return new() { Property = FrozenSet<int>.Empty };
    }

    public override IEnumerable<ArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ArrayTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That((ISet<int>)clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(bytes, new FrozenSetProtoReader<int>(Int32ProtoParser.ProtoReader, 0));
        await Assert.That(deserialized.Count).IsEqualTo(0);
    }
}
#endif
