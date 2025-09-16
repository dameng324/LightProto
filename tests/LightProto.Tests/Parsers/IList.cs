﻿using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class IListPackedTests : BaseTests<IListPackedTests.Message, IListPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public IList<int> Property { get; set; } = [];

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property)}";
        }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = [1, 2, 3, 4, 5] };
        yield return new() { Property = [-1, -2, -3, -4, -5] };
        yield return new() { Property = [0, 0, 0, 0, 0] };
        // TODO:protobuf-net is wrong here  yield return new () { Property = [0] };
        yield return new() { Property = [] };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<IListPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new IListPackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(IListPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
