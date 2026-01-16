using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable RedundantVerbatimStringPrefix
// ReSharper disable ArrangeThisQualifier

#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192, CS8618, CS8604, CS0659
namespace LightProto.Tests
{
    [global::LightProto.ProtoContract()]
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    public partial class CsTestMessage : global::LightProto.IExtensible
    {
        private global::LightProto.IExtension __pbn__extensionData;

        global::LightProto.IExtension global::LightProto.IExtensible.GetExtensionObject(bool createIfMissing) =>
            global::LightProto.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::LightProto.ProtoMember(1)]
        [global::System.ComponentModel.DefaultValue("")]
        public string StringField { get; set; } = "123";

        [global::LightProto.ProtoMember(2)]
        public int Int32Field { get; set; }

        [global::LightProto.ProtoMember(3, Name = @"Int32ArrayField", IsPacked = true)]
        public List<int> Int32ArrayFields { get; set; } = new();

        [global::LightProto.ProtoMember(4, Name = @"StringArrayField")]
        public global::System.Collections.Generic.List<string> StringArrayFields { get; } = new();

        [global::LightProto.ProtoMember(5)]
        public byte[] BytesField { get; set; }

        [global::LightProto.ProtoMember(6)]
        public bool BoolField { get; set; }

        [global::LightProto.ProtoMember(7)]
        public double DoubleField { get; set; }

        [global::LightProto.ProtoMember(8)]
        public float FloatField { get; set; }

        [global::LightProto.ProtoMember(9)]
        public long Int64Field { get; set; }

        [global::LightProto.ProtoMember(10)]
        public uint UInt32Field { get; set; }

        [global::LightProto.ProtoMember(11)]
        public ulong UInt64Field { get; set; }

        [global::LightProto.ProtoMember(12, DataFormat = global::LightProto.DataFormat.ZigZag)]
        public int SInt32Field { get; set; }

        [global::LightProto.ProtoMember(13, DataFormat = global::LightProto.DataFormat.ZigZag)]
        public long SInt64Field { get; set; }

        [global::LightProto.ProtoMember(14, DataFormat = global::LightProto.DataFormat.FixedSize)]
        public uint Fixed32Field { get; set; }

        [global::LightProto.ProtoMember(15, DataFormat = global::LightProto.DataFormat.FixedSize)]
        public ulong Fixed64Field { get; set; }

        [global::LightProto.ProtoMember(16, DataFormat = global::LightProto.DataFormat.FixedSize)]
        public int SFixed32Field { get; set; }

        [global::LightProto.ProtoMember(17, DataFormat = global::LightProto.DataFormat.FixedSize)]
        public long SFixed64Field { get; set; }

        [global::LightProto.ProtoMember(18, Name = @"MapField")]
        [global::LightProto.ProtoMap]
        public global::System.Collections.Generic.Dictionary<string, CsTestMessage> MapFields { get; } =
            new global::System.Collections.Generic.Dictionary<string, CsTestMessage>();

        [global::LightProto.ProtoMember(19)]
        public CsTestEnum EnumField { get; set; }

        [global::LightProto.ProtoMember(20, Name = @"EnumArrayField", IsPacked = true)]
        public global::System.Collections.Generic.List<CsTestEnum> EnumArrayFields { get; } =
            new global::System.Collections.Generic.List<CsTestEnum>();

        [global::LightProto.ProtoMember(21)]
#pragma warning disable CS8669 // Nullable reference types in auto-generated code
        public CsTestMessage? NestedMessageField { get; set; }
#pragma warning restore CS8669 // Nullable reference types in auto-generated code

        [global::LightProto.ProtoMember(22, Name = @"NestedMessageArrayField")]
        public global::System.Collections.Generic.List<CsTestMessage> NestedMessageArrayFields { get; set; } =
            new global::System.Collections.Generic.List<CsTestMessage>();

        [global::LightProto.ProtoMember(27)]
        [global::LightProto.CompatibilityLevel(global::LightProto.CompatibilityLevel.Level300)]
        public global::System.DateTime? TimestampField { get; set; }

        [global::LightProto.ProtoMember(28)]
        [global::LightProto.CompatibilityLevel(global::LightProto.CompatibilityLevel.Level300)]
        public global::System.TimeSpan? DurationField { get; set; }

        [global::LightProto.ProtoMember(29, Name = @"MapField2")]
        [global::LightProto.ProtoMap]
        public global::System.Collections.Generic.Dictionary<string, string> MapField2s { get; } =
            new global::System.Collections.Generic.Dictionary<string, string>();

