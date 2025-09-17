using System.Buffers;

namespace LightProto.Tests;

public partial class IBufferWriterTests
{
    [ProtoContract]
    public partial class TestContract
    {
        [ProtoMember(1)]
        public string Name { get; set; } = string.Empty;
    }

    [Test]
    public async Task TestBufferWriter()
    {
        ArrayBufferWriter<byte> bufferWriter = new();
        var obj = new TestContract() { Name = Guid.NewGuid().ToString() };
        Serializer.Serialize(bufferWriter, obj);
        var data = bufferWriter.WrittenSpan.ToArray();
        var parsed = Serializer.Deserialize<TestContract>(data);
        await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
    }
}
