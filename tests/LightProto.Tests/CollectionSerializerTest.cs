using LightProto.Parser;

namespace LightProto.Tests;

public class CollectionSerializerTest
{
    [Test]
    public async Task DictionaryTest()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        var bytes = map.ToByteArray(StringProtoParser.ProtoWriter, Int32ProtoParser.ProtoWriter);

        var clone = Serializer.Deserialize<Dictionary<string, int>, string, int>(
            bytes.AsSpan(),
            StringProtoParser.ProtoReader,
            Int32ProtoParser.ProtoReader
        );
        await Assert.That(clone).IsEquivalentTo(map);
    }
}
