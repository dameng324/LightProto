using System.Collections.Immutable;
using System.Collections.ObjectModel;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ImmutableHashSetTests
    : BaseTests<ImmutableHashSetTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ImmutableHashSet<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = [1, 2, 3, 4, 5] };
        yield return new() { Property = [-1, -2, -3, -4, -5] };
        yield return new() { Property = [0, 0, 0, 0, 0] };
        yield return new() { Property = [0] };
        yield return new() { Property = [] };
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
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(
            bytes,
            new ImmutableHashSetProtoReader<int>(Int32ProtoParser.ProtoReader, 0)
        );
        await Assert.That(deserialized.Count).IsEqualTo(0);
    }
}
