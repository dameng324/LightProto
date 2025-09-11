using System.Collections.Concurrent;

namespace Dameng.Protobuf.Parser;

public sealed class ConcurrentBagProtoWriter<T> : IEnumerableProtoWriter<ConcurrentBag<T>,T>
{
    public ConcurrentBagProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public sealed class ConcurrentBagProtoReader<T> : IEnumerableProtoReader<ConcurrentBag<T>,T>
{
    public ConcurrentBagProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag,static (capacity) => new ConcurrentBag<T>(), static (collection,item) => collection.Add(item),itemFixedSize)
    {
    }
}