namespace LightProto.Parser;

public sealed class QueueProtoWriter<T> : IEnumerableProtoWriter<Queue<T>,T>
{
    public QueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize,bool isPacked)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize,isPacked)
    {
    }
}
public sealed class QueueProtoReader<T> : IEnumerableProtoReader<Queue<T>,T>
{
    public QueueProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize,bool isPacked)
        : base(itemReader, tag,static (size) => new Queue<T>(size), static (collection,item) =>
        {
             collection.Enqueue(item);
             return collection;
        }, itemFixedSize,isPacked)
    {
    }
}