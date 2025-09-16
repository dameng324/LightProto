using System.Buffers.Binary;

namespace LightProto.Parser;

// message Guid {
// fixed64 lo = 1; // the first 8 bytes of the guid (note:crazy-endian)
// fixed64 hi = 2; // the second 8 bytes of the guid (note:crazy-endian)
// }

[ProtoContract]
[ProtoProxyFor<Guid>]
public partial struct GuidProxy
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
    internal ulong Low { get; set; }

    [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
    internal ulong High { get; set; }

    public static implicit operator Guid(GuidProxy proxy)
    {
        Span<byte> bytes = stackalloc byte[16];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, proxy.Low);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8), proxy.High);
        return new Guid(bytes);
    }

    public static implicit operator GuidProxy(Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        value.TryWriteBytes(bytes);
        return new GuidProxy()
        {
            Low = BinaryPrimitives.ReadUInt64LittleEndian(bytes),
            High = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(8)),
        };
    }
}

public sealed class GuidProtoParser : IProtoParser<Guid>
{
    public static IProtoReader<Guid> Reader { get; } = LightProto.Parser.GuidProxy.Reader;
    public static IProtoWriter<Guid> Writer { get; } = LightProto.Parser.GuidProxy.Writer;
}
