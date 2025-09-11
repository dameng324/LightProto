using pb = global::Google.Protobuf;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Dameng.Protobuf;
using Google.Protobuf.Reflection;

namespace Dameng.Protobuf.Tests;

// Test struct type
[ProtoContract]
public partial struct TestStruct
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public int Value { get; set; }
}

[ProtoContract]
public partial record TestRecord
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public int Value { get; set; }
}

[ProtoContract]
public partial record struct TestRecordStruct
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public int Value { get; set; }
}

[ProtoContract]
public partial class TestRecordRecord
{
    [ProtoMember(1)]
    public TestRecord Record { get; set; }

    public override string ToString() => string.Empty;
}

[ProtoContract]
public partial class ProtoProxy
{
    [ProtoMember(1)]
    public Instrument Instrument { get; set; }
    [ProtoMember(2)]
    public Dictionary<Instrument,Instrument> InstrumentMap { get; set; }
    [ProtoMember(3)]
    public List<Instrument> Instruments { get; set; }
    [ProtoMember(4)]
    public Instrument[] InstrumentArray { get; set; }
    public override string ToString() => string.Empty;
}

[ProtoProxy<InstrumentProxy>()]
public class Instrument
{
    public static Instrument FromNameValue(string name, int value) => new Instrument { Name = name, Value = value };
    private Instrument() { }
    public required string Name { get; set; }
    public required  int Value { get; set; }
}

[ProtoContract]
[ProtoProxyFor<Instrument>()]
public partial class InstrumentProxy
{
    [ProtoMember(11)] public string Name { get; set; }
    [ProtoMember(12)] public int Value { get; set; }

    public static implicit operator Instrument(InstrumentProxy proxy) => Instrument.FromNameValue(proxy.Name,proxy.Value);

    public static implicit operator InstrumentProxy(Instrument instrument) => new InstrumentProxy
        { Name = instrument.Name, Value = instrument.Value };
}
