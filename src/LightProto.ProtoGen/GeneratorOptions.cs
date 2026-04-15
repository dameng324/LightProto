namespace LightProto.ProtoGen;

/// <summary>
/// Controls the C# type shape emitted for each message declaration.
/// </summary>
internal enum TypeShape
{
    /// <summary>Emit <c>partial class</c> (the default).</summary>
    Default = 0,

    /// <summary>Emit <c>partial record</c>.</summary>
    Record,

    /// <summary>Emit <c>partial struct</c>.</summary>
    Struct,

    /// <summary>Emit <c>partial record struct</c>.</summary>
    RecordStruct,
}

/// <summary>
/// Controls how nullability is inferred for non-repeated, non-map fields.
/// </summary>
internal enum NullabilityMode
{
    /// <summary>
    /// Every non-repeated, non-map field is emitted as nullable because proto3 fields are
    /// always optional on the wire (the default).
    /// </summary>
    Default = 0,

    /// <summary>
    /// Only fields explicitly declared with the <c>optional</c> keyword in the <c>.proto</c>
    /// file are emitted as nullable.
    /// </summary>
    StrictOptional,
}

/// <summary>
/// Controls how <c>oneof</c> groups are translated to C# members.
/// </summary>
internal enum OneofHandling
{
    /// <summary>
    /// All <c>oneof</c> fields are emitted as nullable properties regardless of their type
    /// (the default).
    /// </summary>
    Default = 0,

    /// <summary>
    /// A <c>oneof</c> group whose fields are all message-typed is promoted to
    /// <c>[ProtoInclude]</c> attributes on the containing type; the individual oneof fields
    /// are not emitted as properties.
    /// </summary>
    ProtoInclude,
}

/// <summary>
/// Controls how <see cref="LightProtoCSharpGenerator"/> produces C# code.
/// </summary>
internal sealed class GeneratorOptions
{
    /// <summary>
    /// The C# type shape emitted for each message declaration.
    /// Defaults to <see cref="TypeShape.Default"/> (partial class).
    /// </summary>
    public TypeShape TypeShape { get; set; }

    /// <summary>
    /// Controls how nullability is inferred for non-repeated, non-map fields.
    /// Defaults to <see cref="NullabilityMode.Default"/> (all fields nullable).
    /// </summary>
    public NullabilityMode Nullability { get; set; }

    /// <summary>
    /// Controls how <c>oneof</c> groups are translated to C# members.
    /// Defaults to <see cref="OneofHandling.Default"/> (nullable properties).
    /// </summary>
    public OneofHandling OneofHandling { get; set; }

    /// <summary>Returns the C# type keyword for message/record declarations.</summary>
    public string TypeKeyword =>
        TypeShape switch
        {
            TypeShape.Record => "record",
            TypeShape.Struct => "struct",
            TypeShape.RecordStruct => "record struct",
            _ => "class",
        };
}
