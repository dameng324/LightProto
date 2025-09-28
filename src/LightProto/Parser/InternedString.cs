namespace LightProto.Parser;

public sealed class InternedStringProtoReader : IProtoReader<string>
{
    

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
    public static IProtoReader<string> ProtoReader { get; } = new InternedStringProtoReader();
    public static IProtoWriter<string> ProtoWriter { get; } = new StringProtoWriter();
}
