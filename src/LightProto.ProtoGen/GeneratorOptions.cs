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
/// Controls how proto names are converted to C# identifiers.
/// </summary>
internal enum CaseStyle
{
    /// <summary>Convert to PascalCase.</summary>
    Pascal = 0,

    /// <summary>Convert to camelCase.</summary>
    Camel,

    /// <summary>Preserve the original proto name.</summary>
    Preserve,
}

/// <summary>
/// Controls the C# collection type emitted for repeated fields.
/// </summary>
internal enum RepeatedFieldType
{
    /// <summary>Emit <c>List&lt;T&gt;</c> (the default).</summary>
    List = 0,

    /// <summary>Emit <c>T[]</c>.</summary>
    Array,

    /// <summary>Emit <c>HashSet&lt;T&gt;</c>.</summary>
    HashSet,

    /// <summary>Emit <c>IList&lt;T&gt;</c>.</summary>
    IList,

    /// <summary>Emit <c>IReadOnlyList&lt;T&gt;</c>.</summary>
    IReadOnlyList,

    /// <summary>Emit <c>ICollection&lt;T&gt;</c>.</summary>
    ICollection,

    /// <summary>Emit <c>IReadOnlyCollection&lt;T&gt;</c>.</summary>
    IReadOnlyCollection,

    /// <summary>Emit <c>IEnumerable&lt;T&gt;</c>.</summary>
    IEnumerable,

    /// <summary>Emit <c>ISet&lt;T&gt;</c>.</summary>
    ISet,

    /// <summary>Emit <c>Queue&lt;T&gt;</c>.</summary>
    Queue,

    /// <summary>Emit <c>Stack&lt;T&gt;</c>.</summary>
    Stack,

    /// <summary>Emit <c>LinkedList&lt;T&gt;</c>.</summary>
    LinkedList,

    /// <summary>Emit <c>SortedSet&lt;T&gt;</c>.</summary>
    SortedSet,

    /// <summary>Emit <c>Collection&lt;T&gt;</c>.</summary>
    Collection,

    /// <summary>Emit <c>ReadOnlyCollection&lt;T&gt;</c>.</summary>
    ReadOnlyCollection,

    /// <summary>Emit <c>ObservableCollection&lt;T&gt;</c>.</summary>
    ObservableCollection,

    /// <summary>Emit <c>ReadOnlyObservableCollection&lt;T&gt;</c>.</summary>
    ReadOnlyObservableCollection,

    /// <summary>Emit <c>ConcurrentQueue&lt;T&gt;</c>.</summary>
    ConcurrentQueue,

    /// <summary>Emit <c>ConcurrentStack&lt;T&gt;</c>.</summary>
    ConcurrentStack,

    /// <summary>Emit <c>ConcurrentBag&lt;T&gt;</c>.</summary>
    ConcurrentBag,

    /// <summary>Emit <c>BlockingCollection&lt;T&gt;</c>.</summary>
    BlockingCollection,

    /// <summary>Emit <c>ImmutableArray&lt;T&gt;</c>.</summary>
    ImmutableArray,

    /// <summary>Emit <c>ImmutableList&lt;T&gt;</c>.</summary>
    ImmutableList,

    /// <summary>Emit <c>ImmutableHashSet&lt;T&gt;</c>.</summary>
    ImmutableHashSet,

    /// <summary>Emit <c>ImmutableSortedSet&lt;T&gt;</c>.</summary>
    ImmutableSortedSet,

    /// <summary>Emit <c>ImmutableQueue&lt;T&gt;</c>.</summary>
    ImmutableQueue,

    /// <summary>Emit <c>ImmutableStack&lt;T&gt;</c>.</summary>
    ImmutableStack,

    /// <summary>Emit <c>FrozenSet&lt;T&gt;</c>.</summary>
    FrozenSet,
}

/// <summary>
/// Controls the C# dictionary type emitted for map fields.
/// </summary>
internal enum MapFieldType
{
    /// <summary>Emit <c>Dictionary&lt;TKey, TValue&gt;</c> (the default).</summary>
    Dictionary = 0,

