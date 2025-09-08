namespace Dameng.Protobuf.Extension;

public class HashSetProtoWriter<T> : IEnumerableProtoWriter<HashSet<T>,T>
{
    public HashSetProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public class HashSetProtoReader<T> : IEnumerableProtoReader<HashSet<T>,T>
{
    public HashSetProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag, static (size) => new HashSet<T>(size), static (collection,i, item) => collection.Add(item), itemFixedSize)
    {
    }
}