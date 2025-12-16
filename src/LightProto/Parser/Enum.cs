using System.Runtime.CompilerServices;

namespace LightProto.Parser;

public sealed class EnumProtoReader<T> : IProtoReader<T>
    where T : Enum
{
    public static EnumProtoReader<T> Default { get; } = new();

    public WireFormat.WireType WireType => WireFormat.WireType.Varint;
    public bool IsMessage => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ParseFrom(ref ReaderContext input)
    {
        var value = input.ReadEnum();
        return Unsafe.As<int, T>(ref value);
    }
}

public sealed class EnumProtoWriter<T> : IProtoWriter<T>
    where T : Enum
{
    public static EnumProtoWriter<T> Default { get; } = new();
    public WireFormat.WireType WireType => WireFormat.WireType.Varint;
    public bool IsMessage => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(T value)
    {
        return CodedOutputStream.ComputeEnumSize(Unsafe.As<T, int>(ref value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, T value)
    {
        output.WriteEnum(Unsafe.As<T, int>(ref value));
    }
}
