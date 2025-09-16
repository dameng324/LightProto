﻿using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentQueueProtoWriter<T> : IEnumerableProtoWriter<ConcurrentQueue<T>, T>
{
    public ConcurrentQueueProtoWriter(
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

public sealed class ConcurrentQueueProtoReader<T> : IEnumerableProtoReader<ConcurrentQueue<T>, T>
{
    public ConcurrentQueueProtoReader(
        IProtoReader<T> itemReader,
        uint tag,
        int itemFixedSize,
        bool isPacked,
        uint tag2
    )
        : base(
            itemReader,
            tag,
            static (size) => new ConcurrentQueue<T>(),
            static (collection, item) =>
            {
                collection.Enqueue(item);
                return collection;
            },
            itemFixedSize,
            isPacked,
            tag2
        ) { }
}
