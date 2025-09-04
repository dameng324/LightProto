using Dameng.Protobuf.Extension;

namespace Dameng.Protobuf.Extension.Tests;

[ProtoContract]
public partial class SimpleNullableTestV2
{
    [ProtoMember(1)]
    public int? NullableInt { get; set; }
    
    [ProtoMember(2)]
    public string StringField { get; set; }
}