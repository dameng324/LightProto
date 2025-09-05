using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
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
    public string StringField { get; set; }
    
    [ProtoMember(2)]
    public int IntField { get; set; }
    
    [ProtoMember(3)]
    public Guid GuidField { get; set; }
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