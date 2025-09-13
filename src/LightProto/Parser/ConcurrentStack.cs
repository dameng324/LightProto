using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentStackProtoWriter<T> : IEnumerableProtoWriter<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize, bool isPacked,
        uint tag2)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize, isPacked,
            tag2)
    {
    }
}

public sealed class ConcurrentStackProtoReader<T> : IEnumerableProtoReader<ConcurrentStack<T>, T>
{
    public ConcurrentStackProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize, bool isPacked,
        uint tag2)
        : base(
            itemReader,
            tag,
            static _ => new ConcurrentStack<T>(),
            static (collection,item) =>
        {
             collection.Push(item);
             return collection;
        },
            itemFixedSize, isPacked,
            tag2,
            stack => new ConcurrentStack<T>(stack)
        )
    {
    }
}