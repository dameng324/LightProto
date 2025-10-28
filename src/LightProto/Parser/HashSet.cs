namespace LightProto.Parser;

public sealed class HashSetProtoWriter<T> : IEnumerableProtoWriter<HashSet<T>, T>
{
    public HashSetProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class HashSetProtoReader<T> : IEnumerableProtoReader<HashSet<T>, T>
{
    public HashSetProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => new(items), new()) { }
}
