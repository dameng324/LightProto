using System.Diagnostics.CodeAnalysis;

namespace LightProto.Tests.CollectionTest;

[InheritsTests]
public abstract class BaseCollectionTestWithParser<TParser, T> : BaseCollectionTest<T>
    where TParser : IProtoParser<T>
{
    public override IProtoWriter<T> ProtoWriter => TParser.ProtoWriter;
    public override IProtoReader<T> ProtoReader => TParser.ProtoReader;
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
    [MethodDataSource(nameof(GetArguments))]
    [SuppressMessage("Usage", "TUnit0300:Generic type or method may not be AOT-compatible")]
    public async Task ByteListCollectionTest(ICollection<T> original)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, original, ProtoWriter);
        ms.Position = 0;
        var parsed = Serializer.Deserialize<List<T>, T>(ms, ProtoReader);
        await Assert.That(parsed).IsEquivalentTo(original);
    }
}
