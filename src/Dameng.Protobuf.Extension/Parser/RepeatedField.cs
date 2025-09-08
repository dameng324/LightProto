using Google.Protobuf.Collections;

namespace Dameng.Protobuf.Extension;

public class RepeatedFieldProtoWriter<T> : IEnumerableProtoWriter<RepeatedField<T>,T>
{
    public RepeatedFieldProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count,  itemFixedSize)
    {
    }
}
public class RepeatedFieldProtoReader<T> : IEnumerableProtoReader<RepeatedField<T>,T>
{
    public RepeatedFieldProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(itemReader, tag, static capacity => new RepeatedField<T>(), static (collection,i, item) => collection.Add(item), itemFixedSize)
    {
    }
}