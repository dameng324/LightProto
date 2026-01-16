using LightProto.Parser;

namespace LightProto.Tests;

public class WireFormatTest
{
    [Test]
    public async Task GetTagWireType()
    {
        uint tag = 8;
        var wireType = WireFormat.GetTagWireType(tag);
        await Assert.That(wireType).IsEqualTo(WireFormat.WireType.Varint);
    }

    [Test]
    public async Task KeyValuePairParserShouldBeMessage()
    {
        var reader = new KeyValuePairProtoReader<int, int>(Int32ProtoParser.ProtoReader, Int32ProtoParser.ProtoReader);
        await Assert.That(reader.IsMessage).IsTrue();
        await Assert.That(reader.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);

        var writer = new KeyValuePairProtoWriter<int, int>(Int32ProtoParser.ProtoWriter, Int32ProtoParser.ProtoWriter);
        await Assert.That(writer.IsMessage).IsTrue();
        await Assert.That(writer.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);
    }

    [Test]
    public async Task EnumerableParserShouldNotBeMessage()
    {
        var reader = new ListProtoReader<int>(Int32ProtoParser.ProtoReader, 10, 0);
        await Assert.That(reader.IsMessage).IsFalse();
        await Assert.That(reader.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);

        var writer = new ListProtoWriter<int>(Int32ProtoParser.ProtoWriter, 10, 0);
        await Assert.That(writer.IsMessage).IsFalse();
        await Assert.That(writer.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);
    }

    [Test]
    public async Task ArrayParserShouldNotBeMessage()
    {
        var reader = new ArrayProtoReader<int>(Int32ProtoParser.ProtoReader, 10, 0);
        await Assert.That(reader.IsMessage).IsFalse();
        await Assert.That(reader.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);

        var writer = new ArrayProtoWriter<int>(Int32ProtoParser.ProtoWriter, 10, 0);
        await Assert.That(writer.IsMessage).IsFalse();
        await Assert.That(writer.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);
    }

    [Test]
    public async Task CollectionMessageParserShouldNotBeMessage()
    {
        var reader = Int32ProtoParser.ProtoReader.GetListReader();
        await Assert.That(reader.IsMessage).IsFalse();
        await Assert.That(reader.WireType).IsEqualTo(WireFormat.WireType.LengthDelimited);
    }

#if NET6_0_OR_GREATER
    [Test]
    public async Task DateOnlyTimeOnlyParserShouldNotBeMessage()
    {
        await Assert.That(DateOnlyProtoParser.ProtoReader.IsMessage).IsFalse();
        await Assert.That(DateOnlyProtoParser.ProtoReader.WireType).IsEqualTo(WireFormat.WireType.Varint);
        await Assert.That(DateOnlyProtoParser.ProtoWriter.IsMessage).IsFalse();
        await Assert.That(DateOnlyProtoParser.ProtoWriter.WireType).IsEqualTo(WireFormat.WireType.Varint);

        await Assert.That(TimeOnlyProtoParser.ProtoReader.IsMessage).IsFalse();
        await Assert.That(TimeOnlyProtoParser.ProtoReader.WireType).IsEqualTo(WireFormat.WireType.Varint);
        await Assert.That(TimeOnlyProtoParser.ProtoWriter.IsMessage).IsFalse();
        await Assert.That(TimeOnlyProtoParser.ProtoWriter.WireType).IsEqualTo(WireFormat.WireType.Varint);
    }
#endif
}
