using System.Collections;
using LightProto.Parser;

[assembly: LightProto.ProtoRepeatedParserTypeMap(
    typeof(LightProto.Tests.CustomParser.AssemblyMappedList<>),
    typeof(LightProto.Tests.CustomParser.AssemblyMapReader<>),
    typeof(LightProto.Tests.CustomParser.AssemblyMapWriter<>)
)]

namespace LightProto.Tests.CustomParser;

public partial class CustomRepeatedParserTests
{
    [Test]
    public async Task TypeLevelRepeatedParser_UsesPackedAndMessageItemWireFormats()
    {
        var message = new TypeLevelContract
        {
            FixedSizeValues = new TypeLevelList<int>(3) { 1, 2, 3 },
            Items = new TypeLevelList<RepeatedItem>(2)
            {
                new RepeatedItem { Id = 4 },
                new RepeatedItem { Id = 5 },
            },
        };

#if NET5_0_OR_GREATER
        var bytes = message.ToByteArray();
#else
        var bytes = message.ToByteArray(TypeLevelContract.ProtoWriter);
#endif
        await Assert.That(bytes).IsEquivalentTo(new byte[] { 10, 12, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 18, 2, 8, 4, 18, 2, 8, 5 });

#if NET5_0_OR_GREATER
        var cloned = Serializer.Deserialize<TypeLevelContract>(bytes);
#else
        var cloned = Serializer.Deserialize(bytes, TypeLevelContract.ProtoReader);
#endif

        await Assert.That(cloned.FixedSizeValues).IsEquivalentTo(message.FixedSizeValues);
        await Assert.That(cloned.Items.Select(item => item.Id)).IsEquivalentTo(message.Items.Select(item => item.Id));
    }

    [Test]
    public async Task MemberLevelRepeatedParser_UsesConfiguredReaderAndWriter()
    {
        var message = new MemberLevelContract
        {
            Values = new MemberMappedList<int>(2) { 6, 7 },
        };

#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(message, MemberLevelContract.ProtoReader, MemberLevelContract.ProtoWriter);
#endif

        await Assert.That(cloned.Values).IsEquivalentTo(message.Values);
    }

    [Test]
    public async Task ContractLevelRepeatedParserMap_UsesClosedCollectionAndParserTypes()
    {
        var message = new ContractLevelMapContract
        {
            Values = new ContractMappedList<int>(2) { 8, 9 },
        };

#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(message, ContractLevelMapContract.ProtoReader, ContractLevelMapContract.ProtoWriter);
#endif

        await Assert.That(cloned.Values).IsEquivalentTo(message.Values);
    }

    [Test]
    public async Task AssemblyLevelRepeatedParserMap_UsesGenericCollectionAndParserTypes()
    {
        var message = new AssemblyLevelMapContract
        {
            Values = new AssemblyMappedList<int>(2) { 10, 11 },
        };

#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(message, AssemblyLevelMapContract.ProtoReader, AssemblyLevelMapContract.ProtoWriter);
#endif

        await Assert.That(cloned.Values).IsEquivalentTo(message.Values);
    }
}

[ProtoRepeatedParserType(typeof(TypeLevelReader<>), typeof(TypeLevelWriter<>))]
public sealed class TypeLevelList<T> : RepeatedTestList<T>
{
    public TypeLevelList(int capacity)
        : base(capacity) { }
}

public sealed class TypeLevelReader<T> : IEnumerableProtoReader<TypeLevelList<T>, T>
{
    public TypeLevelReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            static capacity => new TypeLevelList<T>(capacity),
            static (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize
        ) { }
}

public sealed class TypeLevelWriter<T> : IEnumerableProtoWriter<TypeLevelList<T>, T>
{
    public TypeLevelWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class MemberMappedList<T> : RepeatedTestList<T>
{
    public MemberMappedList(int capacity)
        : base(capacity) { }
}

public sealed class MemberLevelReader<T> : IEnumerableProtoReader<MemberMappedList<T>, T>
{
    public MemberLevelReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            static capacity => new MemberMappedList<T>(capacity),
            static (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize
        ) { }
}

public sealed class MemberLevelWriter<T> : IEnumerableProtoWriter<MemberMappedList<T>, T>
{
    public MemberLevelWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ContractMappedList<T> : RepeatedTestList<T>
{
    public ContractMappedList(int capacity)
        : base(capacity) { }
}

public sealed class ContractMapReader : IEnumerableProtoReader<ContractMappedList<int>, int>
{
    public ContractMapReader(IProtoReader<int> itemReader, int itemFixedSize)
        : base(
            itemReader,
            static capacity => new ContractMappedList<int>(capacity),
            static (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize
        ) { }
}

public sealed class ContractMapWriter : IEnumerableProtoWriter<ContractMappedList<int>, int>
{
    public ContractMapWriter(IProtoWriter<int> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class AssemblyMappedList<T> : RepeatedTestList<T>
{
    public AssemblyMappedList(int capacity)
        : base(capacity) { }
}

public sealed class AssemblyMapReader<T> : IEnumerableProtoReader<AssemblyMappedList<T>, T>
{
    public AssemblyMapReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            static capacity => new AssemblyMappedList<T>(capacity),
            static (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize
        ) { }
}

public sealed class AssemblyMapWriter<T> : IEnumerableProtoWriter<AssemblyMappedList<T>, T>
{
    public AssemblyMapWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public class RepeatedTestList<T> : IEnumerable<T>
{
    private readonly List<T> _items;

    protected RepeatedTestList(int capacity)
    {
        _items = new List<T>(capacity);
    }

    public int Count => _items.Count;

    public void Add(T item)
    {
        _items.Add(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[ProtoContract]
public partial class TypeLevelContract
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize, IsPacked = true)]
    public TypeLevelList<int> FixedSizeValues { get; set; } = new(0);

    [ProtoMember(2)]
    public TypeLevelList<RepeatedItem> Items { get; set; } = new(0);
}

[ProtoContract]
public partial struct RepeatedItem
{
    [ProtoMember(1)]
    public int Id { get; set; }
}

[ProtoContract]
public partial class MemberLevelContract
{
    [ProtoMember(1)]
    [ProtoRepeatedParserType(typeof(MemberLevelReader<>), typeof(MemberLevelWriter<>))]
    public MemberMappedList<int> Values { get; set; } = new(0);
}

[ProtoContract]
[ProtoRepeatedParserTypeMap(typeof(ContractMappedList<int>), typeof(ContractMapReader), typeof(ContractMapWriter))]
public partial class ContractLevelMapContract
{
    [ProtoMember(1)]
    public ContractMappedList<int> Values { get; set; } = new(0);
}

[ProtoContract]
public partial class AssemblyLevelMapContract
{
    [ProtoMember(1)]
    public AssemblyMappedList<int> Values { get; set; } = new(0);
}
