# LightProto Architecture

## 1. Solution & Project Layout

Top-level solution:

- `LightProto.sln` – root solution
- `global.json` – pins .NET SDK
- `src/` – production code
  - `src/LightProto/` – **runtime library / public API** (NuGet package `LightProto`)
  - `src/LightProto.Generator/` – **Roslyn source generator** for `[ProtoContract]` types
- `tests/` – test & perf projects
  - `tests/LightProto.Tests/` – main unit tests (TUnit), plus generated .proto-based models
  - `tests/LightProto.AssemblyLevelTests/` – tests for assembly/module level configuration
  - `tests/TestAot/` – AOT sample/verification app using Google.Protobuf
  - `tests/Benchmark/` – BenchmarkDotNet benchmarks vs protobuf-net & Google.Protobuf

### 1.1 `src/LightProto/LightProto.csproj`

- Targets: `netstandard2.0; net8.0; net9.0; net10.0`
- Marked `<IsAotCompatible>true</IsAotCompatible>`; allows Native AOT.
- References `LightProto.Generator` as an **Analyzer** and packs its DLL under `analyzers/dotnet/cs` to enable source generation when consumed as a NuGet package.
- For `netstandard2.0`, references `System.Runtime.CompilerServices.Unsafe` and `System.Memory` to polyfill newer APIs.

### 1.2 `src/LightProto.Generator/LightProto.Generator.csproj`

- Target: `netstandard2.0` (Roslyn analyzer compatible).
- Depends on `Microsoft.CodeAnalysis.CSharp` and `Microsoft.CodeAnalysis.Analyzers`.
- Reuses some enums from the runtime via
  - `Compile Include="..\LightProto\DataFormat.cs"`
  - `Compile Include="..\LightProto\CompatibilityLevel.cs"`
  - `Compile Include="..\LightProto\ImplicitFields.cs"`

## 2. Core Concepts

### 2.1 Message Contract & Parser Interfaces

Runtime lives in the `LightProto` namespace.

Key abstractions:

```csharp
public interface IProtoParser<T>
{
#if NET7_0_OR_GREATER
    public static abstract IProtoReader<T> ProtoReader { get; }
    public static abstract IProtoWriter<T> ProtoWriter { get; }
#endif
}

public interface IProtoReader<out T>
{
    WireFormat.WireType WireType { get; }
    bool IsMessage { get; }
    T ParseFrom(ref ReaderContext input);
}

public interface IProtoWriter<in T>
{
    WireFormat.WireType WireType { get; }
    bool IsMessage { get; }
    int CalculateSize(T value);
    void WriteTo(ref WriterContext output, T value);
}
```

- **Message types**: user-defined types marked with `[ProtoContract]` and `[ProtoMember]`.
- **Parser types**: implement `IProtoParser<T>` and expose static `ProtoReader` / `ProtoWriter` implementing `IProtoReader<T>` / `IProtoWriter<T>`.
- On .NET 7+ the generator uses static virtual interface members; on `netstandard2.0` you must pass explicit `ProtoReader` / `ProtoWriter` parameters.

### 2.2 Attributes & Metadata

`src/LightProto/Attributes.cs` defines a protobuf-net-like attribute model, used purely at compile time by the generator:

- `[ProtoContract]` – marks classes/structs/interfaces/records for generation
  - `ImplicitFields` + `ImplicitFirstTag` support implicit tagging of members
  - `SkipConstructor` allows bypassing the ctor in deserialization (with restrictions)
- `[ProtoMember(tag)]` – annotates individual fields/properties
  - `DataFormat`, `IsRequired`, `IsPacked`, `Name`, `ParserType`
- `[ProtoMap]` – specifies key/value `DataFormat` for dictionary-like members
- `[ProtoIgnore]` – opt-out per member
- `[ProtoSurrogateFor<T>]` – marks a parser/surrogate type for a non-contract message type
- `[ProtoInclude(tag, knownType)]` – inheritance / polymorphism support
- `[CompatibilityLevel(level)]` – controls wire-compat mode (200/240/300)
- `[StringIntern]` – enable string interning (per member/type/module/assembly)
- `[ProtoParserType(parserType)]` – ties a **specific message type** to a custom parser type
- `[ProtoParserTypeMap(messageType, parserType)]` – assembly/module/host-type level mapping from message type to parser type

These attributes drive the source generator and never use reflection at runtime.

### 2.3 Wire-Level Primitives

LightProto reuses Google.Protobuf-style low-level infrastructure (adapted to `LightProto`):

