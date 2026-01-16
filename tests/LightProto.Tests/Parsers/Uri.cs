using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class UriTests : BaseProtoBufTestsWithParser<UriTests.Message, UriTests.Message>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
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
        await Assert.That(clone.Property?.OriginalString).IsEqualTo(message.Property?.OriginalString);
    }

    [Test]
    public async Task InvalidUri_DeserializesToNull()
    {
        // Manually create bytes that would represent an invalid URI string
        // The format is: field1 (tag=10, length-delimited string), length, invalid-uri-bytes
        // Tag for field 1 with wire type 2 (LengthDelimited) = (1 << 3) | 2 = 10
        // An invalid URI format that Uri.TryCreate would reject
        // "http://[invalid" is rejected by Uri.TryCreate
        var invalidUri = "https://[invalid"u8;
        byte[] invalidUriBytes = [10, (byte)invalidUri.Length, .. invalidUri];
        var result = Serializer.Deserialize(invalidUriBytes, UriTests.Message.ProtoReader);
        await Assert.That(result.Property).IsNull();
    }
}
