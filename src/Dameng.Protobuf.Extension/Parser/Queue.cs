namespace Dameng.Protobuf.Extension;

public class QueueProtoWriter<T> : IEnumerableProtoWriter<Queue<T>,T>
{
    public QueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public class QueueProtoReader<T> : IEnumerableProtoReader<Queue<T>,T>
{
    public QueueProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag, static (size) => new Queue<T>(size), static (collection,i, item) => collection.Enqueue(item), itemFixedSize)
    {
    }
}