- `WireFormat` – wire types and tag helpers
  - `WireFormat.WireType` enum (Varint, Fixed32, Fixed64, LengthDelimited, StartGroup, EndGroup)
  - `MakeTag(fieldNumber, wireType)`, `GetTagFieldNumber`, `GetTagWireType`
- `CodedInputStream` – streaming reader over `Stream` + internal buffer (`ParserInternalState`)
- `CodedOutputStream` – buffered writer over `Stream` or `byte[]` (`WriterInternalState`)
- `ReaderContext` / `WriterContext` – thin contexts wrapping these primitives with extra state (recursion depth, limits, etc.).

`ParsingPrimitives`, `SegmentedBufferHelper`, `WriteBufferHelper` encapsulate varint, length-prefix, and segmented-buffer handling.

### 2.4 Built-In Parsers (`LightProto.Parser` namespace)

`src/LightProto/Parser/*.cs` contains many specialized parser implementations:

- Primitive/value types: `Int32`, `UInt32`, `Int64`, `UInt64`, `Bool`, `Single`, `Double`, `Decimal`, `Guid`, `DateTime`, `DateOnly`, `TimeSpan`, etc.
- Collections: `Array`, `List`, `HashSet`, `Stack`, `Queue`, `SortedList`, `SortedDictionary`, concurrent variants, `ByteArray`, `ByteList`.
- Higher-level helpers:
  - `IEnumerableProtoReader/Writer` – generic readers/writers for `IEnumerable<T>` based collections
  - `DictionaryProtoReader/Writer` and `IEnumerableKeyValuePairProtoReader/Writer` for maps
  - `MessageWrapper<T>` – wraps non-message primitives/collections as message-like values when a `message` context is required.

These parsers are constructed and composed by the source generator based on member metadata.

## 3. Source Generator Pipeline (`LightProto.Generator`)

`LightProtoGenerator` is an `IIncrementalGenerator` that turns attribute-marked types into `IProtoParser<T>` implementations.

### 3.1 Discovery Phase

- `Initialize` registers a syntax provider which selects all `class/struct/interface/record` declarations.
- For each candidate symbol, it filters to those with `[LightProto.ProtoContractAttribute]`.
- For each `[ProtoContract]` type, `GetProtoContract` builds a `ProtoContract` model:
  - `Type`, `TypeDeclaration`, `Members`, `ImplicitFields`, `ImplicitFirstTag`
  - `SkipConstructor`, `ProxyFor` (from `[ProtoSurrogateFor<T>]`)
  - Derived types from `[ProtoInclude]` on the contract (with validation)

### 3.2 Member Modeling (`ProtoMember`)

For each instance member of a contract type, `GetProtoMembers` produces a `ProtoMember` record with:

