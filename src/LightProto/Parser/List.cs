namespace LightProto.Parser;

public sealed class ListProtoWriter<T> : IEnumerableProtoWriter<List<T>, T>
{
    public ListProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ListProtoReader<T> : IEnumerableProtoReader<List<T>, T>
{
    private static new readonly List<T> Empty = new();

    public ListProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => new(items), Empty) { }
}
