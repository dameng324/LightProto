namespace Dameng.Protobuf.Extension;

public class ArrayProtoWriter<T> : IEnumerableProtoWriter<T[],T>
{
    public ArrayProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Length, itemFixedSize)
    {
    }
}
public class ArrayProtoReader<T> : IEnumerableProtoReader<T[],T>
{
    public ArrayProtoReader(IProtoReader<T> itemReader, uint tag,int itemFixedSize = 0)
        : base(itemReader, tag, static (capacity) => new T[capacity],static (array,i,item)=>array[i]=item,itemFixedSize:itemFixedSize)
    {
    }
}