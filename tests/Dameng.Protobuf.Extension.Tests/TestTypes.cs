using System.Runtime.InteropServices;
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