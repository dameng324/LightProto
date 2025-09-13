

namespace LightProto.Parser;

public sealed class StringProtoReader : IProtoReader<string>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public string ParseFrom(ref ReaderContext input)
    {
        return input.ReadString();
    }
}
public sealed class StringProtoWriter : IProtoWriter<string>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(string value)
    {
        return CodedOutputStream.ComputeStringSize(value);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, string value)
    {
        output.WriteString(value);
    }
}
public sealed class StringProtoParser : IProtoParser<string>
{
    public static IProtoReader<string> Reader { get; } = new StringProtoReader();
    public static IProtoWriter<string> Writer { get; } = new StringProtoWriter();
}