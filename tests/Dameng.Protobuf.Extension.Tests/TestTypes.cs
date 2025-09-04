using Dameng.Protobuf.Extension;

namespace Dameng.Protobuf.Extension.Tests;

// Test record type
[ProtoContract]
public partial record TestRecord
{
    [ProtoMember(1)]
    public string Name { get; set; } = string.Empty;
    
    [ProtoMember(2)]
    public int Value { get; set; }
}

// Test struct type
[ProtoContract]
public partial struct TestStruct
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]
    public int Value { get; set; }
}