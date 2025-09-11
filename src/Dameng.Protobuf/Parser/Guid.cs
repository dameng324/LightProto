
namespace Dameng.Protobuf.Parser;

public sealed class GuidProtoReader : IProtoReader<Guid>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public Guid ParseFrom(ref ReaderContext input)
    {
        var length = input.ReadLength();
        return new Guid(ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state,length));
    }
}
public sealed class GuidProtoWriter : IProtoWriter<Guid>
{
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public int CalculateSize(Guid value)
    {
        return CodedOutputStream.ComputeLengthSize(16)+16;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public void WriteTo(ref WriterContext output, Guid value)
    {
        unsafe
        {
            output.WriteLength(16);
            Span<byte> span = stackalloc byte[16];
            value.TryWriteBytes(span);
#pragma warning disable CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
            WritingPrimitives.WriteRawBytes(ref output.buffer,ref output.state, span);
#pragma warning restore CS9080 // Use of variable in this context may expose referenced variables outside of their declaration scope
        }
    }
}
public sealed class GuidProtoParser : IProtoParser<Guid>
{
    public static IProtoReader<Guid> Reader { get; } = new GuidProtoReader();
    public static IProtoWriter<Guid> Writer { get; } = new GuidProtoWriter();
}