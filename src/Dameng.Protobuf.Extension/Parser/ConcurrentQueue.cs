using System.Collections.Concurrent;

namespace Dameng.Protobuf.Extension;

public class ConcurrentQueueProtoWriter<T> : IEnumerableProtoWriter<ConcurrentQueue<T>,T>
{
    public ConcurrentQueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}

public class ConcurrentQueueProtoReader<T> : IEnumerableProtoReader<ConcurrentQueue<T>, T>
{
    public ConcurrentQueueProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag, static (size) => new ConcurrentQueue<T>(), static (collection,i, item) => collection.Enqueue(item), itemFixedSize)
    {
    }
}