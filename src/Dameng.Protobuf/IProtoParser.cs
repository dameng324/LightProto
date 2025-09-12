

namespace Dameng.Protobuf;

public interface IProtoParser<T>
{
    public static abstract IProtoReader<T> Reader { get; }
    public static abstract IProtoWriter<T> Writer { get; }
}

public interface IProtoMessage<T> : IProtoParser<T> where T : IProtoParser<T>;

public interface IProtoReader<out T>
{
    public bool IsMessage => false;
    public T ParseFrom(ref ReaderContext input);
}

public interface IProtoWriter<in T>
{
    public bool IsMessage => false;
    public int CalculateSize(T pair);
    public void WriteTo(ref WriterContext output, T value);
}