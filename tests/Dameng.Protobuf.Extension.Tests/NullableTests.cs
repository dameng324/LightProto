using AwesomeAssertions;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension.Tests;

public class NullableTests
{
    [Test]
    public void NullableTypeSerialization_ShouldHandleNullValues()
    {
        var message = new NullableTestMessage
        {
            NullableIntField = null,
            NullableBoolField = true,
            NullableDoubleField = null,
            NullableFloatField = 3.14f,
            NullableLongField = null,
            StringField = "test"
        };

        // Test serialization doesn't throw
        var bytes = message.ToByteArray();
        bytes.Should().NotBeNull();

        // Test deserialization
        var deserialized = NullableTestMessage.Parser.ParseFrom(bytes);
        
        // Check values
        deserialized.NullableIntField.Should().BeNull();
        deserialized.NullableBoolField.Should().Be(true);
        deserialized.NullableDoubleField.Should().BeNull();
        deserialized.NullableFloatField.Should().Be(3.14f);
        deserialized.NullableLongField.Should().BeNull();
        deserialized.StringField.Should().Be("test");
    }

    [Test]
    public void NullableTypeSerialization_ShouldHandleNonNullValues()
    {
        var message = new NullableTestMessage
        {
            NullableIntField = 42,
            NullableBoolField = false,
            NullableDoubleField = 2.71,
            NullableFloatField = null,
            NullableLongField = 9999L,
            StringField = null
        };

        // Test round-trip serialization
        var bytes = message.ToByteArray();
        var deserialized = NullableTestMessage.Parser.ParseFrom(bytes);
        
        // Check values
        deserialized.NullableIntField.Should().Be(42);
        deserialized.NullableBoolField.Should().Be(false);
        deserialized.NullableDoubleField.Should().Be(2.71);
        deserialized.NullableFloatField.Should().BeNull();
        deserialized.NullableLongField.Should().Be(9999L);
        deserialized.StringField.Should().Be("");  // String defaults to empty, not null in protobuf
    }

    [Test]
    public void NullableTypeEquality_ShouldWork()
    {
        var message1 = new NullableTestMessage
        {
            NullableIntField = 42,
            NullableBoolField = null,
            NullableDoubleField = 2.71,
            StringField = "test"
        };

        var message2 = new NullableTestMessage
        {
            NullableIntField = 42,
            NullableBoolField = null,
            NullableDoubleField = 2.71,
            StringField = "test"
        };

        message1.Equals(message2).Should().BeTrue();
        message1.GetHashCode().Should().Be(message2.GetHashCode());
    }
}