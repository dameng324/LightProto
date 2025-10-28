﻿using System.Collections.Concurrent;

namespace LightProto.Parser;

public sealed class ConcurrentQueueProtoWriter<T> : IEnumerableProtoWriter<ConcurrentQueue<T>, T>
{
    public ConcurrentQueueProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class ConcurrentQueueProtoReader<T> : IEnumerableProtoReader<ConcurrentQueue<T>, T>
{
    public ConcurrentQueueProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => new(items), new()) { }
}
