using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public abstract class BaseDictionaryTest<TKey, TValue>
    where TKey : notnull
{
    [SuppressMessage("Usage", "TUnit0046:Return a `Func<T>` rather than a `<T>`")]
    public abstract IEnumerable<IReadOnlyDictionary<TKey, TValue>> GetDictionary();

    public IEnumerable<Func<IReadOnlyDictionary<TKey, TValue>>> GetArguments()
    {
        foreach (var arg in GetDictionary())
        {
            yield return () => arg;
            yield return () => arg.ToDictionary(kv => kv.Key, kv => kv.Value);
            yield return () =>
                new ReadOnlyDictionary<TKey, TValue>(
                    arg.ToDictionary(kv => kv.Key, kv => kv.Value)
                );
            yield return () => new ConcurrentDictionary<TKey, TValue>(arg);
            yield return () =>
                new SortedDictionary<TKey, TValue>(arg.ToDictionary(kv => kv.Key, kv => kv.Value));
            yield return () => ImmutableDictionary.CreateRange<TKey, TValue>(arg);
            yield return () =>
                new SortedList<TKey, TValue>(arg.ToDictionary(kv => kv.Key, kv => kv.Value));
        }
    }

    [Test]
    [Category("CollectionTest")]
    [MethodDataSource(nameof(GetArguments))]
    [SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
    public async Task DictionarySerializeAndDeserialize(IReadOnlyDictionary<TKey, TValue> original)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, original);
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IReadOnlyDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<Dictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ReadOnlyDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ConcurrentDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<SortedDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ImmutableDictionary<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<SortedList<TKey, TValue>>(ms);
            await Assert.That(parsed).IsEquivalentTo(original);
        }
    }
}
