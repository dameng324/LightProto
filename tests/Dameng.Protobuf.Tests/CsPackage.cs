using System.Collections.Concurrent;
using System.Text;
using Dameng.Protobuf.WellKnownTypes;

namespace Dameng.Protobuf.Tests;

[ProtoContract]
public partial class CsTestMessage
{
    [ProtoMember(1)]
    public string StringField { get; set; } = string.Empty;

    [ProtoMember(2)]
    public int Int32Field { get; set; }

    [ProtoMember(3)]
    public List<int> Int32ArrayField { get; set; } = [];

    [ProtoMember(4)]
    public List<string> StringArrayField { get; set; } = [];

    [ProtoMember(5)]
    public List<byte> BytesField { get; set; } = [];

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

    [ProtoMember(12, DataFormat = DataFormat.ZigZag)]
    public int SInt32Field { get; set; }

    [ProtoMember(13, DataFormat = DataFormat.ZigZag)]
    public long SInt64Field { get; set; }

    [ProtoMember(14, DataFormat = DataFormat.FixedSize)]
    public uint Fixed32Field { get; set; }

    [ProtoMember(15, DataFormat = DataFormat.FixedSize)]
    public ulong Fixed64Field { get; set; }

    [ProtoMember(16, DataFormat = DataFormat.FixedSize)]
    public int SFixed32Field { get; set; }

    [ProtoMember(17, DataFormat = DataFormat.FixedSize)]
    public long SFixed64Field { get; set; }

    [ProtoMember(18)]
    public Dictionary<string, CsTestMessage> MapField { get; set; } = [];

    [ProtoMember(19)]
    public CsTestEnum EnumField { get; set; }

    [ProtoMember(20)]
    public List<CsTestEnum> EnumArrayField { get; set; } = [];

    [ProtoMember(21)]
    public CsTestMessage NestedField { get; set; }

    [ProtoMember(22)]
    public List<CsTestMessage> NestedMessageArrayField { get; set; } = [];

    // google.protobuf.Timestamp
    [ProtoMember(27)]
    public Timestamp TimestampField { get; set; }

    // google.protobuf.Duration
    [ProtoMember(28)]
    public Duration DurationField { get; set; }

    [ProtoMember(29)]
    public Dictionary<string, string> MapField2 { get; set; } = [];

    [ProtoMember(50)]
    [ProtoMap(KeyFormat = DataFormat.FixedSize, ValueFormat = DataFormat.ZigZag)]
    public Dictionary<int, long> MapField4 { get; set; } = [];

    [ProtoMember(51)]
    public DateTime DateTimeField { get; set; }

    [ProtoMember(52)]
    public int? NullableIntField { get; set; }

    [ProtoMember(53)]
    public int[] IntArrayFieldTest { get; set; } = [];

    [ProtoMember(54)]
    public IEnumerable<string> StringListFieldTest { get; set; } = [];

    [ProtoMember(55)]
    public string[] StringArrayFieldTest { get; set; } = [];

    [ProtoMember(56)]
    public IList<int> IntListFieldTest { get; set; } = [];

    [ProtoMember(57)]
    public IDictionary<string, string> MapField5 { get; set; }

    [ProtoMember(58)]
    public IReadOnlyDictionary<string, string> MapField6 { get; set; }

    [ProtoMember(59)]
    public int RequiredIntField { get; set; }

    [ProtoMember(60)]
    internal ConcurrentDictionary<string, CsTestMessage> MapField7 { get; set; } = [];

    [ProtoMember(61)]
    public HashSet<string> StringSetFieldTest { get; set; } = [];

    [ProtoMember(62)]
    public Queue<string> StringQueueFieldTest { get; set; } = [];

    [ProtoMember(63)]
    public Stack<string> StringStackFieldTest { get; set; } = [];

    [ProtoMember(64)]
    public ConcurrentQueue<string> ConcurrentStringQueueFieldTest { get; set; } = [];

    [ProtoMember(65)]
    public ConcurrentStack<string> ConcurrentStringStackFieldTest { get; set; } = [];

    [ProtoMember(66)]
    public ConcurrentBag<int> IntBag { get; set; } = [];

    [ProtoMember(67)]
    public ISet<string> StringISet { get; set; }

    [ProtoMember(68)]
    public TimeSpan TimeSpanField { get; set; }

    [ProtoMember(69)]
    public DateOnly DateOnlyField { get; set; }

    [ProtoMember(70)]
    public Guid GuidField { get; set; }

    [ProtoMember(71)]
    public TimeOnly TimeOnlyField { get; set; }

    [ProtoMember(72)]
    public StringBuilder StringBuilderField { get; set; }
}

[ProtoContract]
public enum CsTestEnum
{
    None = 0,
    OptionA = 1,
    OptionB = 2,
}
