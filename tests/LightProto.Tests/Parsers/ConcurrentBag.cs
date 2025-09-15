using System.Collections.Concurrent;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ConcurrentBagPackedTests : BaseTests<ConcurrentBagPackedTests.Message,ConcurrentBagPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ConcurrentBag<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }
    public override IEnumerable<Message> GetMessages()
    {
        yield return new () { Property = new ConcurrentBag<int>(){1, 2, 3, 4, 5} };
        yield return new () { Property = new ConcurrentBag<int>(){-1, -2, -3, -4, -5} };
        yield return new () { Property = new ConcurrentBag<int>(){0} };
        yield return new () { Property = new ConcurrentBag<int>(){} };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.OrderBy(x=>x).ToArray()).IsEquivalentTo(message.Property.OrderBy(x=>x).ToArray());
    }
    public override IEnumerable<ConcurrentBagPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o=>new ConcurrentBagPackedTestsMessage()
        {
            Property = { o.Property }
        });
    }
    public override async Task AssertGoogleResult(ConcurrentBagPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.OrderBy(x=>x).ToArray()).IsEquivalentTo(message.Property.OrderBy(x=>x).ToArray());
    }
}
