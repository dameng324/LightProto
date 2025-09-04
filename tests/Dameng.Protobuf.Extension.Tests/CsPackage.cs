using System.Collections.Concurrent;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Extension.Tests;

using System;
using System.Collections.Generic;

[ProtoContract]
public partial class CsTestMessage
{
    [ProtoMember(1)]
    public string StringField { get; set; }

    [ProtoMember(2)]
    public int Int32Field { get; set; }

    [ProtoMember(3)]
    public RepeatedField<int> Int32ArrayField { get; set; }

    [ProtoMember(4)]
    public RepeatedField<string> StringArrayField { get; set; }

    [ProtoMember(5)]
    public ByteString BytesField { get; set; }

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
    public MapField<string, string> MapField { get; set; }

    [ProtoMember(19)]
    public CsTestEnum EnumField { get; set; }

    [ProtoMember(20)]
    public RepeatedField<CsTestEnum> EnumArrayField { get; set; }

    [ProtoMember(21)]
    public CsTestMessage NestedMessageField { get; set; }

    [ProtoMember(22)]
    public RepeatedField<CsTestMessage> NestedMessageArrayField { get; set; }

    // google.protobuf.Timestamp
    [ProtoMember(27)]
    public Timestamp TimestampField { get; set; }

    // google.protobuf.Duration
    [ProtoMember(28)]
    public Duration DurationField { get; set; }

    [ProtoMember(29)]
    public MapField<string, string> MapField2 { get; set; }

    [ProtoMember(30)]
    public MapField<string, string> MapField3 { get; set; }
}

[ProtoContract]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2,
}
