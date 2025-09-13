using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class HashSetPackedTests : BaseTests<HashSetPackedTests.Message>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public HashSet<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }
    public override IEnumerable<Message> GetMessages()
    {
        yield return new Message() { Property = new HashSet<int>(){1, 2, 3, 4, 5} };
        yield return new Message() { Property = new HashSet<int>(){-1, -2, -3, -4, -5} };
        yield return new Message() { Property = new HashSet<int>(){0} };
        yield return new Message() { Property = new HashSet<int>(){} };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }
}
