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
public partial class TestNewTypes
{
    [ProtoMember(1)]
    public Guid GuidField { get; set; }
    
    [ProtoMember(2)]
    public TimeSpan TimeSpanField { get; set; }
    
    [ProtoMember(3)]
    public DateOnly DateOnlyField { get; set; }
    
    [ProtoMember(4)]
    public TimeOnly TimeOnlyField { get; set; }
    
    [ProtoMember(5)]
    public StringBuilder StringBuilderField { get; set; }
    
    [ProtoMember(6)]
    public Guid? NullableGuidField { get; set; }
    
    [ProtoMember(7)]
    public TimeSpan? NullableTimeSpanField { get; set; }
    
    [ProtoMember(8)]
    public DateOnly? NullableDateOnlyField { get; set; }
    
    [ProtoMember(9)]
    public TimeOnly? NullableTimeOnlyField { get; set; }
}