        [global::LightProto.ProtoMember(50, Name = @"MapField4")]
        [global::LightProto.ProtoMap(
            KeyFormat = global::LightProto.DataFormat.FixedSize,
            ValueFormat = global::LightProto.DataFormat.ZigZag
        )]
        public global::System.Collections.Generic.Dictionary<int, long> MapField4s { get; } =
            new global::System.Collections.Generic.Dictionary<int, long>();

        [global::LightProto.ProtoMember(51)]
        public global::System.DateTime? DateTimeField { get; set; }

        [global::LightProto.ProtoMember(52)]
        public int NullableIntField { get; set; }

        [global::LightProto.ProtoMember(53, Name = @"IntArrayFieldTest", IsPacked = true)]
        public int[] IntArrayFieldTests { get; set; } = Array.Empty<int>();

        [global::LightProto.ProtoMember(54, Name = @"StringListFieldTest")]
        public global::System.Collections.Generic.List<string> StringListFieldTests { get; } = new();

        [global::LightProto.ProtoMember(55, Name = @"StringArrayFieldTest")]
        public global::System.Collections.Generic.List<string> StringArrayFieldTests { get; } = new();

        [global::LightProto.ProtoMember(56, Name = @"IntListFieldTest", IsPacked = true)]
        public int[] IntListFieldTests { get; set; } = Array.Empty<int>();

        [global::LightProto.ProtoMember(57, Name = @"MapField5")]
        [global::LightProto.ProtoMap]
        public global::System.Collections.Generic.Dictionary<string, string> MapField5s { get; } =
            new global::System.Collections.Generic.Dictionary<string, string>();

        [global::LightProto.ProtoMember(58, Name = @"MapField6")]
        [global::LightProto.ProtoMap]
        public global::System.Collections.Generic.Dictionary<string, string> MapField6s { get; } =
            new global::System.Collections.Generic.Dictionary<string, string>();

        [global::LightProto.ProtoMember(59)]
        public int RequiredIntField { get; set; }

        [global::LightProto.ProtoMember(60, Name = @"MapField7")]
        [global::LightProto.ProtoMap]
        public global::System.Collections.Generic.Dictionary<string, CsTestMessage> MapField7s { get; } =
            new global::System.Collections.Generic.Dictionary<string, CsTestMessage>();

        [global::LightProto.ProtoMember(61, Name = @"StringSetFieldTest")]
        public global::System.Collections.Generic.HashSet<string> StringSetFieldTests { get; } = new();

        [global::LightProto.ProtoMember(62, Name = @"StringQueueFieldTest")]
        public global::System.Collections.Generic.Queue<string> StringQueueFieldTests { get; set; } = new();

        [global::LightProto.ProtoMember(63, Name = @"StringStackFieldTest")]
        public global::System.Collections.Generic.Stack<string> StringStackFieldTests { get; set; } = new();

        [global::LightProto.ProtoMember(64, Name = @"ConcurrentStringQueueFieldTest")]
        public ConcurrentQueue<string> ConcurrentStringQueueFieldTests { get; set; } = new();

        [global::LightProto.ProtoMember(65, Name = @"ConcurrentStringStackFieldTest")]
        public ConcurrentStack<string> ConcurrentStringStackFieldTests { get; set; } = new();

        [global::LightProto.ProtoMember(66, Name = @"IntList", IsPacked = true)]
        public int[] IntLists { get; set; }

        [global::LightProto.ProtoMember(67, Name = @"StringISet")]
        public global::System.Collections.Generic.ISet<string> StringISets { get; } =
            new global::System.Collections.Generic.HashSet<string>();

        [global::LightProto.ProtoMember(68)]
        public global::System.TimeSpan? TimeSpanField { get; set; }

#if NET6_0_OR_GREATER
        [global::LightProto.ProtoMember(69)]
        public DateOnly DateOnlyField { get; set; }

        [global::LightProto.ProtoMember(71)]
        public TimeOnly TimeOnlyField { get; set; }
#else
        [global::LightProto.ProtoMember(69)]
        public int DateOnlyField { get; set; }

        [global::LightProto.ProtoMember(71)]
        public long TimeOnlyField { get; set; }
#endif

        [global::LightProto.ProtoMember(70)]
        public Guid GuidField { get; set; }

        [global::LightProto.ProtoMember(72)]
        [global::System.ComponentModel.DefaultValue("")]
        public StringBuilder? StringBuilderField { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is not CsTestMessage other)
                return false;

            if (this.BoolField != other.BoolField)
                return false;

            if (this.StringField != other.StringField)
                return false;

