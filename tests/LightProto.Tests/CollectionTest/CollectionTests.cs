using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;
using LightProto.Parser;
using TUnit.Assertions.Conditions.Helpers;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public class Int32CollectionTest : BaseCollectionTest<int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class Int64CollectionTest : BaseCollectionTest<long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SInt32CollectionTest : BaseCollectionTest<int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SInt64CollectionTest : BaseCollectionTest<long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SFixedInt32CollectionTest : BaseCollectionTest<int>
{
    public override IEnumerable<int[]> GetCollection()
    {
        yield return new int[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class SFixedInt64CollectionTest : BaseCollectionTest<long>
{
    public override IEnumerable<long[]> GetCollection()
    {
        yield return new long[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class UInt32CollectionTest : BaseCollectionTest<uint>
{
    public override IEnumerable<uint[]> GetCollection()
    {
        yield return new uint[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class UInt64CollectionTest : BaseCollectionTest<ulong>
{
    public override IEnumerable<ulong[]> GetCollection()
    {
        yield return new ulong[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class FixedInt32CollectionTest : BaseCollectionTest<uint>
{
    public override IEnumerable<uint[]> GetCollection()
    {
        yield return new uint[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class FixedInt64CollectionTest : BaseCollectionTest<ulong>
{
    public override IEnumerable<ulong[]> GetCollection()
    {
        yield return new ulong[] { 1, 2, 3 };
    }
}

[InheritsTests]
public class SingleCollectionTest : BaseCollectionTest<Single>
{
    public override IEnumerable<Single[]> GetCollection()
    {
        yield return new Single[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class DoubleCollectionTest : BaseCollectionTest<Double>
{
    public override IEnumerable<Double[]> GetCollection()
    {
        yield return new Double[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class DecimalCollectionTest : BaseCollectionTest<Decimal>
{
    public override IEnumerable<Decimal[]> GetCollection()
    {
        yield return new Decimal[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class Decimal300CollectionTest : BaseCollectionTest<Decimal>
{
    public override IEnumerable<Decimal[]> GetCollection()
    {
        yield return new Decimal[] { 1, -2, 3 };
    }
}

[InheritsTests]
public class GuidCollectionTest : BaseCollectionTest<Guid>
{
    public override IEnumerable<Guid[]> GetCollection()
    {
        yield return new Guid[] { Guid.Empty, Guid.NewGuid() };
    }
}

[InheritsTests]
public class Guid300CollectionTest : BaseCollectionTest<Guid>
{
    public override IEnumerable<Guid[]> GetCollection()
    {
        yield return new Guid[] { Guid.Empty, Guid.NewGuid() };
    }
}

[InheritsTests]
public class EnumCollectionTest : BaseCollectionTest<PlatformID>
{
    public override IEnumerable<PlatformID[]> GetCollection()
    {
        yield return new PlatformID[] { PlatformID.Win32NT, PlatformID.Unix, PlatformID.MacOSX };
    }
}

[InheritsTests]
public class BoolCollectionTest : BaseCollectionTest<bool>
{
    public override IEnumerable<bool[]> GetCollection()
    {
        yield return new bool[] { true, false };
    }
}

[InheritsTests]
public class ByteArrayCollectionTest : BaseCollectionTest<byte[]>
{
    protected override IEqualityComparer<byte[]> Comparer { get; } =
        StructuralEqualityComparer<byte[]>.Instance;

    public override IEnumerable<byte[][]> GetCollection()
    {
        yield return new byte[][] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } };
    }
}

[InheritsTests]
public class ByteListCollectionTest : BaseCollectionTest<List<byte>>
{
    protected override IEqualityComparer<List<byte>> Comparer { get; } =
        StructuralEqualityComparer<List<byte>>.Instance;

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
public class TimeOnlyCollectionTest : BaseCollectionTest<TimeOnly>
{
    public override IEnumerable<TimeOnly[]> GetCollection()
    {
        yield return new TimeOnly[] { TimeOnly.MinValue, TimeOnly.FromDateTime(DateTime.Now) };
    }
}

[InheritsTests]
public class DateOnlyCollectionTest : BaseCollectionTest<DateOnly>
{
    public override IEnumerable<DateOnly[]> GetCollection()
    {
        yield return new DateOnly[] { DateOnly.MinValue, DateOnly.FromDateTime(DateTime.Now) };
    }
}
#endif

[InheritsTests]
public class DateTimeCollectionTest : BaseCollectionTest<DateTime>
{
    public override IEnumerable<DateTime[]> GetCollection()
    {
        yield return new DateTime[] { DateTime.MinValue, DateTime.Now };
    }
}

[InheritsTests]
public class TimeSpanCollectionTest : BaseCollectionTest<TimeSpan>
{
    public override IEnumerable<TimeSpan[]> GetCollection()
    {
        yield return new TimeSpan[] { TimeSpan.MinValue, DateTime.Now.TimeOfDay };
    }
}

[InheritsTests]
public class DateTime240CollectionTest : BaseCollectionTest<DateTime>
{
    public override IEnumerable<DateTime[]> GetCollection()
    {
        yield return new DateTime[] { DateTime.MinValue, DateTime.Now };
    }
}

[InheritsTests]
public class TimeSpan240CollectionTest : BaseCollectionTest<TimeSpan>
{
    public override IEnumerable<TimeSpan[]> GetCollection()
    {
        yield return new TimeSpan[] { TimeSpan.MinValue, DateTime.Now.TimeOfDay };
    }
}

[InheritsTests]
public class StringCollectionTest : BaseCollectionTest<string>
{
    public override IEnumerable<string[]> GetCollection()
    {
        yield return new string[] { string.Empty, "123" };
    }
}

[InheritsTests]
public class InternedStringCollectionTest : BaseCollectionTest<string>
{
    public override IEnumerable<string[]> GetCollection()
    {
        yield return new string[] { string.Empty, "123" };
    }
}

[InheritsTests]
public class StringBuilderCollectionTest : BaseCollectionTest<StringBuilder>
{
    public class StringBuilderEqualityComparer : EqualityComparer<StringBuilder>
    {
        public override bool Equals(StringBuilder? x, StringBuilder? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x is null || y is null)
                return false;
            return x.ToString() == y.ToString();
        }

        public override int GetHashCode(StringBuilder obj)
        {
            return obj.ToString().GetHashCode();
        }
    }

    protected override IEqualityComparer<StringBuilder> Comparer { get; } =
        new StringBuilderEqualityComparer();

    public override IEnumerable<StringBuilder[]> GetCollection()
    {
        yield return new StringBuilder[] { new StringBuilder(), new StringBuilder("123") };
    }
}

[InheritsTests]
public class NullableCollectionTest : BaseCollectionTest<int?>
{
    public override IEnumerable<int?[]> GetCollection()
    {
        yield return [-1, 0, 1, 2];
    }
}

[InheritsTests]
public partial class ContractCollectionTest : BaseCollectionTest<ContractCollectionTest.Message>
{
    protected override IEqualityComparer<Message> Comparer { get; } =
        StructuralEqualityComparer<Message>.Instance;

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
public class UriCollectionTest : BaseCollectionTest<Uri?>
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
public class HalfCollectionTest : BaseCollectionTest<Half>
{
    public override IEnumerable<Half[]> GetCollection()
    {
        yield return new Half[] { (Half)0.1, (Half)(-2.0), (Half)3.5 };
    }
}

[InheritsTests]
public class RuneCollectionTest : BaseCollectionTest<Rune>
{
    public override IEnumerable<Rune[]> GetCollection()
    {
        yield return new Rune[] { new('A'), new('z'), new('0'), new('€'), new('中') };
    }
}

[InheritsTests]
public class Int128CollectionTest : BaseCollectionTest<Int128>
{
    public override IEnumerable<Int128[]> GetCollection()
    {
        yield return new Int128[]
        {
            Int128.MinValue,
            Int128.MaxValue,
            Int128.Zero,
            1111111111111111111,
            -1111111111111111111,
        };
    }
}

[InheritsTests]
public class UInt128CollectionTest : BaseCollectionTest<UInt128>
{
    public override IEnumerable<UInt128[]> GetCollection()
    {
        yield return new UInt128[] { UInt128.MinValue, UInt128.MaxValue, 1111111111111111111 };
    }
}

[InheritsTests]
public class Vector2CollectionTest : BaseCollectionTest<Vector2>
{
    public override IEnumerable<Vector2[]> GetCollection()
    {
        yield return new Vector2[]
        {
            new Vector2(-1, -2),
            new Vector2(1, -2),
            new Vector2(-1, 2),
            new Vector2(1, 2),
            new Vector2(0, 0),
        };
    }
}

[InheritsTests]
public class Vector3CollectionTest : BaseCollectionTest<Vector3>
{
    public override IEnumerable<Vector3[]> GetCollection()
    {
        yield return new Vector3[]
        {
            new Vector3(-1, -2, 3),
            new Vector3(1, -2, 3),
            new Vector3(-1, 2, 3),
            new Vector3(1, 2, 3),
            new Vector3(0, 0, 0),
        };
    }
}

[InheritsTests]
public class QuaternionCollectionTest : BaseCollectionTest<Quaternion>
{
    public override IEnumerable<Quaternion[]> GetCollection()
    {
        yield return new Quaternion[]
        {
            new Quaternion(-1, -2, 3, 4),
            new Quaternion(1, -2, 3, 4),
            new Quaternion(-1, 2, 3, 4),
            new Quaternion(1, 2, 3, 4),
            new Quaternion(0, 0, 0, 0),
        };
    }
}

[InheritsTests]
public class PlaneCollectionTest : BaseCollectionTest<Plane>
{
    public override IEnumerable<Plane[]> GetCollection()
    {
        yield return new Plane[]
        {
            new Plane(-1, -2, 3, 4),
            new Plane(1, -2, 3, 4),
            new Plane(-1, 2, 3, 4),
            new Plane(1, 2, 3, 4),
            new Plane(0, 0, 0, 0),
        };
    }
}

[InheritsTests]
public class Matrix3x2CollectionTest : BaseCollectionTest<Matrix3x2>
{
    public override IEnumerable<Matrix3x2[]> GetCollection()
    {
        yield return new Matrix3x2[]
        {
            new Matrix3x2(-1, -2, 3, 4, 5, 6),
            new Matrix3x2(1, -2, 3, 4, 5, 6),
            new Matrix3x2(-1, 2, 3, 4, 5, 6),
            new Matrix3x2(1, 2, 3, 4, 5, 6),
            new Matrix3x2(0, 0, 0, 0, 0, 0),
        };
    }
}

[InheritsTests]
public class Matrix4x4CollectionTest : BaseCollectionTest<Matrix4x4>
{
    public override IEnumerable<Matrix4x4[]> GetCollection()
    {
        yield return new Matrix4x4[]
        {
            new Matrix4x4(-1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16),
            new Matrix4x4(1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16),
            new Matrix4x4(-1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16),
            new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16),
            new Matrix4x4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
        };
    }
}
#endif

[InheritsTests]
public class BigIntegerCollectionTest : BaseCollectionTest<BigInteger>
{
    public override IEnumerable<BigInteger[]> GetCollection()
    {
        yield return new BigInteger[]
        {
            BigInteger.Parse("1111111111111111111111111111111111111111111111111111111111111111"),
            BigInteger.Parse("-1111111111111111111111111111111111111111111111111111111111111111"),
        };
    }
}

[InheritsTests]
public class BitArrayCollectionTest : BaseCollectionTest<BitArray>
{
    protected override IEqualityComparer<BitArray> Comparer { get; } =
        StructuralEqualityComparer<BitArray>.Instance;

    public override IEnumerable<BitArray[]> GetCollection()
    {
        yield return new BitArray[]
        {
            new BitArray(new bool[] { true, false, true, false }),
            new BitArray(new bool[] { false, false, false, false }),
            new BitArray(new bool[] { true, true, true, true }),
            new BitArray(new bool[] { }),
        };
    }
}

[InheritsTests]
public class ComplexCollectionTest : BaseCollectionTest<Complex>
{
    public override IEnumerable<Complex[]> GetCollection()
    {
        yield return new Complex[]
        {
            new Complex(-1, -2),
            new Complex(1, -2),
            new Complex(-1, 2),
            new Complex(1, 2),
            new Complex(0, 0),
        };
    }
}

[InheritsTests]
public class CultureInfoCollectionTest : BaseCollectionTest<CultureInfo>
{
    public override IEnumerable<CultureInfo[]> GetCollection()
    {
        yield return new CultureInfo[]
        {
            CultureInfo.InvariantCulture,
            new CultureInfo("en-US"),
            new CultureInfo("fr-FR"),
            new CultureInfo("zh-CN"),
        };
    }
}

[InheritsTests]
public class DateTimeOffsetCollectionTest : BaseCollectionTest<DateTimeOffset>
{
    public override IEnumerable<DateTimeOffset[]> GetCollection()
    {
        yield return new DateTimeOffset[]
        {
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            DateTimeOffset.Now,
            new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromHours(1)),
            new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.FromHours(-1)),
        };
    }
}

[InheritsTests]
public class LazyCollectionTest
{
    [Test]
    [Category("CollectionTest")]
    public async Task CollectionSerializeAndDeserialize()
    {
        int[] original = [-1, 0, 1, 2];
        var lazyList = original.Select(o => new Lazy<int>(() => o)).ToList();
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, lazyList);

        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<Lazy<int>>>(ms);
        var parsedValues = parsed.Select(o => o.Value).ToList();
        await Assert.That(parsedValues).IsEquivalentTo(original);
    }
}

[InheritsTests]
public class TimeZoneInfoCollectionTest : BaseCollectionTest<TimeZoneInfo>
{
    public override IEnumerable<TimeZoneInfo[]> GetCollection()
    {
        List<TimeZoneInfo> timeZones = [TimeZoneInfo.Utc];
#if NET48
        timeZones.Add(TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
#else
        if (OperatingSystem.IsWindows())
        {
            timeZones.Add(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai"));
        }
#endif
        yield return timeZones.ToArray();
    }
}