- `Name` / `Type` (`ITypeSymbol`)
- `FieldNumber` (from `[ProtoMember(tag)]` or implicit numbering)
- `DataFormat`, `MapFormat` (from `[ProtoMap]`)
- `IsRequired` (C# `required`), `IsProtoMemberRequired` (via `[ProtoMember(IsRequired=true)]`)
- `IsReadOnly`, `IsInitOnly`
- `Initializer` code (explicit initializer or synthesized default, or `Empty` static when present)
- `CompatibilityLevel` (resolved from member/type/module/assembly attributes)
- `StringIntern` flag, `ProxyType` (from `ProtoProxy`/`ProtoSurrogateFor`-like attributes)
- Computed wire metadata: `WireType`, `RawTag`, `RawTagSize`, `IsPacked`.

`ProtoMember.GetPbWireType` and helper predicates determine whether a member is encoded as varint, fixed32, fixed64, or length-delimited.

### 3.3 Parser Resolution

`GetProtoParser` and `GetProtoParserMember` choose the appropriate parser shape and generate backing fields:

- Prefer:
  1. Member-level `ParserType` on `[ProtoMember]`
  2. Type/Module/Assembly-level `[ProtoParserTypeMap(messageType, parserType)]`
  3. Type-level `[ProtoParserType(parserType)]`
  4. Self-implemented parser types implementing `IProtoParser<T>`
- When no custom parser exists, map known CLR types to built-in parsers in `LightProto.Parser.*` (with adjustments for `DataFormat`, `CompatibilityLevel` and `StringIntern`).
- For collections:
  - Arrays/collections become `ArrayProtoReader/Writer<T>`, `ListProtoReader/Writer<T>`, `HashSetProtoReader/Writer<T>`, etc., with element parsers recursively resolved.
  - Dictionaries become `DictionaryProtoReader/Writer<TKey, TValue>` or `IEnumerableKeyValuePairProtoReader/Writer` depending on concrete/interface types.

### 3.4 Generated Code Shape

Generated code is emitted per contract type as a partial nested/peer type in the same namespace, with two main shapes:

1. **Simple (no inheritance / no `ProtoInclude`)**
   - For non-interface types without derived `ProtoInclude` entries.
   - Generated body (simplified):

     ```csharp
     [DebuggerDisplay("{ToString(),nq}")]
     partial class MyMessage : IProtoParser<MyMessage>
     {
         public static IProtoReader<MyMessage> ProtoReader { get; } = new LightProtoReader();
         public static IProtoWriter<MyMessage> ProtoWriter { get; } = new LightProtoWriter();

         public sealed class LightProtoWriter : IProtoWriter<MyMessage> { ... }
         public sealed class LightProtoReader : IProtoReader<MyMessage> { ... }
     }
     ```

   - `LightProtoWriter` and `LightProtoReader` handle: tags, packed/unpacked fields, required-field checking, and `SkipConstructor` handling.

2. **Hierarchy/Interface-aware (with `ProtoInclude` or interface-based polymorphism)**
   - For base types with derived `[ProtoInclude]` or contract interfaces.
   - The generator emits:
     - A `MemberStruct` DTO capturing all fields and derived-type member-structs.
     - `MemberStructLightProtoWriter` / `MemberStructLightProtoReader` to work on `MemberStruct`.
     - `LightProtoReader` / `LightProtoWriter` bridging between `MemberStruct` and concrete messages, handling:
       - Base/derived message reconstruction
       - Root-base-type `MemberStruct` decomposition
       - Read-only collection copy semantics.

For nested types, `GenerateNestedClassStructure` wraps generated members inside the chain of containing types so that the source structure mirrors user code.

### 3.5 Diagnostics

`LightProtoGeneratorException` encapsulates structured diagnostics:

- Examples: `LIGHT_PROTO_001` (Init-only with `SkipConstructor`), `LIGHT_PROTO_006` (duplicate tag), `LIGHT_PROTO_007` (interface without `ProtoInclude`), `LIGHT_PROTO_014` (parser type missing `ProtoReader`/`ProtoWriter`), etc.
- These are reported via `spc.ReportDiagnostic` in the generator; they are visible in IDE build output.

## 4. Runtime Serialization Flow

### 4.1 High-Level `Serializer` API

`Serializer` is a static partial class split across files:

- `Serializer.Serialize.cs` – high-level APIs, size calculation, message wrapper handling.
- `Serializer.Deserialize.cs` – high-level deserialize APIs.
- `Serializer.Extensions.cs` – convenience overloads and deep-clone helpers.

Key patterns (on .NET 7+ with static interfaces):

```csharp
[ProtoContract]
public partial class Person
{
    [ProtoMember(1)] public string Name { get; set; } = string.Empty;
    [ProtoMember(2)] public int Age { get; set; }
}

var person = new Person { Name = "Alice", Age = 30 };

// Serialize to byte[] (generated ProtoWriter used implicitly)
byte[] bytes = person.ToByteArray();

// Deserialize from span / sequence / stream
Person p1 = Serializer.Deserialize<Person>(bytes);
Person p2 = Serializer.Deserialize<Person>(new ReadOnlySequence<byte>(bytes));
using var ms = new MemoryStream(bytes);
Person p3 = Serializer.Deserialize<Person>(ms);
```

On `netstandard2.0` or for primitives/collections, callers must pass explicit parser objects:

```csharp
int a = 10;
var bytes = a.ToByteArray(Int32ProtoParser.Writer);
int result = Serializer.Deserialize(bytes, Int32ProtoParser.Reader);

List<int> list = [1, 2, 3];
var listBytes = list.ToByteArray(Int32ProtoParser.Writer);
List<int> clone = Serializer.Deserialize<List<int>, int>(listBytes, Int32ProtoParser.Reader);
```

### 4.2 Message vs Non-message Handling

- `IProtoWriter<T>.IsMessage` / `IProtoReader<T>.IsMessage` indicate whether the value should be framed as a **length-delimited message**.
- Helper extensions:
  - `CalculateMessageSize` – wraps `CalculateSize` with a `ComputeLengthSize` when `IsMessage == true`.
  - `WriteMessageTo` – writes a length prefix then the value.
  - `ParseMessageFrom` – reads a length, pushes limits/recursion depth, delegates to `ParseFrom`, then pops/validates.
- For non-message values used in message contexts, `MessageWrapper<T>` is used to adapt underlying primitive/collection parsers.

### 4.3 Streaming & Buffers

- `Serialize(Stream destination, T instance, IProtoWriter<T> writer)`
  - Wraps `destination` in `CodedOutputStream`, initializes `WriterContext`, writes and flushes.
- `Serialize(IBufferWriter<byte> destination, T instance, IProtoWriter<T> writer)`
  - Writes directly into `IBufferWriter<byte>` via `WriterContext`.
- `Deserialize(Stream source, IProtoReader<T> reader)`
  - Wraps `source` in `CodedInputStream`, initializes `ReaderContext`, and calls `ParseFrom`.
- `Deserialize(ReadOnlySpan<byte>/ReadOnlySequence<byte>, IProtoReader<T>)`
  - Initializes `ReaderContext` over in-memory buffers without `Stream`.

### 4.4 Deep Clone Path

`Serializer.DeepClone<T>`:

- Rents a buffer from `ArrayPool<byte>` sized to `writer.CalculateSize(message)`.
- Serializes into the span via `WriterContext`, then immediately deserializes via `Deserialize(span, reader)`.
- This is used both in tests and in user code, and it works for messages and non-messages as long as appropriate reader/writer are provided (or available via `IProtoParser<T>` on .NET 7+).

## 5. Tests & Benchmarks

### 5.1 `tests/LightProto.Tests`

`LightProto.Tests.csproj`:

- Targets `net8.0; net9.0; net10.0;` plus `net48` on Windows.
- Uses `TUnit` and `Microsoft.Testing.*` packages for test host and code coverage.
- References:
  - `src/LightProto/LightProto.csproj`
  - `src/LightProto.Generator/` as analyzer
  - `Google.Protobuf` and `protobuf-net` for interoperability tests
  - `Grpc.Tools` and `.proto` files (`package.proto`, `Parsers/parser.proto`, `protobuf-net/bcl.proto`) to generate comparison models.

Key areas covered:

- **SerializerTests** – end-to-end tests for:
  - Stream, `IBufferWriter<byte>`, `ReadOnlySpan<byte>`, `ReadOnlySequence<byte>` serialization
  - Collections (arrays, lists, empty/null collections) and dictionaries
  - Large-object path (reading `test.bin`) and large-length slow paths
  - Length-prefix styles (`PrefixStyle.Base128`, `Fixed32`, etc.)
  - Deep clone behavior, non-message primitives, `IExtensible` compatibility shim
- **CollectionTest/** – more fine-grained semantics around collections and fixed-size packed fields.
- **Parsers/** – unit tests for individual built-in parser types (DateTime, Decimal, Guids, concurrent collections, nullable types, implicit fields, inheritance, etc.).
- **CustomParser/** – tests around `ProtoParserTypeMap`, `ProtoSurrogateFor`, and custom parser priority.

### 5.2 `tests/LightProto.AssemblyLevelTests`

- Validates **assembly-level** `[ProtoParserTypeMap]` behavior and priority.
- Example (`AssemblyPriorityTests`):

  ```csharp
  [assembly: ProtoParserTypeMap(
      typeof(CustomPriorityTests.Person),
      typeof(AssemblyPriorityTests.AssemblyLevelPersonProtoParser)
  )]

  public class AssemblyLevelPersonProtoParser : IProtoParser<Person>
  {
      public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();
      public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

      public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;
      public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
  }
  ```

- Asserts that the assembly-level mapping is picked over other mapping levels and that the chained reader/writer are actually used.

### 5.3 `tests/TestAot`

- A small `net9.0` AOT-published console app referencing `Google.Protobuf`.
- Uses `package.proto` and `Grpc.Tools` to validate that the generated Google.Protobuf models behave as expected under AOT.
- Demonstrates interoperability and acts as a smoke test for Google.Protobuf dependencies.

### 5.4 `tests/Benchmark`

- BenchmarkDotNet project comparing LightProto, protobuf-net, and Google.Protobuf.
- Uses shared contracts under `tests/Benchmark/Contracts/` and a `test.bin` payload.
- `Program.cs`:
  - Delegates to `BenchmarkSwitcher.FromAssembly(...).Run(args)`; alternative commented-out manual benchmark runs are provided.

## 6. Extensibility Guidelines

This section is intended as instructions for future Copilot suggestions.

### 6.1 Adding a New Message Type

1. Define the type in your desired namespace as **partial** and mark with `[ProtoContract]`.
2. Add `[ProtoMember(tag)]` for each serializable field/property; ensure unique positive tags.
3. If you want implicit tagging:
   - Set `ImplicitFields = ImplicitFields.AllPublic` or `.FirstTag`, and optionally `ImplicitFirstTag`.
4. For inheritance:
   - Mark base with `[ProtoInclude(tag, typeof(DerivedType))]` for each derived type.
5. For .NET 7+ consumers:
   - The generator will emit `IProtoParser<T>.ProtoReader` / `ProtoWriter` automatically.

Example:

```csharp
[ProtoContract]
public partial class Order
{
    [ProtoMember(1)] public int Id { get; set; }
    [ProtoMember(2)] public string Customer { get; set; } = string.Empty;
    [ProtoMember(3)] public decimal Total { get; set; }
}
```

### 6.2 Custom Parser Types & Surrogates

When you need a custom wire representation or mapping:

- **Type-level parser**:

  ```csharp
  [ProtoParserType(typeof(PersonProtoParser))]
  public class Person
  {
      public string Name { get; set; } = string.Empty;
      private Person() {}
      public static Person FromName(string name) => new() { Name = name };
  }

  [ProtoContract]
  [ProtoSurrogateFor<Person>]
  public partial struct PersonProtoParser
  {
      [ProtoMember(1)] internal string Name { get; set; }

      public static implicit operator Person(PersonProtoParser parser)
          => Person.FromName(parser.Name);

      public static implicit operator PersonProtoParser(Person value)
          => new() { Name = value.Name };
  }
  ```

- **Assembly-level mapping** (when message and parser live in different assemblies):

  ```csharp
  [assembly: ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]
  ```

- Ensure that the parser type either:
  - Is itself a `[ProtoContract]` (so LightProto generates its own parser), or
  - Exposes static `ProtoReader`/`ProtoWriter` properties of the appropriate `IProtoReader<T>` / `IProtoWriter<T>` type.

### 6.3 Collections & Dictionaries

- To serialize/deserialize collections for non-message element types, use the collection helpers off the element's parser:

  ```csharp
  // List<int>
  var writer = Int32ProtoParser.ProtoWriter.GetCollectionWriter();
  var reader = Int32ProtoParser.ProtoReader.GetCollectionReader<List<int>, int>();
  ```

- For dictionaries:

  ```csharp
  var dictWriter = Int32ProtoParser.ProtoWriter
      .GetDictionaryWriter(TestContract.ProtoWriter);

  var dictReader = Int32ProtoParser.ProtoReader
      .GetDictionaryReader<Dictionary<int, TestContract>, int, TestContract>(
          TestContract.ProtoReader
      );
  ```

- The generator automatically selects packed/unpacked encodings based on `IsPacked`, `DataFormat`, and `CompatibilityLevel`.

### 6.4 AOT Considerations

- Avoid runtime reflection-based configuration; all configuration must be static via attributes and generated code.
- On `netstandard2.0` / non-.NET 7+, you cannot rely on `IProtoParser<T>` static properties:
  - Always use overloads accepting explicit `IProtoReader<T>` / `IProtoWriter<T>`.
- Large-object and `GZipStream` scenarios are covered by tests (`LargeObjectTest`, `Test_TriggersLargeSizeSlowPath`).

## 7. How To Work in This Repo (for Copilot)

When generating or modifying code in this repository, prefer the following:

1. **Contracts**
   - Always use `[ProtoContract]` + `[ProtoMember]` for new message types.
   - Make contracts `partial` to allow the generator to emit code into partial types.
   - Keep tags stable to preserve wire compatibility.

2. **Tests**
   - Place standard unit tests in `tests/LightProto.Tests/`.
   - For assembly-level parser mapping scenarios, use `tests/LightProto.AssemblyLevelTests/`.
   - For AOT repros, use or mirror patterns from `tests/TestAot`.

3. **Generator**
   - Favor extending generator logic (`LightProtoGenerator`) rather than adding runtime reflection.
   - When adding new built-in parsers, place them under `src/LightProto/Parser/` and update `GetProtoParser` / compatibility rules where needed.

4. **Performance / Benchmarks**
   - To evaluate perf impact, mirror existing benchmark patterns in `tests/Benchmark`.
   - Keep large test data in files like `test.bin` and reuse them across tests/benchmarks.

This document is intentionally high-level and stable; for detailed runtime semantics, consult the XML comments and unit tests referenced above.