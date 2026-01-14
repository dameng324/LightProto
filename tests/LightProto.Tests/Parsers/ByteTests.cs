using LightProto;

namespace LightProto.Tests.Parsers;

public partial class ByteTests
{
    [ProtoContract]
    public partial class ByteMessage
    {
        [ProtoMember(1)]
        public byte Data { get; set; }

        [ProtoMember(2)]
        public byte MinValue { get; set; }

        [ProtoMember(3)]
        public byte MaxValue { get; set; }
    }

    [Test]
    public async Task ByteSerializationTest()
    {
        var message = new ByteMessage
        {
            Data = 42,
            MinValue = byte.MinValue,
            MaxValue = byte.MaxValue,
        };

        // Serialize
        var bytes = message.ToByteArray(ByteMessage.ProtoWriter);

        // Deserialize
        var deserialized = Serializer.Deserialize(bytes, ByteMessage.ProtoReader);

        // Assert
        await Assert.That(deserialized.Data).IsEqualTo(message.Data);
        await Assert.That(deserialized.MinValue).IsEqualTo(message.MinValue);
        await Assert.That(deserialized.MaxValue).IsEqualTo(message.MaxValue);
    }

    [Test]
    public async Task ByteRoundTripTest()
    {
        // Test all byte values from 0 to 255
        for (int i = 0; i <= 255; i++)
        {
            var message = new ByteMessage { Data = (byte)i };
            var bytes = message.ToByteArray(ByteMessage.ProtoWriter);
            var deserialized = Serializer.Deserialize(bytes, ByteMessage.ProtoReader);
            await Assert.That(deserialized.Data).IsEqualTo((byte)i);
        }
    }
}
