using System.Diagnostics.CodeAnalysis;

namespace LightProto
{
    public interface IProtoParser<T>
    {
#if NET7_0_OR_GREATER
        public static abstract IProtoReader<T> ProtoReader { get; }
        public static abstract IProtoWriter<T> ProtoWriter { get; }
#endif
    }

    public interface IProtoReader<out T>
    {
        public WireFormat.WireType WireType { get; }
        public bool IsMessage { get; }
        public T ParseFrom(ref ReaderContext input);
    }

    public interface IProtoWriter<in T>
    {
        public WireFormat.WireType WireType { get; }
        public bool IsMessage { get; }
        public int CalculateSize(T value);
        public void WriteTo(ref WriterContext output, T value);
    }

    public interface IProtoReader
    {
        public WireFormat.WireType WireType { get; }
        public bool IsMessage { get; }
        public object ParseFrom(ref ReaderContext input);
    }

    public interface IProtoWriter
    {
        public WireFormat.WireType WireType { get; }
        public bool IsMessage { get; }
        public int CalculateSize(object value);
        public void WriteTo(ref WriterContext output, object value);
    }
}
