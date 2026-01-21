# LightProto 🚀

![.NET](https://img.shields.io/badge/netstandard2.0|net8.0|net9.0|net10.0-5C2D91?logo=.NET&labelColor=gray)
[![NuGet](https://img.shields.io/nuget/v/LightProto.svg)](https://www.nuget.org/packages/LightProto/)
[![downloads](https://img.shields.io/nuget/dt/LightProto)](https://www.nuget.org/packages/LightProto)
![Build](https://github.com/dameng324/LightProto/actions/workflows/ci.yml/badge.svg)
[![codecov](https://codecov.io/github/dameng324/lightproto/graph/badge.svg)](https://codecov.io/github/dameng324/lightproto)
[![CodeQL Advanced](https://github.com/dameng324/LightProto/actions/workflows/codeql.yml/badge.svg)](https://github.com/dameng324/LightProto/actions/workflows/codeql.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/27764c82699649fba67d9fc903a0d9d5)](https://app.codacy.com/gh/dameng324/LightProto/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
![GitHub Repo stars](https://img.shields.io/github/stars/dameng324/LightProto.svg?style=flat)
![Size](https://img.shields.io/github/repo-size/dameng324/LightProto.svg)
[![License](https://img.shields.io/github/license/dameng324/LightProto.svg)](LICENSE)

[English](README.md) | [简体中文](README.zh-CN.md)

A high-performance, Native AOT–friendly, production-ready Protocol Buffers implementation for C#/.NET, powered by source generators.

## Why LightProto? 🤔

[protobuf-net](https://github.com/protobuf-net/protobuf-net) is a popular Protocol Buffers implementation in .NET, but some scenarios (especially Native AOT) can be challenging due to runtime reflection and dynamic generation. LightProto addresses this with compile-time code generation and a protobuf-net–style API.

## Features ✨

- Source generator–powered serializers/parsers generated at compile time
- AOT-friendly by design, no IL warnings
- Minimum C# 9.0 requirement for broader compatibility (including Unity)
- No third-party dependencies
- protobuf-net–style Serializer API and familiar attributes
- Performance is about 20% to 50% better than protobuf-net; see [benchmarks](#performance--benchmarks-) below
- Target frameworks: netstandard2.0, net8.0, net9.0, net10.0
- Serialize to Stream or IBufferWriter<byte>, or use ToByteArray
- ReadOnlySpan<byte>/ReadOnlySequence<byte>/Stream deserialization
- Dynamic and non-generic serialization/deserialization APIs
- RuntimeTypeModel-like APIs for dynamic message types
- Surrogates supported

## Rich built-in type support 🧰

- .NET primitives (`byte`,`sbyte`, `int`,`uint`,`long`,`ulong`, `bool`, `char`, `double`, etc.)
- `string`, `decimal`, `Half`, `Int128`, `UInt128`, `Guid`, `Rune`, `BigInteger`
- `TimeSpan`, `DateTime`, `DateTimeOffset`, `TimeOnly`, `DateOnly`, `TimeZoneInfo`
- `Complex`, `Plane`, `Quaternion`, `Matrix3x2`, `Matrix4x4`, `Vector2`, `Vector3`, `Vector4`
- `Uri`, `Version`, `StringBuilder`, `BitArray`, `CultureInfo`
- `Nullable<>`, `Lazy<>`
- `T[]`, `List<>`, `LinkedList<>`, `Queue<>`, `Stack<>`, `HashSet<>`, `SortedSet<>`
- `Dictionary<,>`, `SortedList<,>`, `SortedDictionary<,>`, `ReadOnlyDictionary<,>`
- `Collection<>`, `ReadOnlyCollection<>`, `ObservableCollection<>`, `ReadOnlyObservableCollection<>`
- `IEnumerable<>`, `ICollection<>`, `IList<>`, `IReadOnlyCollection<>`, `IReadOnlyList<>`, `ISet<>`
- `IDictionary<,>`, `IReadOnlyDictionary<,>`
- `ConcurrentBag<>`, `ConcurrentQueue<>`, `ConcurrentStack<>`, `ConcurrentDictionary<,>`, `BlockingCollection<>`
- `ImmutableList<>`, `ImmutableArray<>`, `ImmutableHashSet<>`, `ImmutableDictionary<,>`

## Quick Start ⚡

Install from NuGet:

```bash
dotnet add package LightProto
```

Define your contracts (partial classes) using LightProto attributes:

```csharp
using LightProto;

[ProtoContract]
public partial class Person
{
    [ProtoMember(1)]
    public string Name { get; set; } = string.Empty;

    [ProtoMember(2)]
    public int Age { get; set; }
}

var person = new Person { Name = "Alice", Age = 30 };

// Serialize to a byte[]
byte[] bytes = person.ToByteArray();
// person.ToByteArray(Person.ProtoWriter); // use this overload when targeting .netstandard2.0

// Or serialize to a Stream
using var stream = new MemoryStream();
Serializer.Serialize(stream, person);
// Serializer.Serialize(stream, person, Person.ProtoWriter); // use this overload when targeting .netstandard2.0

byte[] data = stream.ToArray();

// Deserialize from byte[] (ReadOnlySpan<byte> overload will be used)
Person fromBytes = Serializer.Deserialize<Person>(bytes);
// Person fromBytes = Serializer.Deserialize<Person>(bytes, Person.ProtoReader); // use this overload when targeting .netstandard2.0

// Or deserialize from Stream
using var input = new MemoryStream(data);
Person fromStream = Serializer.Deserialize<Person>(input);
// Person fromStream = Serializer.Deserialize<Person>(input, Person.ProtoReader); // use this overload when targeting .netstandard2.0
```

## Migration from protobuf-net 🔁

Most code migrates by swapping the namespace and marking your types partial.

1. Replace ProtoBuf with LightProto.
2. Mark serializable types as partial.

Example:

```diff
- using ProtoBuf;
+ using LightProto;

[ProtoContract]
- public class Person
+ public partial class Person
{
    [ProtoMember(1)]
    public string Name { get; set; } = string.Empty;

    [ProtoMember(2)]
    public int Age { get; set; }
}

var myObject = new Person { Name = "Alice", Age = 30 };

// Serialization
var stream = new MemoryStream();
Serializer.Serialize(stream, myObject);
byte[] data = stream.ToArray();

// Deserialization
var obj = Serializer.Deserialize<Person>(new ReadOnlySpan<byte>(data));
```

## Serialization APIs 🧩

### Generic-constrained APIs 🔒

`Serializer.Serialize<T>(...)` and `Serializer.Deserialize<T>(...)` require that `T` implements `IProtoParser<T>` (i.e., a generated message type).

**Note:** These APIs are not supported in .netstandard2.0 due to lack of static virtual members in interfaces. Use IProtoParser-specified APIs instead.

### IProtoParser-specified APIs 🧭

`Serializer.Serialize<T>(..., IProtoWriter<T>)` and `Serializer.Deserialize<T>(..., IProtoReader<T>)` where `T` is a `[ProtoContract]` marked type with generated `IProtoParser<T>` implementation.

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.Serialize<Person>(bufferWriter, person, Person.ProtoWriter); // must pass writer
var bytes = person.ToByteArray(Person.ProtoWriter); // extension method
Person result = LightProto.Serializer.Deserialize<Person>(bytes, Person.ProtoReader); // must pass reader
```

### Dynamic APIs 🌀

`Serializer.SerializeDynamically<T>(...)` and `Serializer.DeserializeDynamically<T>(...)` resolve `IProtoReader/Writer` at runtime via `T.ProtoReader/Writer` or `Serializer.RegisterParser` or reflection.

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.SerializeDynamically<Person>(bufferWriter, person); // dynamic API
Person result = LightProto.Serializer.DeserializeDynamically<Person>(bufferWriter.WrittenSpan); // dynamic API
```

ProtoWriter/Reader resolution order:

1. If a parser is registered via `Serializer.RegisterParser<T>(reader, writer)`, use the registered parser.
2. If `T` is a primitive/built-in type, use built-in parser from `LightProto.Parser` namespace which is registered internally.
3. If `T` implements `IProtoParser<T>` (usually marked as `[ProtoContract]`), use `T.ProtoWriter/Reader` by reflection. This is fine with AOT, as the generic argument T is marked as `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]`.
4. If `T` is a generic container shape (e.g., `List<>`, `Dictionary<,>`, `Nullable<>`, etc.), try to resolve element type parser recursively. This may fail at runtime on AOT due to missing type metadata.

### Non-generic APIs 🧱

`Serializer.SerializeNonGeneric(..., object instance)` and `Serializer.DeserializeNonGeneric(Type type, ...)` are similar to Dynamic APIs, but the type is specified at runtime.

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.SerializeNonGeneric(bufferWriter, person); // non-generic API
Person result = (Person)LightProto.Serializer.DeserializeNonGeneric(typeof(Person), bufferWriter.WrittenSpan); // non-generic API
```

The ProtoWriter/Reader resolution order is the same as [Dynamic APIs](#dynamic-apis-).

## .NET Standard 📦

In .NET Standard target frameworks, we can't use [static virtual members in interface](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members) to find `T.ProtoReader/Writer`.

So LightProto requires you to specify a `ProtoWriter` when serializing and a `ProtoReader` when deserializing.

For `[ProtoContract]`-marked message types, `ProtoReader/Writer` is generated by LightProto, so use `MessageType.ProtoReader/Writer`.

For primitive types, LightProto provides predefined parsers in the `LightProto.Parser` namespace, such as `LightProto.Parser.DateTimeParser`.

If you don't need AOT support, you can use the dynamic APIs `Serializer.SerializeDynamically<T>` and `Serializer.DeserializeDynamically<T>` without passing ProtoReader/Writer.

### Unity Support 🎮

LightProto's generated code supports C# 9, making it compatible with Unity projects targeting .NET Standard 2.0. You can use LightProto in Unity by following the same installation and usage instructions as for other .NET projects.
IL2CPP builds are supported because LightProto is AOT-friendly.

## Surrogates 🧬

protobuf-net can register surrogates via RuntimeTypeModel at runtime.

LightProto lets you specify a custom ProtoParserType for MessageType.
For example, if MessageType is `Person` and the custom ProtoParserType is `PersonProtoParser`, you can use the following attributes in precedence order:

1. member level: `[ProtoMember(1,ParserType=typeof(PersonProtoParser))]`
2. class level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
3. module/assembly level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]` (messageType and parserType should not be in the same assembly; if they are, use the type-level attribute.)
4. type level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
5. default: `global::LightProto.Parser.PersonProtoParser`

The ProtoParserType must implement `IProtoParser<MessageType>`. The easiest way is to define a SurrogateType with `[ProtoContract]` and mark it with `[ProtoSurrogateFor<MessageType>]`.
Example for Person (can be any type):

```csharp
[ProtoParserType(typeof(PersonProtoParser))] // type level ProtoParser
public class Person
{
 public string Name {get; set;}
 Person(){}
 public static Person FromName(string name) => new Person() { Name = name };
}

[ProtoContract]
[ProtoSurrogateFor<Person>] // mark this to tell source generator generate IProtoParser<Person> instead of `IProtoParser<PersonProtoParser>`
public partial struct PersonProtoParser
{
  [ProtoMember(1)]
  internal string Name { get; set; }    
  public static implicit operator Person(PersonProtoParser parser) // must define implicit conversions for surrogate type
  {
      return Person.FromName(parser.Name);
  }    
  public static implicit operator PersonProtoParser(Person value) // must define implicit conversions for surrogate type
  {
      return new PersonProtoParser() { Name = value.Name };
  }
}
[assembly: ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))] // assembly level ProtoParser

[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))] // class level ProtoParser
public class MessageContract
{
  [ProtoMember(1,ParserType=typeof(PersonProtoParser))] //member level ProtoParser
  public Person Person {get; set;}      
}
```

You can also read/write raw binary data, but only WireType.LengthDelimited is supported for now because LightProtoGenerator needs to compute tags at compile time and any unknown type will be treated as LengthDelimited.

## StringIntern 🧵

`[StringIntern]` attribute can be applied to individual string members, classes, modules, or assemblies.

## RuntimeTypeModel 🧠

LightProto provides a set of APIs in `RuntimeProtoWriter`, `RuntimeProtoReader`, and `RuntimeProtoParser`.

`RuntimeProtoParser<T>` can be used to get `IProtoReader<T>` and `IProtoWriter<T>` at runtime.

You can use them to serialize/deserialize, or use Serializer.RegisterParser<T>(reader, writer) to register globally, then use Serializer.SerializeDynamically/DeserializeDynamically or Serializer.SerializeNonGeneric APIs.

```csharp
public class TestMessage
{
    public int Value { get; set; }
    public string StringValue { get; set; } = string.Empty;
    public int[] IntArray { get; set; } = [];
}
var runtimeParser = new RuntimeProtoParser<TestMessage>(() => new());
runtimeParser.AddMember(1, message => message.Value, (message, value) => message.Value = value);
runtimeParser.AddMember(typeof(string), 2, message => message.StringValue, (message, value) => message.StringValue = (string)value);
runtimeParser.AddMember<int[]>(
    3,
    message => message.IntArray,
    (message, value) => message.IntArray = value,
    // specify array reader/writer for aot support
    Int32ProtoParser.ProtoReader.GetArrayReader(),
    Int32ProtoParser.ProtoWriter.GetCollectionWriter()
);

// Use the runtime parser to serialize/deserialize.
var writer = runtimeParser.ProtoWriter;
var reader = runtimeParser.ProtoReader;
```

If you do not need both Serialize and Deserialize, you can use `RuntimeProtoWriter<T>` or `RuntimeProtoReader<T>` to create only writer or reader.

```csharp
public class TestMessage
{
    public int Value { get; set; }
    public string StringValue { get; set; } = string.Empty;
    public int[] IntArray { get; set; } = [];
}

var protoReader = new RuntimeProtoReader<TestMessage>(() => new());
protoReader.AddMember<int>(1, (message, value) => message.Value = value);
protoReader.AddMember(typeof(string), 2, (message, value) => message.StringValue = (string)value);
protoReader.AddMember<int[]>(3, (message, value) => message.IntArray = value, Int32ProtoParser.ProtoReader.GetArrayReader());

var protoWriter = new RuntimeProtoWriter<TestMessage>();
protoWriter.AddMember<int>(1, message => message.Value);
protoWriter.AddMember(typeof(string), 2, message => message.StringValue);
protoWriter.AddMember<int[]>(3, message => message.IntArray, Int32ProtoParser.ProtoWriter.GetCollectionWriter());
```

## IExtensible 🧷

`IExtensible` is defined for compatibility only and has no effect.

## Performance & Benchmarks 📊

The following benchmarks compare serialization performance between LightProto, protobuf-net, and Google.Protobuf.

You can reproduce these by cloning the repo and running tests/Benchmark.

```md
BenchmarkDotNet v0.15.3, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100-rc.1.25451.107
[Host]    : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3
.NET 10.0 : .NET 10.0.0 (10.0.0-rc.1.25451.107, 10.0.25.45207), X64 RyuJIT x86-64-v3
.NET 8.0  : .NET 8.0.20 (8.0.20, 8.0.2025.41914), X64 RyuJIT x86-64-v3
.NET 9.0  : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
```
| Method                   | Job       | Runtime   | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |---------- |---------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| Serialize_ProtoBuf_net   | .NET 10.0 | .NET 10.0 | 645.6 μs | 12.70 μs | 11.88 μs |  1.39 |    0.03 | 526.41 KB |        1.03 |
| Serialize_GoogleProtoBuf | .NET 10.0 | .NET 10.0 | 539.9 μs | 10.71 μs | 12.75 μs |  1.16 |    0.03 | 512.95 KB |        1.00 |
| Serialize_LightProto     | .NET 10.0 | .NET 10.0 | 465.1 μs |  7.88 μs |  6.99 μs |  1.00 |    0.02 | 512.95 KB |        1.00 |
|                          |           |           |          |          |          |       |         |           |             |
| Serialize_ProtoBuf_net   | .NET 8.0  | .NET 8.0  | 757.0 μs | 12.80 μs | 11.98 μs |  1.42 |    0.04 | 526.41 KB |        1.03 |
| Serialize_GoogleProtoBuf | .NET 8.0  | .NET 8.0  | 553.9 μs | 10.97 μs |  9.72 μs |  1.04 |    0.03 | 512.95 KB |        1.00 |
| Serialize_LightProto     | .NET 8.0  | .NET 8.0  | 531.9 μs | 10.52 μs | 14.04 μs |  1.00 |    0.04 | 512.95 KB |        1.00 |
|                          |           |           |          |          |          |       |         |           |             |
| Serialize_ProtoBuf_net   | .NET 9.0  | .NET 9.0  | 712.6 μs | 13.61 μs | 12.73 μs |  1.39 |    0.04 | 526.41 KB |        1.03 |
| Serialize_GoogleProtoBuf | .NET 9.0  | .NET 9.0  | 546.7 μs | 10.70 μs | 16.33 μs |  1.07 |    0.04 | 512.95 KB |        1.00 |
| Serialize_LightProto     | .NET 9.0  | .NET 9.0  | 513.6 μs | 10.15 μs | 13.89 μs |  1.00 |    0.04 | 512.95 KB |        1.00 |


| Method                     | Job       | Runtime   | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------------- |---------- |---------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| Deserialize_ProtoBuf_net   | .NET 10.0 | .NET 10.0 | 569.2 μs | 10.88 μs | 12.53 μs |  1.38 |    0.04 |    562 KB |        0.84 |
| Deserialize_GoogleProtoBuf | .NET 10.0 | .NET 10.0 | 441.4 μs |  8.67 μs | 10.64 μs |  1.07 |    0.04 |  648.7 KB |        0.97 |
| Deserialize_LightProto     | .NET 10.0 | .NET 10.0 | 411.5 μs |  8.08 μs |  9.92 μs |  1.00 |    0.03 | 665.95 KB |        1.00 |
|                            |           |           |          |          |          |       |         |           |             |
| Deserialize_ProtoBuf_net   | .NET 8.0  | .NET 8.0  | 688.0 μs | 13.51 μs | 15.56 μs |  1.55 |    0.05 |    562 KB |        0.84 |
| Deserialize_GoogleProtoBuf | .NET 8.0  | .NET 8.0  | 595.5 μs | 11.51 μs | 16.14 μs |  1.34 |    0.04 |  648.7 KB |        0.97 |
| Deserialize_LightProto     | .NET 8.0  | .NET 8.0  | 444.8 μs |  8.88 μs |  9.12 μs |  1.00 |    0.03 | 665.95 KB |        1.00 |
|                            |           |           |          |          |          |       |         |           |             |
| Deserialize_ProtoBuf_net   | .NET 9.0  | .NET 9.0  | 662.3 μs | 12.60 μs | 11.17 μs |  1.53 |    0.04 |    562 KB |        0.84 |
| Deserialize_GoogleProtoBuf | .NET 9.0  | .NET 9.0  | 491.7 μs |  9.64 μs | 13.52 μs |  1.14 |    0.04 |  648.7 KB |        0.97 |
| Deserialize_LightProto     | .NET 9.0  | .NET 9.0  | 431.9 μs |  8.33 μs |  9.25 μs |  1.00 |    0.03 | 665.95 KB |        1.00 |

Note: Results vary by hardware, runtime, and data model. Please run the benchmarks on your environment for the most relevant numbers.

## Working with .proto files 📄

LightProto doesn't ship a .proto → C# generator yet. You can generate C# using protobuf-net (or other tools), then adapt the output to LightProto (typically replacing the ProtoBuf namespace with LightProto and marking types partial). If something doesn't work, please file an issue.

If you need a dedicated .proto → C# generator, please vote on this [issue](https://github.com/dameng324/LightProto/issues/85).

## Contributing 🤝

Contributions are welcome! Please see [CONTRIBUTING](CONTRIBUTING.md) for detailed contribution guidelines.

[ARCHITECTURE.md](ARCHITECTURE.md) describes the internal design and structure of LightProto, which may be helpful for contributors.

## License 📄

MIT License — see LICENSE for details.
