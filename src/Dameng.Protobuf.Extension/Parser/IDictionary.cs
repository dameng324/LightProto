using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public class IDictionaryProtoWriter<TKey, TValue> : IProtoWriter<IDictionary<TKey, TValue>>
{
    public int CalculateSize(IDictionary<TKey, TValue> value)
    {
        throw new NotImplementedException();
    }

    public void WriteTo(ref WriteContext output, IDictionary<TKey, TValue> value)
    {
        throw new NotImplementedException();
    }
}
public class IDictionaryProtoReader<TKey, TValue> : IProtoReader<IDictionary<TKey, TValue>>
{
    public IDictionary<TKey, TValue> ParseFrom(ref ParseContext input)
    {
        throw new NotImplementedException();
    }
}