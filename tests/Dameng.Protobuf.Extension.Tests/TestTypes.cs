using System.Runtime.InteropServices;
using System.Text;
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