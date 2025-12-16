using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public abstract class BaseCollectionTest<T>
{
    [SuppressMessage("Usage", "TUnit0046:Return a `Func<T>` rather than a `<T>`")]
    public abstract IEnumerable<T[]> GetCollection();

    public IEnumerable<Func<ICollection<T>>> GetArguments()
    {
        foreach (var arg in GetCollection())
        {
            yield return () => arg;
            yield return () => new List<T>(arg);
            yield return () => new HashSet<T>(arg);
        }
    }

    protected virtual IEqualityComparer<T> Comparer { get; } = EqualityComparer<T>.Default;

    [Test]
    [Category("CollectionTest")]
    [MethodDataSource(nameof(GetArguments))]
    [SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
    public async Task CollectionSerializeAndDeserialize(ICollection<T> original)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, original);
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ICollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IEnumerable<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IList<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IReadOnlyList<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<IReadOnlyCollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<List<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<HashSet<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<T[]>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ConcurrentQueue<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ConcurrentBag<T>>(ms);
            // ConcurrentBag<T> does not guarantee enumeration order,
            // so we use CollectionOrdering.Any to ignore order during comparison.
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Any);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<Collection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ReadOnlyCollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ObservableCollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<ReadOnlyObservableCollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<LinkedList<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize<BlockingCollection<T>>(ms);
            await Assert
                .That(parsed)
                .IsEquivalentTo(original, Comparer, ordering: CollectionOrdering.Matching);
        }
    }
}
