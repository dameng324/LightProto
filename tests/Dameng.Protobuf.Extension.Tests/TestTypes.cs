using pb = global::Google.Protobuf;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Dameng.Protobuf.Extension;
using Google.Protobuf.Reflection;

namespace Dameng.Protobuf.Extension.Tests;

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

// Test new types: Guid, TimeSpan, DateOnly, TimeOnly, StringBuilder
[ProtoContract]
public partial class TestSimpleNewTypes
{
    [ProtoMember(1)] public TimeSpan TimeSpanField { get; set; }

    [ProtoMember(2)] public DateOnly DateOnlyField { get; set; }

    [ProtoMember(3)] public Guid GuidField { get; set; }

    [ProtoMember(4)] public TimeOnly TimeOnlyField { get; set; }

    [ProtoMember(5)] public StringBuilder StringBuilderField { get; set; }
    [ProtoMember(6)] public DateTime DateTimeField { get; set; }
}

// Test types for HashSet and ISet support
[ProtoContract]
public partial class TestHashSet
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public HashSet<int> IntSet { get; set; }

    [ProtoMember(3)] public HashSet<string> StringSet { get; set; }
}

[ProtoContract]
public partial class TestISet
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public ISet<int> IntSet { get; set; }

    [ProtoMember(3)] public ISet<string> StringSet { get; set; }
}

[ProtoContract]
public partial class TestConcurrentCollection
{
    [ProtoMember(1)] public string Name { get; set; }

    [ProtoMember(2)] public ConcurrentBag<int> IntBag { get; set; }
    [ProtoMember(3)] public ConcurrentQueue<string> ConcurrentQueue { get; set; }
    [ProtoMember(4)] public ConcurrentStack<string> ConcurrentStack { get; set; }
    [ProtoMember(6)] public List<int> IntList { get; set; }
    [ProtoMember(7)] public IList<int> IntIList { get; set; }

    [ProtoMember(8)] public ImmutableArray<int> IntImmutableArray { get; set; }
    [ProtoMember(9)] public ImmutableHashSet<int> ImmutableHashSet { get; set; }
    [ProtoMember(10)] public ImmutableList<int> ImmutableList { get; set; }
    [ProtoMember(11)] public ImmutableQueue<int> ImmutableQueue { get; set; }
    [ProtoMember(12)] public ImmutableStack<int> ImmutableStack { get; set; }
}

[ProtoContract]
public partial class TestOrder
{
    [ProtoMember(1)]
    public Instrument Instrument { get; set; }

    public override string ToString() => string.Empty;
}

[ProtoProxy<InstrumentProxy>()]
public class Instrument
{
    public static Instrument FromNameValue(string name, int value) => new Instrument { Name = name, Value = value };
    private Instrument()
    {
    }
    public required string Name { get; set; }
    public required  int Value { get; set; }
}

[ProtoContract]
public partial class InstrumentProxy
{
    [ProtoMember(11)] public string Name { get; set; }
    [ProtoMember(12)] public int Value { get; set; }

    public static implicit operator Instrument(InstrumentProxy proxy) => Instrument.FromNameValue(proxy.Name,proxy.Value);

    public static implicit operator InstrumentProxy(Instrument instrument) => new InstrumentProxy
        { Name = instrument.Name, Value = instrument.Value };
}
