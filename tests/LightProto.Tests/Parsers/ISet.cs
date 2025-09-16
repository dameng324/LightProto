using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ISetPackedTests : BaseTests<ISetPackedTests.Message, ISetPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public required ISet<int> Property { get; set; } = new HashSet<int>();

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new()
        {
            Property = new HashSet<int>() { 1, 2, 3, 4, 5 },
        };
        yield return new()
        {
            Property = new HashSet<int>() { -1, -2, -3, -4, -5 },
        };
        // TODO:protobuf-net is wrong here  yield return new () { Property = new HashSet<int>(){0} };
        yield return new() { Property = new HashSet<int>() { } };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ISetPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ISetPackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ISetPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
