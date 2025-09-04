using AwesomeAssertions;
using Google.Protobuf;

namespace Dameng.Protobuf.Extension.Tests;

public class NullableTests
{
    [Test]
    public void SimpleNullableTest_ShouldWork()
    {
        var message = new SimpleNullableTestV2
        {
            NullableInt = 42,
            StringField = "test"
        };

        // Test serialization doesn't throw
        var bytes = message.ToByteArray();
        bytes.Should().NotBeNull();

        // Test deserialization
        var deserialized = SimpleNullableTestV2.Parser.ParseFrom(bytes);
        
        // Check values
        deserialized.NullableInt.Should().Be(42);
        deserialized.StringField.Should().Be("test");
    }
}