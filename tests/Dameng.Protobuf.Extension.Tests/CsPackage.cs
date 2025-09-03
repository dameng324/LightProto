using System.Collections.Concurrent;

namespace Dameng.Protobuf.Extension.Tests;
using System;
using System.Collections.Generic;

[ProtoContract]
public partial class CsTestMessage
{
    [ProtoMember(1)] public partial string? StringField { get; set; }

    [ProtoMember(2)]
    public partial int Int32Field { get; set; }

    [ProtoMember(3)]
    public partial IList<int> Int32ArrayField { get; set; } = new();

    [ProtoMember(4)]
    public partial IList<string> StringArrayField { get; set; } = new();

    [ProtoMember(5)]
    public partial byte[]? BytesField { get; set; }

    [ProtoMember(6)]
    public partial bool BoolField { get; set; }

    [ProtoMember(7)]
    public partial double DoubleField { get; set; }

    [ProtoMember(8)]
    public partial float FloatField { get; set; }

    [ProtoMember(9)]
    public partial long Int64Field { get; set; }

    [ProtoMember(10)]
    public partial uint UInt32Field { get; set; }

    [ProtoMember(11)]
    public partial ulong UInt64Field { get; set; }

    [ProtoMember(12)]
    public partial int SInt32Field { get; set; }

    [ProtoMember(13)]
    public partial long SInt64Field { get; set; }

    [ProtoMember(14)]
    public partial uint Fixed32Field { get; set; }

    [ProtoMember(15)]
    public partial ulong Fixed64Field { get; set; }

    [ProtoMember(16)]
    public partial int SFixed32Field { get; set; }

    [ProtoMember(17)]
    public partial long SFixed64Field { get; set; }

    [ProtoMember(18)]
    public partial IDictionary<string, string> MapField { get; set; } 

    [ProtoMember(19)]
    public partial CsTestEnum EnumField { get; set; }

    [ProtoMember(20)]
    public partial IList<CsTestEnum> EnumArrayField { get; set; } 

    [ProtoMember(21)]
    public partial CsTestMessage? NestedMessageField { get; set; }

    [ProtoMember(22)]
    public partial IList<CsTestMessage>? NestedMessageArrayField { get; set; } 

    // oneof OneofField
    [ProtoMember(23)]
    public partial string? OneofStringField { get; set; }

    [ProtoMember(24)]
    public partial int? OneofInt32Field { get; set; }

    // oneof OneofNestedMessageField
    [ProtoMember(25)]
    public partial CsTestMessage? OneofNestedMessage1 { get; set; }

    [ProtoMember(26)]
    public partial CsTestMessage? OneofNestedMessage2 { get; set; }

    // google.protobuf.Timestamp
    [ProtoMember(27)]
    public partial DateTime TimestampField { get; set; }

    // google.protobuf.Duration
    [ProtoMember(28)]
    public partial TimeSpan DurationField { get; set; }
    [ProtoMember(29)]
    public partial Dictionary<string, string> MapField2 { get; set; } 
    [ProtoMember(30)]
    public partial ConcurrentDictionary<string, string> MapField3 { get; set; } 
}

[ProtoContract]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2
}
