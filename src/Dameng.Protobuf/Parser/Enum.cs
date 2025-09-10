using System.Runtime.CompilerServices;


namespace Dameng.Protobuf.Parser;

public class EnumProtoReader<T> : IProtoReader<T>
    where T : Enum
{
    public T ParseFrom(ref ReaderContext input)
    {
        var value = input.ReadEnum();
        return Unsafe.As<int, T>(ref value);
    }
}

public class EnumProtoWriter<T> : IProtoWriter<T>
    where T : Enum
{
    public int CalculateSize(T value)
    {
        return CodedOutputStream.ComputeEnumSize(Unsafe.As<T, int>(ref value));
    }

    public void WriteTo(ref WriterContext output, T value)
    {
        output.WriteEnum(Unsafe.As<T, int>(ref value));
    }
}

public class EnumProtoParser<T> : IProtoParser<T>
    where T : Enum
{
    public static IProtoReader<T> Reader { get; } = new EnumProtoReader<T>();
    public static IProtoWriter<T> Writer { get; } = new EnumProtoWriter<T>();
}
