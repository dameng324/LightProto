namespace LightProto.Tests;

public partial class Proxy2Tests
{
    [Test]
    public async Task ProxyTest()
    {
        ProtoProxy testObj = new() { Instrument = Instrument.FromNameValue("hello", 20) };

        var bytes = testObj.ToByteArray(ProtoProxy.ProtoWriter);
        var parsed = Serializer.Deserialize<ProtoProxy>(bytes, ProtoProxy.ProtoReader);
        await Assert.That(parsed.Instrument.Name).IsEqualTo(testObj.Instrument.Name);
        await Assert.That(parsed.Instrument.Value).IsEqualTo(testObj.Instrument.Value);
    }

    [ProtoContract]
    public partial class ProtoProxy
    {
        [ProtoMember(1)]
        public Instrument Instrument { get; set; } = null!;
    }

    [ProtoParserType(typeof(InstrumentProtoParser))]
    public class Instrument
    {
        public static Instrument FromNameValue(string name, int value) =>
            new Instrument { Name = name, Value = value };

        private Instrument() { }

        public required string Name { get; set; }
        public required int Value { get; set; }
    }

    [ProtoContract]
    [ProtoSurrogateFor(typeof(Instrument))]
    public partial class InstrumentProtoParser
    {
        [ProtoMember(11)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(12)]
        public int Value { get; set; }

        public static implicit operator Instrument(InstrumentProtoParser proxy) =>
            Instrument.FromNameValue(proxy.Name, proxy.Value);

        public static implicit operator InstrumentProtoParser(Instrument instrument) =>
            new InstrumentProtoParser { Name = instrument.Name, Value = instrument.Value };
    }
}
