using System.Collections.Concurrent;
using System.Text;
using ProtoBuf;
using DataFormat2 = ProtoBuf.DataFormat;
using ProtoContract1 = LightProto.ProtoContractAttribute;
using ProtoContract2 = ProtoBuf.ProtoContractAttribute;
using ProtoMember1 = LightProto.ProtoMemberAttribute;
using ProtoMember2 = ProtoBuf.ProtoMemberAttribute;
using ProtoMap1 = LightProto.ProtoMapAttribute;
using ProtoMap2 = ProtoBuf.ProtoMapAttribute;

namespace LightProto.Tests;

[ProtoContract1]
[ProtoContract2]
public partial class CsTestMessage
{
    [ProtoMember1(1)]
    [ProtoMember2(1)]
    public string StringField { get; set; } = string.Empty;

    [ProtoMember1(2,IsPacked = false)]
    [ProtoMember2(2,IsPacked = false)]
    public int Int32Field { get; set; }

    [ProtoMember1(3,IsPacked = true)]
    [ProtoMember2(3,IsPacked = true)]
    public List<int> Int32ArrayField { get; set; } = [];

    [ProtoMember1(4)]
    [ProtoMember2(4)]
    public List<string> StringArrayField { get; set; } = [];

    [ProtoMember1(5)]
    [ProtoMember2(5)]
    public byte[] BytesField { get; set; } = [];

    [ProtoMember1(6)]
    [ProtoMember2(6)]
    public bool BoolField { get; set; }

    [ProtoMember1(7)]
    [ProtoMember2(7)]
    public double DoubleField { get; set; }

    [ProtoMember1(8)]
    [ProtoMember2(8)]
    public float FloatField { get; set; }

    [ProtoMember1(9)]
    [ProtoMember2(9)]
    public long Int64Field { get; set; }

    [ProtoMember1(10)]
    [ProtoMember2(10)]
    public uint UInt32Field { get; set; }

    [ProtoMember1(11)]
    [ProtoMember2(11)]
    public ulong UInt64Field { get; set; }

    [ProtoMember1(12, DataFormat = DataFormat.ZigZag)]
    [ProtoMember2(12, DataFormat = DataFormat2.ZigZag)]
    public int SInt32Field { get; set; }

    [ProtoMember1(13, DataFormat = DataFormat.ZigZag)]
    [ProtoMember2(13, DataFormat = DataFormat2.ZigZag)]
    public long SInt64Field { get; set; }

    [ProtoMember1(14, DataFormat = DataFormat.FixedSize)]
    [ProtoMember2(14, DataFormat = DataFormat2.FixedSize)]
    public uint Fixed32Field { get; set; }

    [ProtoMember1(15, DataFormat = DataFormat.FixedSize)]
    [ProtoMember2(15, DataFormat = DataFormat2.FixedSize)]
    public ulong Fixed64Field { get; set; }

    [ProtoMember1(16, DataFormat = DataFormat.FixedSize)]
    [ProtoMember2(16, DataFormat = DataFormat2.FixedSize)]
    public int SFixed32Field { get; set; }

    [ProtoMember1(17, DataFormat = DataFormat.FixedSize)]
    [ProtoMember2(17, DataFormat = DataFormat2.FixedSize)]
    public long SFixed64Field { get; set; }

    [ProtoMember1(18)]
    [ProtoMember2(18)]
    public Dictionary<string, CsTestMessage> MapField { get; set; } = [];

    [ProtoMember1(19)]
    [ProtoMember2(19)]
    public CsTestEnum EnumField { get; set; }

    [ProtoMember1(20,IsPacked = true)]
    [ProtoMember2(20,IsPacked = true)]
    public List<CsTestEnum> EnumArrayField { get; set; } = [];

    [ProtoMember1(21)]
    [ProtoMember2(21)]
    public CsTestMessage? NestedField { get; set; }

    [ProtoMember1(22)]
    [ProtoMember2(22)]
    public List<CsTestMessage> NestedMessageArrayField { get; set; } = [];
    
