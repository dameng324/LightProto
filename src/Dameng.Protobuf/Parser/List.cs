namespace Dameng.Protobuf.Parser;

public class ListProtoWriter<T> : IEnumerableProtoWriter<List<T>,T>
{
    public ListProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public class ListProtoReader<T> : IEnumerableProtoReader<List<T>,T>
{
    public ListProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag,static capacity => new List<T>(capacity), static (collection,item) => collection.Add(item), itemFixedSize)
    {
    }
}