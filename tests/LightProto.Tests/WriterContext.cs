namespace LightProto.Tests;

public class WriterContextTest
{
    [Test]
    public void WriteLengthTest()
    {
        ByteArrayPoolBufferWriter writer = new ByteArrayPoolBufferWriter();
        var ctx = new LightProto.WriterContext(writer);
    }
}