            if (this.Int32Field != other.Int32Field)
                return false;

            if (
                ((this.Int32ArrayFields?.Count ?? 0) != 0 && (other.Int32ArrayFields?.Count ?? 0) != 0)
                && !this.Int32ArrayFields.SequenceEqual(other.Int32ArrayFields)
            )
                return false;

            if (
                ((this.BytesField?.Length ?? 0) != 0 && (other.BytesField?.Length ?? 0) != 0)
                && !this.BytesField.SequenceEqual(other.BytesField)
            )
                return false;

            if (this.BoolField != other.BoolField)
                return false;

            if (!(Math.Abs(this.DoubleField - other.DoubleField) < 0.001))
                return false;

            if (!(Math.Abs(this.FloatField - other.FloatField) < 0.001))
                return false;

            if (this.Int64Field != other.Int64Field)
                return false;

            if (this.UInt32Field != other.UInt32Field)
                return false;

            if (this.UInt64Field != other.UInt64Field)
                return false;

            if (this.SInt32Field != other.SInt32Field)
                return false;

            if (this.SInt64Field != other.SInt64Field)
                return false;

            if (this.Fixed32Field != other.Fixed32Field)
                return false;

            if (this.Fixed64Field != other.Fixed64Field)
                return false;

            if (this.SFixed32Field != other.SFixed32Field)
                return false;

            if (this.SFixed64Field != other.SFixed64Field)
                return false;
            if (this.EnumField != other.EnumField)
                return false;

            if (
                ((this.NestedMessageArrayFields?.Count ?? 0) != 0 && (other.NestedMessageArrayFields?.Count ?? 0) != 0)
                && !this.NestedMessageArrayFields.SequenceEqual(other.NestedMessageArrayFields)
            )
                return false;

            if (this.TimestampField != other.TimestampField)
                return false;

            if (this.DurationField != other.DurationField)
                return false;

            if (this.DateTimeField != other.DateTimeField)
                return false;

            if (this.NullableIntField != other.NullableIntField)
                return false;

            if (this.IntArrayFieldTests.Length != other.IntArrayFieldTests.Length)
                return false;

            if (this.IntListFieldTests.Length != other.IntListFieldTests.Length)
                return false;

            if (this.RequiredIntField != other.RequiredIntField)
                return false;

            if (!this.StringQueueFieldTests.SequenceEqual(other.StringQueueFieldTests))
                return false;

            if (!this.StringStackFieldTests.SequenceEqual(other.StringStackFieldTests))
                return false;

            if (!this.ConcurrentStringQueueFieldTests.SequenceEqual(other.ConcurrentStringQueueFieldTests))
                return false;

            if (!this.ConcurrentStringStackFieldTests.SequenceEqual(other.ConcurrentStringStackFieldTests))
                return false;

            if (((this.IntLists?.Length ?? 0) != 0 && (other.IntLists?.Length ?? 0) != 0) && !this.IntLists.SequenceEqual(other.IntLists))
                return false;

            if (this.TimeSpanField != other.TimeSpanField)
                return false;

            if (this.DateOnlyField != other.DateOnlyField)
                return false;

            if (this.TimeOnlyField != other.TimeOnlyField)
                return false;

            if (this.GuidField != other.GuidField)
                return false;

            if (this.StringBuilderField?.ToString() != other.StringBuilderField?.ToString())
                return false;

            if (this.MapFields.Count != other.MapFields.Count)
                return false;

            if (!this.EnumArrayFields.SequenceEqual(other.EnumArrayFields))
                return false;
            if (this.MapField2s.Count != other.MapField2s.Count)
                return false;

            if (this.MapField4s.Count != other.MapField4s.Count)
                return false;
            if (!this.StringArrayFields.SequenceEqual(other.StringArrayFields))
                return false;
            if (!this.StringListFieldTests.SequenceEqual(other.StringListFieldTests))
                return false;

            if (!this.StringArrayFieldTests.SequenceEqual(other.StringArrayFieldTests))
                return false;

            if (this.MapField5s.Count != other.MapField5s.Count)
                return false;

            if (this.MapField6s.Count != other.MapField6s.Count)
                return false;

            if (this.MapField7s.Count != other.MapField7s.Count)
                return false;

            if (!this.StringSetFieldTests.SequenceEqual(other.StringSetFieldTests))
                return false;
            if (!this.StringISets.SequenceEqual(other.StringISets))
                return false;
            return true;
        }
    }

    [global::LightProto.ProtoContract()]
    public enum CsTestEnum
    {
        None = 0,
        OptionA = 1,
        OptionB = 2,
    }
}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
