using System.Collections.Concurrent;

namespace Dameng.Protobuf.Extension.Tests;
using System;
using System.Collections.Generic;

[ProtoContract]
public partial class CsTestMessage
{
    [ProtoMember(1)] public string? StringField { get; set; }

    [ProtoMember(2)]
    public int Int32Field { get; set; }

    [ProtoMember(3)]
    public IList<int> Int32ArrayField { get; set; } = new List<int>();

    [ProtoMember(4)]
    public IList<string> StringArrayField { get; set; } = new List<string>();

    [ProtoMember(5)]
    public byte[]? BytesField { get; set; }

    [ProtoMember(6)]
    public bool BoolField { get; set; }

    [ProtoMember(7)]
    public double DoubleField { get; set; }

    [ProtoMember(8)]
    public float FloatField { get; set; }

    [ProtoMember(9)]
    public long Int64Field { get; set; }

    [ProtoMember(10)]
    public uint UInt32Field { get; set; }

    [ProtoMember(11)]
    public ulong UInt64Field { get; set; }

    [ProtoMember(12)]
    public int SInt32Field { get; set; }

    [ProtoMember(13)]
    public long SInt64Field { get; set; }

    [ProtoMember(14)]
    public uint Fixed32Field { get; set; }

    [ProtoMember(15)]
    public ulong Fixed64Field { get; set; }

    [ProtoMember(16)]
    public int SFixed32Field { get; set; }

    [ProtoMember(17)]
    public long SFixed64Field { get; set; }

    [ProtoMember(18)]
    public IDictionary<string, string> MapField { get; set; } = new Dictionary<string, string>();

    [ProtoMember(19)]
    public CsTestEnum EnumField { get; set; }

    [ProtoMember(20)]
    public IList<CsTestEnum> EnumArrayField { get; set; } = new List<CsTestEnum>(); 

    [ProtoMember(21)]
    public CsTestMessage? NestedMessageField { get; set; }

    [ProtoMember(22)]
    public IList<CsTestMessage>? NestedMessageArrayField { get; set; } = new List<CsTestMessage>(); 

    // oneof OneofField
    [ProtoMember(23)]
    public string? OneofStringField { get; set; }

    [ProtoMember(24)]
    public int? OneofInt32Field { get; set; }

    // oneof OneofNestedMessageField
    [ProtoMember(25)]
    public CsTestMessage? OneofNestedMessage1 { get; set; }

    [ProtoMember(26)]
    public CsTestMessage? OneofNestedMessage2 { get; set; }

    // google.protobuf.Timestamp
    [ProtoMember(27)]
    public DateTime TimestampField { get; set; }

    // google.protobuf.Duration
    [ProtoMember(28)]
    public TimeSpan DurationField { get; set; }
    [ProtoMember(29)]
    public Dictionary<string, string> MapField2 { get; set; } = new Dictionary<string, string>();
    [ProtoMember(30)]
    public ConcurrentDictionary<string, string> MapField3 { get; set; } = new ConcurrentDictionary<string, string>(); 
}

[ProtoContract]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2
}
