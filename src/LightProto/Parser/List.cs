namespace LightProto.Parser;

public sealed class ListProtoWriter<T> : IEnumerableProtoWriter<List<T>, T>
{
    public ListProtoWriter(
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

public sealed class ListProtoReader<T> : IEnumerableProtoReader<List<T>, T>
{
    public ListProtoReader(
        IProtoReader<T> itemReader,
        uint tag,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
        : base(
            itemReader,
            tag,
            static capacity => new List<T>(capacity),
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
