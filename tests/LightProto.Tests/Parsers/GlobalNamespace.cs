using LightProto;
using LightProto.Tests.Parsers;

[ProtoContract]
[ProtoBuf.ProtoContract]
// ReSharper disable once CheckNamespace
public partial struct GlobalNamespaceMessage
{
    [ProtoMember(1)]
    [ProtoBuf.ProtoMember(1)]
    public string Property { get; set; }
}

[InheritsTests]
public partial class GlobalNamespaceTests : BaseTests<GlobalNamespaceMessage, StructTestsMessage>
{
    public override IEnumerable<GlobalNamespaceMessage> GetMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override IEnumerable<StructTestsMessage> GetGoogleMessages()
    {
        yield return new() { Property = string.Empty };
        yield return new() { Property = Guid.NewGuid().ToString("N") };
    }

    public override async Task AssertResult(GlobalNamespaceMessage clone, GlobalNamespaceMessage message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(StructTestsMessage clone, GlobalNamespaceMessage message)
    {
        await Assert.That(clone.Property ?? string.Empty).IsEquivalentTo(message.Property ?? string.Empty);
    }
}
