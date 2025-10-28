using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentBagProtoWriter<T> : IEnumerableProtoWriter<ConcurrentBag<T>, T>
{
    public ConcurrentBagProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ConcurrentBagProtoReader<T> : IEnumerableProtoReader<ConcurrentBag<T>, T>
{
    public ConcurrentBagProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => new ConcurrentBag<T>(items), new()) { }
}