    /// <summary>Emit <c>IDictionary&lt;TKey, TValue&gt;</c>.</summary>
    IDictionary,

    /// <summary>Emit <c>IReadOnlyDictionary&lt;TKey, TValue&gt;</c>.</summary>
    IReadOnlyDictionary,

    /// <summary>Emit <c>SortedDictionary&lt;TKey, TValue&gt;</c>.</summary>
    SortedDictionary,

    /// <summary>Emit <c>SortedList&lt;TKey, TValue&gt;</c>.</summary>
    SortedList,

    /// <summary>Emit <c>ConcurrentDictionary&lt;TKey, TValue&gt;</c>.</summary>
    ConcurrentDictionary,

    /// <summary>Emit <c>ReadOnlyDictionary&lt;TKey, TValue&gt;</c>.</summary>
    ReadOnlyDictionary,

    /// <summary>Emit <c>ImmutableDictionary&lt;TKey, TValue&gt;</c>.</summary>
    ImmutableDictionary,

    /// <summary>Emit <c>ImmutableSortedDictionary&lt;TKey, TValue&gt;</c>.</summary>
    ImmutableSortedDictionary,

    /// <summary>Emit <c>FrozenDictionary&lt;TKey, TValue&gt;</c>.</summary>
    FrozenDictionary,
}

/// <summary>
/// A naming rule that maps a proto FullName glob <see cref="Pattern"/> to a <see cref="Style"/>.
/// </summary>
/// <param name="Pattern">A glob pattern matched against proto FullName.</param>
/// <param name="Style">The style to apply when matched.</param>
internal sealed record CaseStyleRule(string Pattern, CaseStyle Style);

/// <summary>
/// A type-shape rule that maps a proto repeated-field FullName glob <see cref="Pattern"/> to a <see cref="Type"/>.
/// </summary>
internal sealed record RepeatedFieldTypeRule(string Pattern, RepeatedFieldType Type);

/// <summary>
/// A type-shape rule that maps a proto map-field FullName glob <see cref="Pattern"/> to a <see cref="Type"/>.
/// </summary>
internal sealed record MapFieldTypeRule(string Pattern, MapFieldType Type);

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

    /// <summary>
    /// Default case style applied when no <see cref="CaseStyleRules"/> match.
    /// Defaults to <see cref="CaseStyle.Pascal"/>.
    /// </summary>
    public CaseStyle DefaultCaseStyle { get; set; } = CaseStyle.Pascal;

    /// <summary>
    /// Ordered glob rules matched against proto FullName.
    /// When multiple rules match, the most specific one wins; ties are resolved by later rules.
    /// </summary>
    public List<CaseStyleRule> CaseStyleRules { get; } = [];

    /// <summary>
    /// Default repeated-field collection type applied when no <see cref="RepeatedFieldTypeRules"/> match.
    /// Defaults to <see cref="RepeatedFieldType.List"/>.
    /// </summary>
    public RepeatedFieldType DefaultRepeatedType { get; set; } = RepeatedFieldType.List;

    /// <summary>
    /// Ordered glob rules matched against proto repeated-field FullName.
    /// When multiple rules match, the most specific one wins; ties are resolved by later rules.
    /// </summary>
    public List<RepeatedFieldTypeRule> RepeatedFieldTypeRules { get; } = [];

    /// <summary>
    /// Default map-field dictionary type applied when no <see cref="MapFieldTypeRules"/> match.
    /// Defaults to <see cref="MapFieldType.Dictionary"/>.
    /// </summary>
    public MapFieldType DefaultMapType { get; set; } = MapFieldType.Dictionary;

    /// <summary>
    /// Ordered glob rules matched against proto map-field FullName.
    /// When multiple rules match, the most specific one wins; ties are resolved by later rules.
    /// </summary>
    public List<MapFieldTypeRule> MapFieldTypeRules { get; } = [];

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
