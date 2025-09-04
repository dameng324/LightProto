using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Dameng.Protobuf.Extension.Tests;

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

    [ProtoMember(12,DataFormat = DataFormat.ZigZag)]
    public int SInt32Field { get; set; }

    [ProtoMember(13,DataFormat = DataFormat.ZigZag)]
    public long SInt64Field { get; set; }

    [ProtoMember(14,DataFormat = DataFormat.FixedSize)]
    public uint Fixed32Field { get; set; }

    [ProtoMember(15,DataFormat = DataFormat.FixedSize)]
    public ulong Fixed64Field { get; set; }

    [ProtoMember(16,DataFormat = DataFormat.FixedSize)]
    public int SFixed32Field { get; set; }

    [ProtoMember(17,DataFormat = DataFormat.FixedSize)]
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
    
    [ProtoMember(31)] public bool BoolField31 { get; set; }
    [ProtoMember(32)] public bool BoolField32 { get; set; }
    [ProtoMember(33)] public bool BoolField33 { get; set; }
    [ProtoMember(34)] public bool BoolField34 { get; set; }
    [ProtoMember(35)] public bool BoolField35 { get; set; }
    [ProtoMember(36)] public bool BoolField36 { get; set; }
    [ProtoMember(37)] public bool BoolField37 { get; set; }
    [ProtoMember(38)] public bool BoolField38 { get; set; }
    [ProtoMember(39)] public bool BoolField39 { get; set; }
    [ProtoMember(41)] public bool BoolField41 { get; set; }
    [ProtoMember(42)] public bool BoolField42 { get; set; }
    [ProtoMember(43)] public bool BoolField43 { get; set; }
    [ProtoMember(44)] public bool BoolField44 { get; set; }
    [ProtoMember(45)] public bool BoolField45 { get; set; }
    [ProtoMember(46)] public bool BoolField46 { get; set; }
    [ProtoMember(47)] public bool BoolField47 { get; set; }
    [ProtoMember(48)] public bool BoolField48 { get; set; }
    [ProtoMember(49)] public bool BoolField49 { get; set; }
    [ProtoMember(50)][ProtoMap(KeyFormat = DataFormat.FixedSize,ValueFormat = DataFormat.ZigZag)] public MapField<int,long> MapField4 { get; set; }
    [ProtoMember(51)] public DateTime DateTimeField { get; set; }
    [ProtoMember(52)] public int? NullableIntField { get; set; }
    
    [ProtoMember(53)] public int[] IntArrayFieldTest { get; set; }
    [ProtoMember(54)] public List<string> StringListFieldTest { get; set; }
    [ProtoMember(55)] public string[] StringArrayFieldTest { get; set; }
    [ProtoMember(56)] public List<int> IntListFieldTest { get; set; }
}

[ProtoContract]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2,
}
