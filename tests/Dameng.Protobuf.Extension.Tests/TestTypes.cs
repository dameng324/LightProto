using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Dameng.Protobuf.Extension;

namespace Dameng.Protobuf.Extension.Tests;

// Test struct type
[ProtoContract]
public partial struct TestStruct
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public int Value { get; set; }
}
[ProtoContract]
public partial record TestRecord
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public int Value { get; set; }
}
[ProtoContract]
public partial record struct TestRecordStruct
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public int Value { get; set; }
}

// Test new types: Guid, TimeSpan, DateOnly, TimeOnly, StringBuilder
[ProtoContract]
public partial class TestSimpleNewTypes
{
    [ProtoMember(1)]
    public TimeSpan TimeSpanField { get; set; }
    
    [ProtoMember(2)]
    public DateOnly DateOnlyField { get; set; }
    
    [ProtoMember(3)]
    public Guid GuidField { get; set; }
    
    [ProtoMember(4)]
    public TimeOnly TimeOnlyField { get; set; }
    
    [ProtoMember(5)]
    public StringBuilder StringBuilderField { get; set; }
    [ProtoMember(6)]
    public DateTime DateTimeField { get; set; }
}

// Test types for HashSet and ISet support
[ProtoContract]
public partial class TestHashSet
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public HashSet<int> IntSet { get; set; }
    
    [ProtoMember(3)]
    public HashSet<string> StringSet { get; set; }
}

[ProtoContract]
public partial class TestISet
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public ISet<int> IntSet { get; set; }
    
    [ProtoMember(3)]
    public ISet<string> StringSet { get; set; }
}

// Test types for Concurrent Collections will be added once the generator logic is working

// Temporary test for ConcurrentBag using the same pattern as HashSet
[ProtoContract]
public partial class TestConcurrentCollection
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public ConcurrentBag<int> IntBag { get; set; }
    [ProtoMember(3)]
    public ConcurrentQueue<string> ConcurrentQueue { get; set; }
    [ProtoMember(4)]
    public ConcurrentStack<string> ConcurrentStack { get; set; }
    [ProtoMember(6)]
    public List<int> IntList { get; set; }
    [ProtoMember(7)]
    public IList<int> IntIList { get; set; }

    [ProtoMember(8)]
    public ImmutableArray<int> IntImmutableArray { get; set; }
    [ProtoMember(9)]
    public ImmutableHashSet<int> ImmutableHashSet { get; set; }
    [ProtoMember(10)]
    public ImmutableList<int> ImmutableList { get; set; }
    [ProtoMember(11)]
    public ImmutableQueue<int> ImmutableQueue { get; set; }
    [ProtoMember(12)]
    public ImmutableStack<int> ImmutableStack { get; set; }

}