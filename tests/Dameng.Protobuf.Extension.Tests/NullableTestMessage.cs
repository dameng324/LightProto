using Dameng.Protobuf.Extension;

namespace Dameng.Protobuf.Extension.Tests;

[ProtoContract]
public partial class NullableTestMessage
{
    [ProtoMember(1)]
    public int? NullableIntField { get; set; }

    [ProtoMember(2)]
    public bool? NullableBoolField { get; set; }

    [ProtoMember(3)]
    public double? NullableDoubleField { get; set; }

    [ProtoMember(4)]
    public float? NullableFloatField { get; set; }

    [ProtoMember(5)]
    public long? NullableLongField { get; set; }

    [ProtoMember(6)]
    public string StringField { get; set; } // For comparison with non-nullable
    
    // Added comment to trigger regeneration
}