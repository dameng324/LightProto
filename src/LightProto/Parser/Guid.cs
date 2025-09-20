using System.Buffers.Binary;

namespace LightProto.Parser;

// message Guid {
// fixed64 lo = 1; // the first 8 bytes of the guid (note:crazy-endian)
// fixed64 hi = 2; // the second 8 bytes of the guid (note:crazy-endian)
// }

[ProtoContract]
[ProtoSurrogateFor<Guid>]
public partial struct GuidProtoParser
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
    internal ulong Low { get; set; }

    [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
    internal ulong High { get; set; }

    public static implicit operator Guid(GuidProtoParser protoParser)
    {
        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, protoParser.Low);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8), protoParser.High);
        return new Guid(bytes);
    }

    public static implicit operator GuidProtoParser(Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        value.TryWriteBytes(bytes);
        return new GuidProtoParser()
        {
            Low = BinaryPrimitives.ReadUInt64LittleEndian(bytes),
            High = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(8)),
        };
    }
}
