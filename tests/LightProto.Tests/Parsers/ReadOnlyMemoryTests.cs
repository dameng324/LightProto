using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ReadOnlyMemoryTests : BaseTests<ReadOnlyMemoryTests.Message, ArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ReadOnlyMemory<int> Property { get; set; } = ReadOnlyMemory<int>.Empty;

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
        yield return new() { Property = ReadOnlyMemory<int>.Empty };
    }

    public override IEnumerable<ArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new ArrayTestsMessage() { Property = { o.Property.ToArray() } });
    }

    public override async Task AssertGoogleResult(ArrayTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property.ToArray());
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(bytes, new ReadOnlyMemoryProtoReader<int>(Int32ProtoParser.ProtoReader, 0));
        await Assert.That(deserialized.IsEmpty).IsTrue();
    }
}