    [ProtoMember2(27)]
    [LightProto.CompatibilityLevel(LightProto.CompatibilityLevel.Level240)]
    [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level240)]
    public DateTime TimestampField2{ get; set; }

    // google.protobuf.Duration

    [ProtoMember2(28)]
    [LightProto.CompatibilityLevel(LightProto.CompatibilityLevel.Level240)]
    [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level240)]
    public TimeSpan DurationField2 { get; set; }

    [ProtoMember1(29)]
    [ProtoMember2(29)]
    public Dictionary<string, string> MapField2 { get; set; } = [];

    [ProtoMember1(50)]
    [ProtoMember2(50)]
    [ProtoMap1(KeyFormat = DataFormat.FixedSize, ValueFormat = DataFormat.ZigZag)]
    [ProtoMap2(KeyFormat = DataFormat2.FixedSize, ValueFormat = DataFormat2.ZigZag)]
    public Dictionary<int, long> MapField4 { get; set; } = [];

    [ProtoMember1(51)]
    [ProtoMember2(51)]
    public DateTime DateTimeField { get; set; }

    [ProtoMember1(52)]
    [ProtoMember2(52)]
    public int? NullableIntField { get; set; }

    [ProtoMember1(53,IsPacked = true)]
    [ProtoMember2(53,IsPacked = true)]
    public int[] IntArrayFieldTest { get; set; } = [];

    [ProtoMember1(54)]
    [ProtoMember2(54)]
    public IEnumerable<string> StringListFieldTest { get; set; } = new List<string>();

    [ProtoMember1(55)]
    [ProtoMember2(55)]
    public string[] StringArrayFieldTest { get; set; } = [];

    [ProtoMember1(56,IsPacked = true)]
    [ProtoMember2(56,IsPacked = true)]
    public IList<int> IntListFieldTest { get; set; } = [];

    [ProtoMember1(57)]
    [ProtoMember2(57)]
    public IDictionary<string, string>? MapField5 { get; set; }

    [ProtoMember1(58)]
    [ProtoMember2(58)]
    public IReadOnlyDictionary<string, string>? MapField6 { get; set; }

    [ProtoMember1(59)]
    [ProtoMember2(59)]
    public int RequiredIntField { get; set; }

    [ProtoMember1(60)]
    [ProtoMember2(60)]
    internal ConcurrentDictionary<string, CsTestMessage> MapField7 { get; set; } = [];

    [ProtoMember1(61)]
    [ProtoMember2(61)]
    public HashSet<string> StringSetFieldTest { get; set; } = [];

    [ProtoMember1(62)]
    [ProtoMember2(62)]
    public Queue<string> StringQueueFieldTest { get; set; } = [];

    [ProtoMember1(63)]
    [ProtoMember2(63)]
    public Stack<string> StringStackFieldTest { get; set; } = [];

    [ProtoMember1(64)]
    [ProtoMember2(64)]
    public ConcurrentQueue<string> ConcurrentStringQueueFieldTest { get; set; } = [];

    [ProtoMember1(65)]
    [ProtoMember2(65)]
    public ConcurrentStack<string> ConcurrentStringStackFieldTest { get; set; } = [];

    [ProtoMember1(66,IsPacked = true)]
    [ProtoMember2(66,IsPacked = true)]
    public List<int> IntList { get; set; } = [];

    [ProtoMember1(67)]
    [ProtoMember2(67)]
    public ISet<string>? StringISet { get; set; }

    [ProtoMember1(68)]
    [ProtoMember2(68)]
    public TimeSpan TimeSpanField { get; set; }

    [ProtoMember1(69)]
    [ProtoMember2(69)]
    public DateOnly DateOnlyField { get; set; }

    [ProtoMember1(70)]
    [ProtoMember2(70)]
    public Guid GuidField { get; set; }

    [ProtoMember1(71)]
    [ProtoMember2(71)]
    public TimeOnly TimeOnlyField { get; set; }

    [ProtoMember1(72)]
    public StringBuilder? StringBuilderField { get; set; }

    [ProtoMember2(72)]
    public string StringBuilderField2
    {
        get => StringBuilderField?.ToString()??string.Empty;
        set => StringBuilderField = new StringBuilder(value);
    }

    [ProtoMember1(73)]
    [ProtoMember2(73)]
    public Dictionary<string, Dictionary<int, string>>? NestDictionary { get; set; }
}

[ProtoContract1]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2,
}
