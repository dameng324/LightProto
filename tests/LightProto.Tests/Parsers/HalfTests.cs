using LightProto;

namespace LightProto.Tests.Parsers;

public partial class HalfTests
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Half HalfValue { get; set; }
    }

    [Test]
    public async Task WhenSerializingHalf_ThenCanDeserialize()
    {
        var message = new Message { HalfValue = (Half)3.14f };
        var bytes = message.ToByteArray(ProtoParser<Message>.ProtoWriter);
        var clone = Serializer.Deserialize(bytes, ProtoParser<Message>.ProtoReader);
        
        await Assert.That(clone.HalfValue).IsEquivalentTo(message.HalfValue);
    }

    [Test]
    public async Task WhenSerializingMultipleHalfValues_ThenCanDeserializeAll()
    {
        var testValues = new[]
        {
            (Half)0.0f,
            (Half)1.0f,
            (Half)(-1.0f),
            (Half)3.14159f,
            (Half)100.5f,
            (Half)(-42.75f),
            Half.MaxValue,
            Half.MinValue,
            Half.Epsilon
        };

        foreach (var testValue in testValues)
        {
            var message = new Message { HalfValue = testValue };
            var bytes = message.ToByteArray(ProtoParser<Message>.ProtoWriter);
            var clone = Serializer.Deserialize(bytes, ProtoParser<Message>.ProtoReader);
            
            await Assert.That(clone.HalfValue).IsEquivalentTo(message.HalfValue);
        }
    }

    [Test]
    public async Task WhenHalfIsSerializedAsFloat_ThenDeserializesCorrectly()
    {
        var message = new Message { HalfValue = (Half)5.5f };
        var bytes = message.ToByteArray(ProtoParser<Message>.ProtoWriter);
        
        await Assert.That(bytes.Length).IsGreaterThan(0);
        
        var clone = Serializer.Deserialize(bytes, ProtoParser<Message>.ProtoReader);
        await Assert.That(clone.HalfValue).IsEquivalentTo((Half)5.5f);
    }
}
