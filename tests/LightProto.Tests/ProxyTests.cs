namespace LightProto.Tests;

public partial class ProxyTests
{
    [Test]
    public async Task ProxyTest()
    {
        ProtoProxy testObj = new() { Instrument = Instrument.FromNameValue("hello", 20) };

        var bytes = testObj.ToByteArray(ProtoProxy.ProtoWriter);
        var parsed = Serializer.Deserialize<ProtoProxy>(bytes, ProtoProxy.ProtoReader);
        await Assert.That(parsed.Instrument.Name).IsEqualTo(testObj.Instrument.Name);
        await Assert.That(parsed.Instrument.Value).IsEqualTo(testObj.Instrument.Value);
        //parsed.GetHashCode()await Assert.That().IsEqualTo(testObj.GetHashCode());
    }

    [ProtoContract]
    public partial class ProtoProxy
    {
        [ProtoMember(1)]
        public Instrument Instrument { get; set; } = null!;
    }
}

public class Instrument
{
    public static Instrument FromNameValue(string name, int value) =>
        new Instrument { Name = name, Value = value };

    private Instrument() { }

    public required string Name { get; set; }
    public required int Value { get; set; }
}
