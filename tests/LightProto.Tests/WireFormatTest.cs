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
}