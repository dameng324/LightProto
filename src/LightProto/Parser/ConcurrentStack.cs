using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentStackProtoWriter<T> : IEnumerableProtoWriter<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ConcurrentStackProtoReader<T> : IEnumerableProtoReader<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            itemFixedSize,
            static items =>
            {
                items.Reverse();
                return new(items);
            },
            new()
        ) { }
}
