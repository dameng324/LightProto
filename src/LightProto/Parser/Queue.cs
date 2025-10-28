namespace LightProto.Parser;

public sealed class QueueProtoWriter<T> : IEnumerableProtoWriter<Queue<T>, T>
{
    public QueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class QueueProtoReader<T> : IEnumerableProtoReader<Queue<T>, T>
{
    public QueueProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => new(items), new()) { }
}
