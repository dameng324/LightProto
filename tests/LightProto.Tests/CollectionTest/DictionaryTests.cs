using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Numerics;
using System.Text;
using LightProto.Parser;
using TUnit.Assertions.Conditions.Helpers;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public class Int32DictionaryTest : BaseDictionaryTest<string, int>
{
    public override IEnumerable<IReadOnlyDictionary<string, int>> GetDictionary()
    {
        yield return new Dictionary<string, int>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
        yield return new Dictionary<string, int>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class Int64DictionaryTest : BaseDictionaryTest<string, long>
{
    public override IEnumerable<IReadOnlyDictionary<string, long>> GetDictionary()
    {
        yield return new Dictionary<string, long>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class SInt32DictionaryTest : BaseDictionaryTest<string, int>
{
    public override IEnumerable<IReadOnlyDictionary<string, int>> GetDictionary()
    {
        yield return new Dictionary<string, int>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class SInt64DictionaryTest : BaseDictionaryTest<string, long>
{
    public override IEnumerable<IReadOnlyDictionary<string, long>> GetDictionary()
    {
        yield return new Dictionary<string, long>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class SFixedInt32DictionaryTest : BaseDictionaryTest<string, int>
{
    public override IEnumerable<IReadOnlyDictionary<string, int>> GetDictionary()
    {
        yield return new Dictionary<string, int>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class SFixedInt64DictionaryTest : BaseDictionaryTest<string, long>
{
    public override IEnumerable<IReadOnlyDictionary<string, long>> GetDictionary()
    {
        yield return new Dictionary<string, long>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class UInt32DictionaryTest : BaseDictionaryTest<string, uint>
{
    public override IEnumerable<IReadOnlyDictionary<string, uint>> GetDictionary()
    {
        yield return new Dictionary<string, uint>()
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class UInt64DictionaryTest : BaseDictionaryTest<string, ulong>
{
    public override IEnumerable<IReadOnlyDictionary<string, ulong>> GetDictionary()
    {
        yield return new Dictionary<string, ulong>()
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class FixedInt32DictionaryTest : BaseDictionaryTest<string, uint>
{
    public override IEnumerable<IReadOnlyDictionary<string, uint>> GetDictionary()
    {
        yield return new Dictionary<string, uint>()
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class FixedInt64DictionaryTest : BaseDictionaryTest<string, ulong>
{
    public override IEnumerable<IReadOnlyDictionary<string, ulong>> GetDictionary()
    {
        yield return new Dictionary<string, ulong>()
        {
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class SingleDictionaryTest : BaseDictionaryTest<string, Single>
{
    public override IEnumerable<IReadOnlyDictionary<string, Single>> GetDictionary()
    {
        yield return new Dictionary<string, Single>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class DoubleDictionaryTest : BaseDictionaryTest<string, Double>
{
    public override IEnumerable<IReadOnlyDictionary<string, Double>> GetDictionary()
    {
        yield return new Dictionary<string, Double>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class DecimalDictionaryTest : BaseDictionaryTest<string, Decimal>
{
    public override IEnumerable<IReadOnlyDictionary<string, Decimal>> GetDictionary()
    {
        yield return new Dictionary<string, Decimal>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class Decimal300DictionaryTest : BaseDictionaryTest<string, Decimal>
{
    public override IEnumerable<IReadOnlyDictionary<string, Decimal>> GetDictionary()
    {
        yield return new Dictionary<string, Decimal>()
        {
            { "1", 1 },
            { "-2", -2 },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public class GuidDictionaryTest : BaseDictionaryTest<string, Guid>
{
    public override IEnumerable<IReadOnlyDictionary<string, Guid>> GetDictionary()
    {
        yield return new Dictionary<string, Guid>()
        {
            { Guid.Empty.ToString(), Guid.Empty },
            { Guid.NewGuid().ToString(), Guid.NewGuid() },
        };
    }
}

[InheritsTests]
public class Guid300DictionaryTest : BaseDictionaryTest<string, Guid>
{
    public override IEnumerable<IReadOnlyDictionary<string, Guid>> GetDictionary()
    {
        yield return new Dictionary<string, Guid>()
        {
            { Guid.Empty.ToString(), Guid.Empty },
            { Guid.NewGuid().ToString(), Guid.NewGuid() },
        };
    }
}

[InheritsTests]
public class EnumDictionaryTest : BaseDictionaryTest<string, PlatformID>
{
    public override IEnumerable<IReadOnlyDictionary<string, PlatformID>> GetDictionary()
    {
        yield return new Dictionary<string, PlatformID>()
        {
            { PlatformID.Win32NT.ToString(), PlatformID.Win32NT },
            { PlatformID.Unix.ToString(), PlatformID.Unix },
            { PlatformID.MacOSX.ToString(), PlatformID.MacOSX },
        };
    }
}

[InheritsTests]
public class BoolDictionaryTest : BaseDictionaryTest<string, bool>
{
    public override IEnumerable<IReadOnlyDictionary<string, bool>> GetDictionary()
    {
        yield return new Dictionary<string, bool>()
        {
            { true.ToString(), true },
            { false.ToString(), false },
        };
    }
}

[InheritsTests]
public class ByteArrayDictionaryTest : BaseDictionaryTest<string, byte[]>
{
    public override IEnumerable<IReadOnlyDictionary<string, byte[]>> GetDictionary()
    {
        yield return new Dictionary<string, byte[]>()
        {
            { "1", new byte[] { 1, 2, 3 } },
            { "2", new byte[] { 4, 5, 6 } },
        };
    }
}

[InheritsTests]
public class ByteListDictionaryTest : BaseDictionaryTest<string, List<byte>>
{
    public override IEnumerable<IReadOnlyDictionary<string, List<byte>>> GetDictionary()
    {
        yield return new Dictionary<string, List<byte>>()
        {
            {
                "1",
                new List<byte> { 1, 2, 3 }
            },
            {
                "2",
                new List<byte> { 4, 5, 6 }
            },
        };
    }
}

#if NET6_0_OR_GREATER
[InheritsTests]
public class TimeOnlyDictionaryTest : BaseDictionaryTest<string, TimeOnly>
{
    public override IEnumerable<IReadOnlyDictionary<string, TimeOnly>> GetDictionary()
    {
        yield return new Dictionary<string, TimeOnly>()
        {
            { TimeOnly.MinValue.ToString(), TimeOnly.MinValue },
            { TimeOnly.FromDateTime(DateTime.Now).ToString(), TimeOnly.FromDateTime(DateTime.Now) },
        };
    }
}

[InheritsTests]
public class DateOnlyDictionaryTest : BaseDictionaryTest<string, DateOnly>
{
    public override IEnumerable<IReadOnlyDictionary<string, DateOnly>> GetDictionary()
    {
        yield return new Dictionary<string, DateOnly>()
        {
            { DateOnly.MinValue.ToString(), DateOnly.MinValue },
            { DateOnly.FromDateTime(DateTime.Now).ToString(), DateOnly.FromDateTime(DateTime.Now) },
        };
    }
}
#endif

[InheritsTests]
public class DateTimeDictionaryTest : BaseDictionaryTest<string, DateTime>
{
    public override IEnumerable<IReadOnlyDictionary<string, DateTime>> GetDictionary()
    {
        yield return new Dictionary<string, DateTime>()
        {
            { DateTime.MinValue.ToString(), DateTime.MinValue },
            { DateTime.Now.ToString(), DateTime.Now },
        };
    }
}

[InheritsTests]
public class TimeSpanDictionaryTest : BaseDictionaryTest<string, TimeSpan>
{
    public override IEnumerable<IReadOnlyDictionary<string, TimeSpan>> GetDictionary()
    {
        yield return new Dictionary<string, TimeSpan>()
        {
            { TimeSpan.MinValue.ToString(), TimeSpan.MinValue },
            { DateTime.Now.TimeOfDay.ToString(), DateTime.Now.TimeOfDay },
        };
    }
}

[InheritsTests]
public class DateTime240DictionaryTest : BaseDictionaryTest<string, DateTime>
{
    public override IEnumerable<IReadOnlyDictionary<string, DateTime>> GetDictionary()
    {
        yield return new Dictionary<string, DateTime>()
        {
            { DateTime.MinValue.ToString(), DateTime.MinValue },
            { DateTime.Now.ToString(), DateTime.Now },
        };
    }
}

[InheritsTests]
public class TimeSpan240DictionaryTest : BaseDictionaryTest<string, TimeSpan>
{
    public override IEnumerable<IReadOnlyDictionary<string, TimeSpan>> GetDictionary()
    {
        yield return new Dictionary<string, TimeSpan>()
        {
            { TimeSpan.MinValue.ToString(), TimeSpan.MinValue },
            { DateTime.Now.TimeOfDay.ToString(), DateTime.Now.TimeOfDay },
        };
    }
}

[InheritsTests]
public class StringDictionaryTest : BaseDictionaryTest<string, string>
{
    public override IEnumerable<IReadOnlyDictionary<string, string>> GetDictionary()
    {
        yield return new Dictionary<string, string>()
        {
            { string.Empty.ToString(), string.Empty },
            { "123".ToString(), "123" },
        };
    }
}

[InheritsTests]
public class NullableDictionaryTest : BaseDictionaryTest<string, int?>
{
    public override IEnumerable<IReadOnlyDictionary<string, int?>> GetDictionary()
    {
        yield return new Dictionary<string, int?>()
        {
            { "1", 1 },
            //{ "null", null },
            { "3", 3 },
        };
    }
}

[InheritsTests]
public partial class ContractDictionaryTest
    : BaseDictionaryTest<string, ContractDictionaryTest.Message>
{
    [ProtoContract]
    public partial class Message
    {
        [ProtoMember(1)]
        public string Value { get; set; } = string.Empty;
    }

    public override IEnumerable<IReadOnlyDictionary<string, Message>> GetDictionary()
    {
        yield return new Dictionary<string, Message>()
        {
            {
                "1",
                new Message() { Value = "" }
            },
            {
                "2",
                new Message() { Value = "123" }
            },
        };
    }
}
