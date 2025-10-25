using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public abstract class BaseCollectionTestWithParser<TParser, T> : BaseCollectionTest<T>
    where TParser : IProtoParser<T>
{
#if NET7_0_OR_GREATER
    public override IProtoReader<T> ProtoReader => TParser.ProtoReader;
    public override IProtoWriter<T> ProtoWriter => TParser.ProtoWriter;
#else
    public override IProtoReader<T> ProtoReader =>
        (typeof(TParser).GetProperty("ProtoReader")!.GetValue(null) as IProtoReader<T>)!;
    public override IProtoWriter<T> ProtoWriter =>
        (typeof(TParser).GetProperty("ProtoWriter")!.GetValue(null) as IProtoWriter<T>)!;
#endif
}

public abstract class BaseCollectionTest<T>
{
    public abstract IProtoWriter<T> ProtoWriter { get; }
    public abstract IProtoReader<T> ProtoReader { get; }

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

    [Test]
    [Category("CollectionTest")]
    [MethodDataSource(nameof(GetArguments))]
    [SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
    public async Task CollectionSerializeAndDeserialize(ICollection<T> original)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, original, ProtoWriter.GetCollectionWriter());
        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetCollectionReader<List<T>, T>());
            await Assert.That(parsed).IsEquivalentTo(original);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetListReader());
            await Assert.That(parsed).IsEquivalentTo(original);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetHashSetReader());
            await Assert.That(parsed).IsEquivalentTo(original);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetArrayReader());
            await Assert.That(parsed).IsEquivalentTo(original);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetConcurrentQueueReader());
            await Assert.That(parsed).IsEquivalentTo(original);
        }

        {
            ms.Position = 0;
            var parsed = Serializer.Deserialize(ms, ProtoReader.GetConcurrentBagReader());
            // ConcurrentBag<T> enumerates items in reverse order of insertion,
            // so we reverse the result to match the original collection's order for comparison.
            await Assert.That(parsed.Reverse()).IsEquivalentTo(original);
        }
    }
}
