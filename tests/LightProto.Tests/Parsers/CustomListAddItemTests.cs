using LightProto.Parser;

namespace LightProto.Tests.Parsers;

public class CustomListAddItemTests
{
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task CustomListReaderHonorsAddItem(bool packed)
    {
        int calls = 0;
        var reader = new IEnumerableProtoReader<List<int>, int>(
            Int32ProtoParser.ProtoReader,
            capacity => new List<int>(capacity),
            (collection, item) =>
            {
                calls++;
                collection.Add(item + 1);
                return collection;
            },
            itemFixedSize: 0
        );
        byte[] bytes = packed ? [10, 2, 1, 2] : [8, 1, 8, 2];

        var parsed = Serializer.Deserialize(bytes, reader);

        await Assert.That(calls).IsEqualTo(2);
        await Assert.That(parsed).IsEquivalentTo(new[] { 2, 3 });
    }

    [Test]
    public async Task CustomFixedWidthListReaderHonorsAddItem()
    {
        int calls = 0;
        var reader = new IEnumerableProtoReader<List<double>, double>(
            DoubleProtoParser.ProtoReader,
            capacity => new List<double>(capacity),
            (collection, item) =>
            {
                calls++;
                collection.Add(item + 1);
                return collection;
            },
            itemFixedSize: 8
        );
        byte[] bytes = [10, 16, .. BitConverter.GetBytes(1d), .. BitConverter.GetBytes(2d)];

        var parsed = Serializer.Deserialize(bytes, reader);

        await Assert.That(calls).IsEqualTo(2);
        await Assert.That(parsed).IsEquivalentTo(new[] { 2d, 3d });
    }
}
