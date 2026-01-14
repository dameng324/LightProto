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
        for (byte i = 0; i < 255; i++)
        {
            var message = new ByteMessage { Data = i };
            var bytes = message.ToByteArray(ByteMessage.ProtoWriter);
            var deserialized = Serializer.Deserialize(bytes, ByteMessage.ProtoReader);
            await Assert.That(deserialized.Data).IsEqualTo(i);
        }

        // Test max value (255)
        var maxMessage = new ByteMessage { Data = 255 };
        var maxBytes = maxMessage.ToByteArray(ByteMessage.ProtoWriter);
        var maxDeserialized = Serializer.Deserialize(maxBytes, ByteMessage.ProtoReader);
        await Assert.That(maxDeserialized.Data).IsEqualTo((byte)255);
    }
}
