using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class UriTests : BaseProtoBufTestsWithParser<UriTests.Message, UriTests.Message>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1, ParserType = typeof(UriProtoParser))]
        public Uri? Property { get; set; }
    }

    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = null };
        yield return new() { Property = new Uri("https://example.com") };
        yield return new() { Property = new Uri("https://example.com/path/to/resource") };
        yield return new() { Property = new Uri("https://example.com:8080/path?query=value#fragment") };
        yield return new() { Property = new Uri("file:///home/user/file.txt") };
        yield return new() { Property = new Uri("/relative/path", UriKind.Relative) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property?.OriginalString).IsEquivalentTo(message.Property?.OriginalString);
    }
}
