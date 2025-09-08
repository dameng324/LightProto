namespace Dameng.Protobuf.Extension;


public class StackProtoWriter<T> : IEnumerableProtoWriter<Stack<T>,T>
{
    public StackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public class StackProtoReader<T> : IEnumerableProtoReader<Stack<T>,T>
{
    public StackProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag, static (size) => new Stack<T>(size), static (collection,i, item) => collection.Push(item), itemFixedSize)
    {
    }
}