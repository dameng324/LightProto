namespace LightProto.Tests;

public partial class ProxyTests
{
    [Test]
    public async Task ProxyTest()
    {
        ProtoProxy testObj = new() { Instrument = Instrument.FromNameValue("hello", 20) };

        var bytes = testObj.ToByteArray();
        var parsed = ProtoProxy.Reader.ParseFrom(bytes);
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

    [ProtoProxy<InstrumentProxy>()]
    public class Instrument
    {
        public static Instrument FromNameValue(string name, int value) =>
            new Instrument { Name = name, Value = value };

        private Instrument() { }

        public required string Name { get; set; }
        public required int Value { get; set; }
    }

    [ProtoContract]
    [ProtoProxyFor<Instrument>()]
    public partial class InstrumentProxy
    {
        [ProtoMember(11)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(12)]
        public int Value { get; set; }

        public static implicit operator Instrument(InstrumentProxy proxy) =>
            Instrument.FromNameValue(proxy.Name, proxy.Value);

        public static implicit operator InstrumentProxy(Instrument instrument) =>
            new InstrumentProxy { Name = instrument.Name, Value = instrument.Value };
    }
}
