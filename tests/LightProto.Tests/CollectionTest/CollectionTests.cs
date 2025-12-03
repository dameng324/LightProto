using System.Text;
using LightProto.Parser;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public class Int32CollectionTest : BaseCollectionTestWithParser<Int32ProtoParser, int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class Int64CollectionTest : BaseCollectionTestWithParser<Int64ProtoParser, long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SInt32CollectionTest : BaseCollectionTestWithParser<SInt32ProtoParser, int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SInt64CollectionTest : BaseCollectionTestWithParser<SInt64ProtoParser, long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SFixedInt32CollectionTest : BaseCollectionTestWithParser<SFixed32ProtoParser, int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SFixedInt64CollectionTest : BaseCollectionTestWithParser<SFixed64ProtoParser, long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class UInt32CollectionTest : BaseCollectionTestWithParser<UInt32ProtoParser, uint>
{
    public override IEnumerable<uint[]> GetCollection()
    {
        yield return new uint[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class UInt64CollectionTest : BaseCollectionTestWithParser<UInt64ProtoParser, ulong>
{
    public override IEnumerable<ulong[]> GetCollection()
    {
        yield return new ulong[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class FixedInt32CollectionTest : BaseCollectionTestWithParser<Fixed32ProtoParser, uint>
{
    public override IEnumerable<uint[]> GetCollection()
    {
        yield return new uint[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class FixedInt64CollectionTest : BaseCollectionTestWithParser<Fixed64ProtoParser, ulong>
{
    public override IEnumerable<ulong[]> GetCollection()
    {
        yield return new ulong[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class SingleCollectionTest : BaseCollectionTestWithParser<SingleProtoParser, Single>
{
    public override IEnumerable<Single[]> GetCollection()
    {
        yield return new Single[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class DoubleCollectionTest : BaseCollectionTestWithParser<DoubleProtoParser, Double>
{
    public override IEnumerable<Double[]> GetCollection()
    {
        yield return new Double[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class DecimalCollectionTest : BaseCollectionTestWithParser<DecimalProtoParser, Decimal>
{
    public override IEnumerable<Decimal[]> GetCollection()
    {
        yield return new Decimal[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class Decimal300CollectionTest : BaseCollectionTestWithParser<Decimal300ProtoParser, Decimal>
{
    public override IEnumerable<Decimal[]> GetCollection()
    {
        yield return new Decimal[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class GuidCollectionTest : BaseCollectionTestWithParser<GuidProtoParser, Guid>
{
    public override IEnumerable<Guid[]> GetCollection()
    {
        yield return new Guid[] { Guid.Empty, Guid.NewGuid() };
    }
}

[InheritsTests]
public class Guid300CollectionTest : BaseCollectionTestWithParser<Guid300ProtoParser, Guid>
{
    public override IEnumerable<Guid[]> GetCollection()
    {
        yield return new Guid[] { Guid.Empty, Guid.NewGuid() };
    }
}

[InheritsTests]
public class EnumCollectionTest
    : BaseCollectionTestWithParser<EnumProtoParser<PlatformID>, PlatformID>
{
    public override IEnumerable<PlatformID[]> GetCollection()
    {
        yield return new PlatformID[] { PlatformID.Win32NT, PlatformID.Unix, PlatformID.MacOSX };
    }
}

[InheritsTests]
public class BoolCollectionTest : BaseCollectionTestWithParser<BooleanProtoParser, bool>
{
    public override IEnumerable<bool[]> GetCollection()
    {
        yield return new bool[] { true, false };
    }
}

[InheritsTests]
public class ByteArrayCollectionTest : BaseCollectionTestWithParser<ByteArrayProtoParser, byte[]>
{
    public override IEnumerable<byte[][]> GetCollection()
    {
        yield return new byte[][] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
    }
}

[InheritsTests]
public class ByteListCollectionTest : BaseCollectionTestWithParser<ByteListProtoParser, List<byte>>
{
    public override IEnumerable<List<byte>[]> GetCollection()
    {
        yield return new List<byte>[]
        {
            new List<byte> { 1, 2, 3 },
            new List<byte> { 4, 5, 6 },
        };
    }
}

#if NET6_0_OR_GREATER
[InheritsTests]
public class TimeOnlyCollectionTest : BaseCollectionTestWithParser<TimeOnlyProtoParser, TimeOnly>
{
    public override IEnumerable<TimeOnly[]> GetCollection()
    {
        yield return new TimeOnly[] { TimeOnly.MinValue, TimeOnly.FromDateTime(DateTime.Now) };
    }
}

[InheritsTests]
public class DateOnlyCollectionTest : BaseCollectionTestWithParser<DateOnlyProtoParser, DateOnly>
{
    public override IEnumerable<DateOnly[]> GetCollection()
    {
        yield return new DateOnly[] { DateOnly.MinValue, DateOnly.FromDateTime(DateTime.Now) };
    }
}
#endif

[InheritsTests]
public class DateTimeCollectionTest : BaseCollectionTestWithParser<DateTimeProtoParser, DateTime>
{
    public override IEnumerable<DateTime[]> GetCollection()
    {
        yield return new DateTime[] { DateTime.MinValue, DateTime.Now };
    }
}

[InheritsTests]
public class TimeSpanCollectionTest : BaseCollectionTestWithParser<TimeSpanProtoParser, TimeSpan>
{
    public override IEnumerable<TimeSpan[]> GetCollection()
    {
        yield return new TimeSpan[] { TimeSpan.MinValue, DateTime.Now.TimeOfDay };
    }
}

[InheritsTests]
public class DateTime240CollectionTest
    : BaseCollectionTestWithParser<DateTime240ProtoParser, DateTime>
{
    public override IEnumerable<DateTime[]> GetCollection()
    {
        yield return new DateTime[] { DateTime.MinValue, DateTime.Now };
    }
}

[InheritsTests]
public class TimeSpan240CollectionTest
    : BaseCollectionTestWithParser<TimeSpan240ProtoParser, TimeSpan>
{
    public override IEnumerable<TimeSpan[]> GetCollection()
    {
        yield return new TimeSpan[] { TimeSpan.MinValue, DateTime.Now.TimeOfDay };
    }
}

[InheritsTests]
public class StringCollectionTest : BaseCollectionTestWithParser<StringProtoParser, string>
{
    public override IEnumerable<string[]> GetCollection()
    {
        yield return new string[] { string.Empty, "123" };
    }
}

[InheritsTests]
public class InternedStringCollectionTest
    : BaseCollectionTestWithParser<InternedStringProtoParser, string>
{
    public override IEnumerable<string[]> GetCollection()
    {
        yield return new string[] { string.Empty, "123" };
    }
}

[InheritsTests]
public class StringBuilderCollectionTest
    : BaseCollectionTestWithParser<StringBuilderProtoParser, StringBuilder>
{
    public override IEnumerable<StringBuilder[]> GetCollection()
    {
        yield return new StringBuilder[] { new StringBuilder(), new StringBuilder("123") };
    }
}

[InheritsTests]
public class NullableCollectionTest : BaseCollectionTest<int?>
{
    public override IProtoWriter<int?> ProtoWriter { get; } =
        new NullableProtoWriter<int>(Int32ProtoParser.ProtoWriter);
    public override IProtoReader<int?> ProtoReader { get; } =
        new NullableProtoReader<int>(Int32ProtoParser.ProtoReader);

    public override IEnumerable<int?[]> GetCollection()
    {
        yield return [-1, 0, 1, 2];
    }
}

[InheritsTests]
public partial class ContractCollectionTest
    : BaseCollectionTestWithParser<ContractCollectionTest.Message, ContractCollectionTest.Message>
{
    [ProtoContract]
    public partial class Message : IComparable<Message>
    {
        [ProtoMember(1)]
        public string Value { get; set; } = string.Empty;

        public int CompareTo(Message? other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (other is null)
                return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }

    public override IEnumerable<Message[]> GetCollection()
    {
        yield return new Message[]
        {
            new Message() { Value = "" },
            new Message() { Value = "123" },
        };
    }
}

[InheritsTests]
public class UriCollectionTest : BaseCollectionTestWithParser<UriProtoParser, Uri?>
{
    public override IEnumerable<Uri[]> GetCollection()
    {
        yield return
        [
            new Uri("file:///home/user/file.txt"),
            new Uri("https://localhost:8080"),
            new Uri("/relative/path", UriKind.Relative),
        ];
    }
}

#if NET7_0_OR_GREATER
[InheritsTests]
public class HalfCollectionTest : BaseCollectionTestWithParser<HalfProtoParser, Half>
{
    public override IEnumerable<Half[]> GetCollection()
    {
        yield return new Half[] { (Half)0.1, (Half)(-2.0), (Half)3.5 };
    }
}

[InheritsTests]
public class RuneCollectionTest : BaseCollectionTestWithParser<RuneProtoParser, Rune>
{
    public override IEnumerable<Rune[]> GetCollection()
    {
        yield return new Rune[] { new('A'), new('z'), new('0'), new('€'), new('中') };
    }
}
#endif
