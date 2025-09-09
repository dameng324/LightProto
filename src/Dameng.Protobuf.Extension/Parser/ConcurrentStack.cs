using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Dameng.Protobuf.Extension;

public class ConcurrentStackProtoWriter<T> : IEnumerableProtoWriter<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public class ConcurrentStackProtoReader<T> : IEnumerableProtoReader<ConcurrentStack<T>, T>
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
