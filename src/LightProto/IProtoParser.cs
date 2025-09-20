namespace LightProto;

public interface IProtoParser<T>
{
    public static abstract IProtoReader<T> ProtoReader { get; }
    public static abstract IProtoWriter<T> ProtoWriter { get; }
}

public interface IProtoReader<out T>
{
    public WireFormat.WireType WireType { get; }
    public bool IsMessage => false;
    public T ParseFrom(ref ReaderContext input);
}

public interface IProtoWriter<in T>
{
    public WireFormat.WireType WireType { get; }
    public bool IsMessage => false;
    public int CalculateSize(T value);
    public void WriteTo(ref WriterContext output, T value);
}
