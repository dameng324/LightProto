using System.Runtime.CompilerServices;

namespace Dameng.Protobuf.Extension;

using Google.Protobuf;

public class EnumProtoReader<T> : IProtoReader<T>
    where T : Enum
{
    public T ParseFrom(ref ParseContext input)
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

    public void WriteTo(ref WriteContext output, T value)
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
