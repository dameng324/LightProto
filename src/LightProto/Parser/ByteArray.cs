﻿namespace LightProto.Parser;

public sealed class ByteArrayProtoReader : IProtoReader<byte[]>
{
    

    public byte[] ParseFrom(ref ReaderContext input)
    {
        var length = input.ReadLength();
        return ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length);
    }
}

public sealed class ByteArrayProtoWriter : IProtoWriter<byte[]>
{
    

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int CalculateSize(byte[] value)
    {
        return CodedOutputStream.ComputeLengthSize(value.Length) + value.Length;
    }

    public void WriteTo(ref WriterContext output, byte[] value)
    {
        output.WriteLength(value.Length);
        WritingPrimitives.WriteRawBytes(ref output.buffer, ref output.state, value);
    }
}

public sealed class ByteArrayProtoParser : IProtoParser<byte[]>
{
    public static IProtoReader<byte[]> ProtoReader { get; } = new ByteArrayProtoReader();
    public static IProtoWriter<byte[]> ProtoWriter { get; } = new ByteArrayProtoWriter();
}
