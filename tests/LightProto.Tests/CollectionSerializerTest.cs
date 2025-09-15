using LightProto.Parser;

namespace LightProto.Tests;

public class CollectionSerializerTest
{
    [Test]
    public async Task IntListTest()
    {
        List<int> list = [1, 2, 3, 4];
        var bytes = list.ToByteArray(Int32ProtoParser.Writer);
        var clone = Serializer.Deserialize<List<int>, int>(bytes.AsSpan(), Int32ProtoParser.Reader);
        await Assert.That(clone).IsEquivalentTo(list);
    }

    [Test]
    public async Task StringListTest()
    {
        List<string> list = ["123", "", "111"];
        var bytes = list.ToByteArray(StringProtoParser.Writer);

        var clone = Serializer.Deserialize<List<string>, string>(
            bytes.AsSpan(),
            StringProtoParser.Reader
        );
        await Assert.That(clone).IsEquivalentTo(list);
    }

    [Test]
    public async Task DictionaryTest()
    {
        var map = new Dictionary<string, int>() { ["a"] = 1, ["b"] = 2 };
        var bytes = map.ToByteArray(StringProtoParser.Writer, Int32ProtoParser.Writer);

        var clone = Serializer.Deserialize<Dictionary<string, int>, string, int>(
            bytes.AsSpan(),
            StringProtoParser.Reader,
            Int32ProtoParser.Reader
        );
        await Assert.That(clone).IsEquivalentTo(map);
    }
}
