using System.Runtime.CompilerServices;


namespace Dameng.Protobuf.Parser;

public sealed class EnumProtoReader<T> : IProtoReader<T>
    where T : Enum
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public T ParseFrom(ref ReaderContext input)
    {
        var value = input.ReadEnum();
        return Unsafe.As<int, T>(ref value);
    }
}

public sealed class EnumProtoWriter<T> : IProtoWriter<T>
    where T : Enum
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(T pair)
    {
        return CodedOutputStream.ComputeEnumSize(Unsafe.As<T, int>(ref pair));
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, T value)
    {
        output.WriteEnum(Unsafe.As<T, int>(ref value));
    }
}

public sealed class EnumProtoParser<T> : IProtoParser<T>
    where T : Enum
{
    public static IProtoReader<T> Reader { get; } = new EnumProtoReader<T>();
    public static IProtoWriter<T> Writer { get; } = new EnumProtoWriter<T>();
}
