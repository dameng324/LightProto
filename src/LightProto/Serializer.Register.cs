using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using LightProto.Parser;

namespace LightProto;

public static partial class Serializer
{
    static readonly ConcurrentDictionary<Type, object> Readers = new();
    static readonly ConcurrentDictionary<Type, object> Writers = new();
    static readonly ConcurrentDictionary<Type, Type> GenericReaderTypes = new();
    static readonly ConcurrentDictionary<Type, Type> GenericWriterTypes = new();

    /// <summary>
    /// Registers a custom ProtoReader and ProtoWriter for type T
    /// </summary>
    public static void RegisterParser<T>(IProtoReader<T> reader, IProtoWriter<T> writer)
    {
        Readers.TryAdd(typeof(T), reader);
        Writers.TryAdd(typeof(T), writer);
    }

    static void RegisterGenericParser(Type type,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type readerType,
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type writerType)
    {
        GenericReaderTypes.TryAdd(type, readerType);
        GenericWriterTypes.TryAdd(type, writerType);
    }

    static Serializer()
    {
        RegisterParser(BigIntegerProtoParser.ProtoReader, BigIntegerProtoParser.ProtoWriter);
        RegisterParser(BitArrayProtoParser.ProtoReader, BitArrayProtoParser.ProtoWriter);
        RegisterParser(BooleanProtoParser.ProtoReader, BooleanProtoParser.ProtoWriter);
        RegisterParser(ByteArrayProtoParser.ProtoReader, ByteArrayProtoParser.ProtoWriter);
        RegisterParser(ByteListProtoParser.ProtoReader, ByteListProtoParser.ProtoWriter);
        RegisterParser(ComplexProtoParser.ProtoReader, ComplexProtoParser.ProtoWriter);
        RegisterParser(CultureInfoProtoParser.ProtoReader, CultureInfoProtoParser.ProtoWriter);
        RegisterParser(DateTimeProtoParser.ProtoReader, DateTimeProtoParser.ProtoWriter);
        RegisterParser(
            DateTimeOffsetProtoParser.ProtoReader,
            DateTimeOffsetProtoParser.ProtoWriter
        );
        RegisterParser(DecimalProtoParser.ProtoReader, DecimalProtoParser.ProtoWriter);
        RegisterParser(GuidProtoParser.ProtoReader, GuidProtoParser.ProtoWriter);
        RegisterParser(TimeSpanProtoParser.ProtoReader, TimeSpanProtoParser.ProtoWriter);
        RegisterParser(UriProtoParser.ProtoReader, UriProtoParser.ProtoWriter);
        RegisterParser(DoubleProtoParser.ProtoReader, DoubleProtoParser.ProtoWriter);
        RegisterParser(SingleProtoParser.ProtoReader, SingleProtoParser.ProtoWriter);

        RegisterParser(Int32ProtoParser.ProtoReader, Int32ProtoParser.ProtoWriter);
        RegisterParser(UInt32ProtoParser.ProtoReader, UInt32ProtoParser.ProtoWriter);
        RegisterParser(Int64ProtoParser.ProtoReader, Int64ProtoParser.ProtoWriter);
        RegisterParser(UInt64ProtoParser.ProtoReader, UInt64ProtoParser.ProtoWriter);
        RegisterParser(StringProtoParser.ProtoReader, StringProtoParser.ProtoWriter);
        RegisterParser(StringBuilderProtoParser.ProtoReader, StringBuilderProtoParser.ProtoWriter);
        RegisterParser(TimeZoneInfoProtoParser.ProtoReader, TimeZoneInfoProtoParser.ProtoWriter);
        RegisterParser(VersionProtoParser.ProtoReader, VersionProtoParser.ProtoWriter);
#if NET6_0_OR_GREATER
        RegisterParser(DateOnlyProtoParser.ProtoReader, DateOnlyProtoParser.ProtoWriter);
        RegisterParser(TimeOnlyProtoParser.ProtoReader, TimeOnlyProtoParser.ProtoWriter);
        RegisterParser(Int128ProtoParser.ProtoReader, Int128ProtoParser.ProtoWriter);
        RegisterParser(UInt128ProtoParser.ProtoReader, UInt128ProtoParser.ProtoWriter);
        RegisterParser(HalfProtoParser.ProtoReader, HalfProtoParser.ProtoWriter);
        RegisterParser(Matrix3x2ProtoParser.ProtoReader, Matrix3x2ProtoParser.ProtoWriter);
        RegisterParser(Matrix4x4ProtoParser.ProtoReader, Matrix4x4ProtoParser.ProtoWriter);
        RegisterParser(PlaneProtoParser.ProtoReader, PlaneProtoParser.ProtoWriter);
        RegisterParser(QuaternionProtoParser.ProtoReader, QuaternionProtoParser.ProtoWriter);
        RegisterParser(RuneProtoParser.ProtoReader, RuneProtoParser.ProtoWriter);
        RegisterParser(Vector2ProtoParser.ProtoReader, Vector2ProtoParser.ProtoWriter);
        RegisterParser(Vector3ProtoParser.ProtoReader, Vector3ProtoParser.ProtoWriter);
#endif
        RegisterGenericParser(typeof(List<>), typeof(ListProtoReader<>), typeof(ListProtoWriter<>));
        RegisterGenericParser(
            typeof(Nullable<>),
            typeof(NullableProtoReader<>),
            typeof(NullableProtoWriter<>)
        );
        RegisterGenericParser(typeof(Lazy<>), typeof(LazyProtoReader<>), typeof(LazyProtoWriter<>));
        RegisterGenericParser(
            typeof(Stack<>),
            typeof(StackProtoReader<>),
            typeof(StackProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(Queue<>),
            typeof(QueueProtoReader<>),
            typeof(QueueProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(Collection<>),
            typeof(CollectionProtoReader<>),
            typeof(CollectionProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(BlockingCollection<>),
            typeof(BlockingCollectionProtoReader<>),
            typeof(BlockingCollectionProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ConcurrentBag<>),
            typeof(ConcurrentBagProtoReader<>),
            typeof(ConcurrentBagProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ConcurrentQueue<>),
            typeof(ConcurrentQueueProtoReader<>),
            typeof(ConcurrentQueueProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ConcurrentStack<>),
            typeof(ConcurrentStackProtoReader<>),
            typeof(ConcurrentStackProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(HashSet<>),
            typeof(HashSetProtoReader<>),
            typeof(HashSetProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ImmutableArray<>),
            typeof(ImmutableArrayProtoReader<>),
            typeof(ImmutableArrayProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ImmutableList<>),
            typeof(ImmutableListProtoReader<>),
            typeof(ImmutableListProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ImmutableHashSet<>),
            typeof(ImmutableHashSetProtoReader<>),
            typeof(ImmutableHashSetProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(LinkedList<>),
            typeof(LinkedListProtoReader<>),
            typeof(LinkedListProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ObservableCollection<>),
            typeof(ObservableCollectionProtoReader<>),
            typeof(ObservableCollectionProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ReadOnlyCollection<>),
            typeof(ReadOnlyCollectionProtoReader<>),
            typeof(ReadOnlyCollectionProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(ReadOnlyObservableCollection<>),
            typeof(ReadOnlyObservableCollectionProtoReader<>),
            typeof(ReadOnlyObservableCollectionProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(SortedSet<>),
            typeof(SortedSetProtoReader<>),
            typeof(SortedSetProtoWriter<>)
        );
        RegisterGenericParser(
            typeof(Dictionary<,>),
            typeof(DictionaryProtoReader<,>),
            typeof(DictionaryProtoWriter<,>)
        );
        RegisterGenericParser(
            typeof(ReadOnlyDictionary<,>),
            typeof(ReadOnlyDictionaryProtoReader<,>),
            typeof(ReadOnlyDictionaryProtoWriter<,>)
        );
        RegisterGenericParser(
            typeof(ConcurrentDictionary<,>),
            typeof(ConcurrentDictionaryProtoReader<,>),
            typeof(ConcurrentDictionaryProtoWriter<,>)
        );
        RegisterGenericParser(
            typeof(SortedDictionary<,>),
            typeof(SortedDictionaryProtoReader<,>),
            typeof(SortedDictionaryProtoWriter<,>)
        );
        RegisterGenericParser(
            typeof(SortedList<,>),
            typeof(SortedListProtoReader<,>),
            typeof(SortedListProtoWriter<,>)
        );
        RegisterGenericParser(
            typeof(ImmutableDictionary<,>),
            typeof(ImmutableDictionaryProtoReader<,>),
            typeof(ImmutableDictionaryProtoWriter<,>)
        );
    }

#if NET7_0_OR_GREATER
    private const DynamicallyAccessedMemberTypes LightProtoRequiredMembers =
        DynamicallyAccessedMemberTypes.All;
#endif

    internal static IProtoReader<T> GetProtoReader<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        T>() => (IProtoReader<T>)GetProtoParser(typeof(T), isReader: true);

    internal static IProtoWriter<T> GetProtoWriter<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        T>()
    {
        return (IProtoWriter<T>)GetProtoParser(typeof(T), isReader: false);
    }

    static object GetProtoParser(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(LightProtoRequiredMembers)]
#endif
        Type type, bool isReader)
    {
        var parsers = isReader ? Readers : Writers;
        if (parsers.TryGetValue(type, out var obj))
        {
            return obj;
        }

        if (typeof(IProtoParser<>).MakeGenericType(type).IsAssignableFrom(type))
        {
            if (isReader)
            {
                var parser = type.GetProperty(
                            "ProtoReader",
                            BindingFlags.Public | BindingFlags.Static
                        )!
                    .GetValue(null)!;
                parsers.TryAdd(type, parser);
                return parser;
            }
            else
            {
                var parser = type.GetProperty(
                            "ProtoWriter",
                            BindingFlags.Public | BindingFlags.Static
                        )!
                    .GetValue(null)!;
                parsers.TryAdd(type, parser);
                return parser;
            }
        }

        if (type.IsEnum)
        {
            var parserType = isReader ? typeof(EnumProtoReader<>) : typeof(EnumProtoWriter<>);
            var enumReaderType = parserType.MakeGenericType(type);
            var enumReader = Activator.CreateInstance(enumReaderType)!;
            parsers.TryAdd(type, enumReader);
            return enumReader;
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var elementParser = GetProtoParser(elementType, isReader);

            var parserType = isReader ? typeof(ArrayProtoReader<>) : typeof(ArrayProtoWriter<>);
            var arrayParserType = parserType.MakeGenericType(elementType);
            uint tag = 0;
            if (isReader == false)
            {
                // For writers, we need to provide a tag to indicate the field wire type
                var writer = (IProtoWriter)elementParser;
                tag = WireFormat.MakeTag(1, writer.WireType);
            }
            var arrayParser = Activator.CreateInstance(arrayParserType, elementParser, tag, 0)!;
            parsers.TryAdd(type, arrayParser);
            return arrayParser;
        }

        if (type.IsGenericType == false)
        {
            throw new InvalidOperationException($"No ProtoParser registered for type {type}");
        }

        var genericDef = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments();

        var genericTypes = isReader ? GenericReaderTypes : GenericWriterTypes;

        if (genericArguments.Length == 1)
        {
            var itemType = genericArguments[0];
            var itemParser = GetProtoParser(itemType, isReader);
            if (genericTypes.TryGetValue(genericDef, out var genericParserType) == false)
            {
                if (
                    isReader
                    && typeof(IEnumerable<>).MakeGenericType(itemType).IsAssignableFrom(type)
                )
                    genericParserType = typeof(ListProtoReader<>);
                else if (
                    isReader == false
                    && typeof(ICollection<>).MakeGenericType(itemType).IsAssignableFrom(type)
                )
                    genericParserType = typeof(ICollectionProtoWriter<>);
                else
                    throw new InvalidOperationException(
                        $"No ProtoParser registered for type {genericDef}"
                    );
            }
            var parserType = genericParserType.MakeGenericType(itemType);
            uint tag = 0;
            if (isReader == false)
            {
                // For writers, we need to provide a tag to indicate the field wire type
                var writer = (IProtoWriter)itemParser;
                tag = WireFormat.MakeTag(1, writer.WireType);
            }

            object parser;
            if (
                genericParserType == typeof(NullableProtoWriter<>)
                || genericParserType == typeof(NullableProtoReader<>)
                || genericParserType == typeof(LazyProtoReader<>)
                || genericParserType == typeof(LazyProtoWriter<>)
            )
            {
                parser = Activator.CreateInstance(parserType, itemParser)!;
            }
            else
            {
                parser = Activator.CreateInstance(parserType, itemParser, tag, 0)!;
            }

            parsers.TryAdd(type, parser);
            return parser;
        }

        if (genericArguments.Length == 2)
        {
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];
            if (genericTypes.TryGetValue(genericDef, out var genericParserType) == false)
            {
                if (
                    isReader
                    && typeof(IEnumerable<>)
                        .MakeGenericType(
                            typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType)
                        )
                        .IsAssignableFrom(type)
                )
                    genericParserType = typeof(DictionaryProtoReader<,>);
                else if (
                    isReader == false
                    && typeof(IReadOnlyDictionary<,>)
                        .MakeGenericType(keyType, valueType)
                        .IsAssignableFrom(type)
                )
                    genericParserType = typeof(IReadOnlyDictionaryProtoWriter<,>);
                else
                    throw new InvalidOperationException(
                        $"No ProtoParser registered for type {genericDef}"
                    );
            }

            var keyParser = GetProtoParser(keyType, isReader);
            var valueParser = GetProtoParser(valueType, isReader);
            var parserType = genericParserType.MakeGenericType(keyType, valueType);

            uint tag = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
            var parser = Activator.CreateInstance(parserType, keyParser, valueParser, tag)!;

            parsers.TryAdd(type, parser);
            return parser;
        }

        throw new InvalidOperationException($"No ProtoParser registered for type {type}");
    }
}
