using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentBagProtoWriter<T> : IEnumerableProtoWriter<ConcurrentBag<T>, T>
{
    public ConcurrentBagProtoWriter(
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

public sealed class ConcurrentBagProtoReader<T> : IEnumerableProtoReader<ConcurrentBag<T>, T>
{
    public ConcurrentBagProtoReader(
        IProtoReader<T> itemReader,
        uint tag,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
        : base(
            itemReader,
            tag,
            static (capacity) => new ConcurrentBag<T>(),
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
