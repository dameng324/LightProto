using System.Collections.Concurrent;

namespace Dameng.Protobuf.Parser;

public sealed class ConcurrentStackProtoWriter<T> : IEnumerableProtoWriter<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ConcurrentStackProtoReader<T> : IEnumerableProtoReader<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(
            itemReader,
            tag,
            static capacity => new ConcurrentStack<T>(),
            static (collection, item) => collection.Push(item),
            itemFixedSize,
            stack => new ConcurrentStack<T>(stack)
        ) { }
}
