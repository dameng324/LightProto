﻿using Google.Protobuf.Collections;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ListPackedTests : BaseTests<ListPackedTests.Message, ListPackedTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, IsPacked = true)]
        [ProtoBuf.ProtoMember(1, IsPacked = true)]
        public List<int> Property { get; set; } = [];

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
        yield return new() { Property = [0] };
        yield return new() { Property = [] };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override IEnumerable<ListPackedTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new ListPackedTestsMessage() { Property = { o.Property } });
    }

    public override async Task AssertGoogleResult(ListPackedTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }
}
