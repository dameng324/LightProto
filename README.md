# LightProto

[![NuGet](https://img.shields.io/nuget/v/LightProto.svg)](https://www.nuget.org/packages/LightProto/)
![Build](https://github.com/dameng324/LightProto/actions/workflows/ci.yml/badge.svg)
[![codecov](https://codecov.io/github/dameng324/lightproto/graph/badge.svg)](https://codecov.io/github/dameng324/lightproto)
[![License](https://img.shields.io/github/license/dameng324/LightProto.svg)](LICENSE)

A high‑performance, Native AOT–friendly Protocol Buffers implementation for C#/.NET. LightProto focuses on protobuf-net compatibility, zero runtime reflection, and ahead-of-time compilation.

- Native AOT compatible (no runtime codegen or reflection required)
- Comfortable migration path for protobuf-net users
- Competitive performance with low allocations


## Table of contents

- Why LightProto?
- Features
- Performance & Benchmarks
- Quick Start
- Migration from protobuf-net
- Known differences
- Working with .proto files
- API overview
- Target frameworks & AOT
- Development status
- Contributing
- License


## Why LightProto?

protobuf-net is a popular Protocol Buffers implementation in .NET, but some scenarios (especially Native AOT) can be challenging due to runtime reflection and dynamic generation. LightProto addresses this with compile-time code generation and a protobuf-net–style API.

Goals:

1. Native AOT compatibility: no runtime reflection or emit.
2. protobuf-net familiarity: support the majority of protobuf-net patterns for easy migration.
3. Performance: match or exceed protobuf-net where possible.


## Features

- Source generator–powered serializers/parsers at compile time
- No runtime reflection; AOT-friendly by design
- protobuf-net–style Serializer API and familiar attributes
- Stream and IBufferWriter<byte> serialization
- ReadOnlySpan<byte>/ReadOnlySequence<byte>/Stream deserialization
- Collections and dictionaries support
- Extension helpers: ToByteArray, CalculateSize, DeepClone


## Performance & Benchmarks

The following benchmarks compare serialization performance between LightProto, protobuf-net, and Google.Protobuf.

You can reproduce these by cloning the repo and running tests/Benchmark.

```md
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
AMD Ryzen 7 5800X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.305
[Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
```

| Method                   |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
|--------------------------|---------:|---------:|---------:|------:|--------:|----------:|------------:|
| Serialize_ProtoBuf_net   | 898.8 us | 18.29 us | 52.48 us |  1.61 |    0.11 | 526.41 KB |        1.03 |
| Serialize_GoogleProtoBuf | 651.7 us | 16.70 us | 48.70 us |  1.17 |    0.10 | 512.95 KB |        1.00 |
| Serialize_LightProto     | 559.3 us | 11.07 us | 21.34 us |  1.00 |    0.05 | 512.95 KB |        1.00 |

| Method                     |     Mean |    Error |   StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------------|---------:|---------:|---------:|------:|--------:|----------:|------------:|
| Deserialize_ProtoBuf_net   | 664.9 us | 13.28 us | 28.00 us |  1.53 |    0.08 |    562 KB |        0.88 |
| Deserialize_GoogleProtoBuf | 538.1 us | 10.73 us | 25.70 us |  1.24 |    0.07 |  648.7 KB |        1.02 |
| Deserialize_LightProto     | 436.0 us |  8.53 us | 14.71 us |  1.00 |    0.05 | 635.15 KB |        1.00 |

Note: Results vary by hardware, runtime, and data model. Please run the benchmarks on your environment for the most relevant numbers.


## Quick Start

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

// Or serialize to a Stream
using var stream = new MemoryStream();
Serializer.Serialize(stream, person);
byte[] data = stream.ToArray();

// Deserialize from byte[] (ReadOnlySpan<byte> overload will be used)
Person fromBytes = Serializer.Deserialize<Person>(bytes);

// Or deserialize from Stream
using var input = new MemoryStream(data);
Person fromStream = Serializer.Deserialize<Person>(input);
```


## Migration from protobuf-net

Most code migrates by swapping the namespace and marking your types partial.

1. Replace ProtoBuf with LightProto.
2. Mark serializable types as partial.
3. Remove runtime configuration (e.g., RuntimeTypeModel). LightProto generates code at compile time.

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

Common replacements:

- RuntimeTypeModel and runtime surrogates → use compile-time attributes (see Surrogates below).
- Non-partial types → mark as partial to enable generator output.


## Need to know

LightProto aims to minimize differences from protobuf-net; notable ones include:

- Partial classes required
  - protobuf-net: partial not required
  - LightProto: mark [ProtoContract] types as partial so the generator can emit code

- Generic Serialize/Deserialize type constraint
  - protobuf-net: Serializer.Serialize<int>(...) and Serializer.Deserialize<int>(...)
  - LightProto: T must implement IProtoParser<T> (i.e., a generated message type); primitives are not supported directly. Use another method which pass `IProtoReader/Writer` explicitly.
    ```csharp
    int a=10;
    ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
    LightProto.Serializer.Serialize<int>(writer, a,Int32ProtoParser.Writer); // must pass writer
    var bytes = a.ToByteArray(Int32ProtoParser.Writer); // extension method
    int result = LightProto.Serializer.Deserialize<int>(bytes,Int32ProtoParser.Reader); // must pass reader
    ```
    ```cs
    List<int> list=[1,2,3];
    var bytes = list.ToByteArray(Int32ProtoParser.Writer);// extension method
    ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
    LightProto.Serializer.Serialize(writer, list,Int32ProtoParser.Writer);// must pass element writer
    List<int> arr2=LightProto.Serializer.Deserialize<List<int>,int>(bytes,Int32ProtoParser.Reader); // must pass element reader
    ```

- IExtensible
  - protobuf-net: supports IExtensible for dynamic extensions
  - LightProto: IExtensible is defined for compatibility only and has no effect

- Surrogates
  - protobuf-net: can register surrogates via RuntimeTypeModel at runtime
  - LightProto: define at compile time via attributes
  
    Example for Guid:
      ```csharp
      namespace LightProto.Parser; //must defined in this namespace
      [ProtoContract]
      [ProtoSurrogateFor<Guid>]
      public partial struct GuidProtoParser // name must be <OriginalTypeName>ProtoParser
      {
          [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
          internal ulong Low { get; set; }
        
          [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
          internal ulong High { get; set; }
    
          public static implicit operator Guid(GuidProtoParser protoParser) //must define implicit conversions
          {
              Span<byte> bytes = stackalloc byte[16];
              BinaryPrimitives.WriteUInt64LittleEndian(bytes, protoParser.Low);
              BinaryPrimitives.WriteUInt64LittleEndian(bytes.Slice(8), protoParser.High);
              return new Guid(bytes);
          }
    
          public static implicit operator GuidProtoParser(Guid value) //must define implicit conversions
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
      ```

- StringIntern
    - protobuf-net: Use `RuntimeTypeModel.Default.StringInterning = true;` to enable string interning globally
    - LightProto: [StringIntern] attribute can apply to individual string members/class/module/assembly

- RuntimeTypeModel
  - Not supported; all configuration is static via attributes and generated code

If you encounter different behavior versus protobuf-net, please open an issue.


## Working with .proto files

LightProto doesn’t ship a .proto → C# generator. You can generate C# using protobuf-net (or other tools), then adapt the output to LightProto (typically replacing the ProtoBuf namespace with LightProto and marking types partial). If something doesn’t work, please file an issue.


## API overview

- Attributes
  - [ProtoContract], [ProtoMember], [ProtoMap], [ProtoInclude], [StringIntern], [CompatibilityLevel]
  - Surrogates: [ProtoSurrogateFor<TOriginal>]

- Core interfaces and types
  - IProtoParser<T>, IProtoReader<T>, IProtoWriter<T>
  - Serializer static API: Serialize(Stream/IBufferWriter), Deserialize(ReadOnlySpan/ReadOnlySequence/Stream), DeepClone, CalculateSize
  - Extensions: ToByteArray(), CalculateSize(this T), SerializeTo(this ICollection<T>, ...)


## Target frameworks & AOT

- Target frameworks: net8.0, net9.0, net10.0
- IsAotCompatible: true
- Designed to work in Native AOT scenarios (no runtime reflection/codegen)


## Development status

This project is under active development and may introduce breaking changes. Use in production at your own risk.


## Contributing

Contributions are welcome!

1. Check existing issues and discussions
2. Follow the project’s coding standards
3. Add tests for new functionality
4. Update documentation as needed

Development setup:

```bash
git clone https://github.com/dameng324/LightProto.git
cd LightProto
dotnet restore
dotnet build
dotnet test
```


## License

MIT License — see LICENSE for details.
