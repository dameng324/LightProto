namespace LightProto.Tests;

[ProtoContract]
public partial class MultiPartialTestMessage
{
    [ProtoMember(1)]
    public int Part1 { get; set; }
}

partial class MultiPartialTestMessage
{
    [ProtoMember(2)]
    public string Part2 { get; set; } = "";
}

public class MultiPartialTest
{
    [Test]
    public async Task MultiPartialClass_Should_SerializeAndDeserializeCorrectly()
    {
        var message = new MultiPartialTestMessage() { Part1 = 42, Part2 = "Hello, World!" };

#if NET7_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            MultiPartialTestMessage.ProtoReader,
            MultiPartialTestMessage.ProtoWriter
        );
#endif
        await Assert.That(cloned.Part1).IsEqualTo(message.Part1);
        await Assert.That(cloned.Part2).IsEqualTo(message.Part2);
    }
}
