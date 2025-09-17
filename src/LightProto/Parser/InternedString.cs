namespace LightProto.Parser;

public sealed class InternedStringProtoReader : IProtoReader<string>
{
    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public string ParseFrom(ref ReaderContext input)
    {
        return string.Intern(input.ReadString());
    }
}

public sealed class InternedStringProtoParser : IProtoParser<string>
{
    public static IProtoReader<string> Reader { get; } = new InternedStringProtoReader();
    public static IProtoWriter<string> Writer { get; } = new StringProtoWriter();
}
