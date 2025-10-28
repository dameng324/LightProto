using LightProto.Internal;

namespace LightProto.Parser;

public sealed class ArrayProtoWriter<T> : IEnumerableProtoWriter<T[], T>
{
    public ArrayProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Length, itemFixedSize) { }
}

public sealed class ArrayProtoReader<T> : IEnumerableProtoReader<T[], T>
{
    public ArrayProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(itemReader, itemFixedSize, static items => items.ToArray(), []) { }
}
