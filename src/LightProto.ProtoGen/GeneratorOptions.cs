namespace LightProto.ProtoGen;

/// <summary>
/// Controls how <see cref="LightProtoCSharpGenerator"/> produces C# code.
/// </summary>
internal sealed class GeneratorOptions
{
    /// <summary>
    /// When <see langword="true"/>, emits <c>partial struct</c> instead of <c>partial class</c>.
    /// </summary>
    public bool UseStruct { get; set; }

    /// <summary>
    /// When <see langword="true"/>, emits <c>partial record</c> (or <c>partial record struct</c>
    /// when combined with <see cref="UseStruct"/>).
    /// </summary>
    public bool UseRecord { get; set; }

    /// <summary>
    /// When <see langword="false"/> (the default), every non-repeated, non-map field is emitted
    /// as a nullable type, because proto3 fields are always optional on the wire.
    /// When <see langword="true"/>, only fields explicitly declared with the <c>optional</c>
    /// keyword in the <c>.proto</c> file are emitted as nullable.
    /// </summary>
    public bool StrictOptional { get; set; }

    /// <summary>
    /// When <see langword="true"/>, a <c>oneof</c> group whose fields are all message-typed
    /// is promoted to <c>[ProtoInclude]</c> attributes on the containing type and the
    /// individual oneof fields are not emitted as properties.
    /// The default is <see langword="false"/>, which always emits oneof fields as nullable
    /// properties regardless of their type.
    /// </summary>
    public bool UseProtoIncludeForOneof { get; set; }

    /// <summary>Returns the C# type keyword for message/record declarations.</summary>
    public string TypeKeyword =>
        (UseRecord, UseStruct) switch
        {
            (true, true) => "record struct",
            (true, false) => "record",
            (false, true) => "struct",
            _ => "class",
        };
}
