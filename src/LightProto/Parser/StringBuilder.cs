﻿using System.Text;

namespace LightProto.Parser;

public sealed class StringBuilderProtoReader : IProtoReader<StringBuilder>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

    public StringBuilder ParseFrom(ref ReaderContext input)
    {
        return new StringBuilder(input.ReadString());
    }
}

public sealed class StringBuilderProtoWriter : IProtoWriter<StringBuilder>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

    public int CalculateSize(StringBuilder value)
    {
        int size = 0;
        foreach (var readOnlyMemory in value.GetChunks())
        {
            int byteArraySize = WritingPrimitives.Utf8Encoding.GetByteCount(readOnlyMemory.Span);
            size += CodedOutputStream.ComputeLengthSize(byteArraySize) + byteArraySize;
        }
        return size;
    }

    public void WriteTo(ref WriterContext output, StringBuilder value)
    {
        output.WriteString(value.ToString());
    }
}

public sealed class StringBuilderProtoParser : IProtoParser<StringBuilder>
{
    public static IProtoReader<StringBuilder> ProtoReader { get; } = new StringBuilderProtoReader();
    public static IProtoWriter<StringBuilder> ProtoWriter { get; } = new StringBuilderProtoWriter();
}
