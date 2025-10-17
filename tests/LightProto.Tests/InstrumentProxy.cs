using LightProto;
using LightProto.Tests;

namespace LightProto.Parser;

[ProtoContract]
[ProtoSurrogateFor<Instrument>()]
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
