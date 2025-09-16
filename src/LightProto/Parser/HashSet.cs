namespace LightProto.Parser;

public sealed class HashSetProtoWriter<T> : IEnumerableProtoWriter<HashSet<T>, T>
{
    public HashSetProtoWriter(
        IProtoWriter<T> itemWriter,
        uint tag,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
        : base(
            itemWriter,
            tag,
            static collection => collection.Count,
            itemFixedSize,
            isPacked,
            tag2
        ) { }
}

public sealed class HashSetProtoReader<T> : IEnumerableProtoReader<HashSet<T>, T>
{
    public HashSetProtoReader(
        IProtoReader<T> itemReader,
        uint tag,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
        : base(
            itemReader,
            tag,
            static (size) => new HashSet<T>(size),
            static (collection, item) =>
            {
                collection.Add(item);
                return collection;
            },
            itemFixedSize,
            isPacked,
            tag2
        ) { }
}
