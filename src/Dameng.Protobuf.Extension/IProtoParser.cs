using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public interface IProtoParser<T>
{
    public static abstract IProtoReader<T> Reader { get; }
    public static abstract IProtoWriter<T> Writer { get; }
}

public interface IProtoMessage<T> : IProtoParser<T> where T : IProtoParser<T>;

public interface IProtoReader<out T>
{
    public T ParseFrom(ref ParseContext input);
}

public interface IProtoWriter<in T>
{
    public int CalculateSize(T value);
    public void WriteTo(ref WriteContext output, T value);
}
public interface IProtoMessageWriter<in T>: IProtoWriter<T>
{
    public int CalculateMessageSize(T value)
    {
        var size = CalculateSize(value);
        return CodedOutputStream.ComputeLengthSize(size) + size;
    }

    public void WriteMessageTo(ref WriteContext output, T value)
    {
        var size = CalculateSize(value);
        output.WriteLength(size);
        WriteTo(ref output, value);
    }
}