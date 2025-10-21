# LightProto

![.NET](https://img.shields.io/badge/netstandard2.0|net8.0|net9.0-5C2D91?logo=.NET&labelColor=gray)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/05f2a11f569d4f51bfd31d937b7fd319)](https://app.codacy.com/gh/dameng324/LightProto?utm_source=github.com&utm_medium=referral&utm_content=dameng324/LightProto&utm_campaign=Badge_Grade)
[![NuGet](https://img.shields.io/nuget/v/LightProto.svg)](https://www.nuget.org/packages/LightProto/)
[![downloads](https://img.shields.io/nuget/dt/LightProto)](https://www.nuget.org/packages/LightProto)
![Build](https://github.com/dameng324/LightProto/actions/workflows/ci.yml/badge.svg)
[![codecov](https://codecov.io/github/dameng324/lightproto/graph/badge.svg)](https://codecov.io/github/dameng324/lightproto)
[![CodeQL Advanced](https://github.com/dameng324/LightProto/actions/workflows/codeql.yml/badge.svg)](https://github.com/dameng324/LightProto/actions/workflows/codeql.yml)
![GitHub Repo stars](https://img.shields.io/github/stars/dameng324/LightProto.svg?style=flat)
![Size](https://img.shields.io/github/repo-size/dameng324/LightProto.svg)
[![License](https://img.shields.io/github/license/dameng324/LightProto.svg)](LICENSE)

A high‑performance, Native AOT–friendly Protocol Buffers implementation for C#/.NET. 

## Warnings

This project is under active development and may introduce breaking changes. Use in production at your own risk.

## Why LightProto?

[protobuf-net](https://github.com/protobuf-net/protobuf-net) is a popular Protocol Buffers implementation in .NET, but some scenarios (especially Native AOT) can be challenging due to runtime reflection and dynamic generation. LightProto addresses this with compile-time code generation and a protobuf-net–style API.

## Features

- Source generator–powered serializers/parsers at compile time
- protobuf-net–style Serializer API and familiar attributes
- Target frameworks: netstandard2.0, net8.0, net9.0, net10.0
- Performance is about 20%-50% better than protobuf-net
- AOT-friendly by design
- Stream and IBufferWriter<byte> serialization or ToByteArray
- ReadOnlySpan<byte>/ReadOnlySequence<byte>/Stream deserialization

## Performance & Benchmarks

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
// person.ToByteArray(Person.ProtoWriter); // use this overload when .netstandard2.0

// Or serialize to a Stream
using var stream = new MemoryStream();
Serializer.Serialize(stream, person);
// Serializer.Serialize(stream, person, Person.ProtoWriter); // use this overload when .netstandard2.0

byte[] data = stream.ToArray();

// Deserialize from byte[] (ReadOnlySpan<byte> overload will be used)
Person fromBytes = Serializer.Deserialize<Person>(bytes);
// Person fromBytes = Serializer.Deserialize<Person>(bytes,Person.ProtoReader); // use this overload when .netstandard2.0

// Or deserialize from Stream
using var input = new MemoryStream(data);
Person fromStream = Serializer.Deserialize<Person>(input);
// Person fromStream = Serializer.Deserialize<Person>(input,Person.ProtoReader); // use this overload when .netstandard2.0
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

### Partial classes required

protobuf-net: partial not required

LightProto: mark [ProtoContract] types as partial so the generator can emit code. 

### Generic Serialize/Deserialize type constraint

protobuf-net: Serializer.Serialize<int>(...) and Serializer.Deserialize<int>(...)

LightProto: T must implement IProtoParser<T> (i.e., a generated message type); primitives are not supported directly. Use another method which pass `IProtoReader/Writer` explicitly.
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
### .netstandard

In .netstandard targeting platform such as .NET framework, we can't use [static virtual members in interface](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members) to find ProtoReader/Writer.

So LightProto requires user to specify a ProtoWriter when serializing and a ProtoReader when deserializing.

For `[ProtoContract]` marked MessageType the ProtoReader/Writer is generated by LightProto, just use MessageType.ProtoReader/Writer.

For primitive types, LightProto has predefined in `LightProto.Parser` namespace, such as `LightProto.Parser.DateTimeParser`. 

### IExtensible

protobuf-net: supports IExtensible for dynamic extensions

LightProto: IExtensible is defined for compatibility only and has no effect

### Surrogates

protobuf-net: can register surrogates via RuntimeTypeModel at runtime

LightProto: You can specify a custom ProtoParserType for MessageType.
For example, MessageType is `Person`, custom ProtoParserType is `PersonProtoParser`, you can use following attribute to specify a custom ProtoParserType, list as precedence order:

1. member level: `[ProtoMember(1,ParserType=typeof(PersonProtoParser))]`
2. class level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
3. module/assembly level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]` (messageType and parserType should not be in same assembly, If so, type level attribute is suggested.)
4. type level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
5. default: `LightProto.Parser.PersonProtoParser`

The ProtoParserType must implement `IProtoParser<MessageType>`. The easiest way is defining a SurrogateType with `[ProtoContract]` and mark `[ProtoSurrogateFor<MessageType>]`.     
Example for Person(can be any type):

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
  public static implicit operator Person(PersonProtoParser parser) //must define implicit conversions for surrogate type
  {
      return Person.FromName(parser.Name)
  }    
  public static implicit operator PersonProtoParser(Person value) //must define implicit conversions for surrogate type
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

You can also read/write raw binary data, for example see:[DateOnlyProtoParser](https://github.com/dameng324/LightProto/blob/main/src/LightProto/Parser/DateOnly.cs) 

### StringIntern

protobuf-net: Use `RuntimeTypeModel.Default.StringInterning = true;` to enable string interning globally

LightProto: [StringIntern] attribute can apply to individual string members/class/module/assembly. 

### RuntimeTypeModel

Not supported; all configuration is static via attributes and generated code

If you encounter different behavior versus protobuf-net, please open an issue.

## Working with .proto files

LightProto doesn’t ship a .proto → C# generator. You can generate C# using protobuf-net (or other tools), then adapt the output to LightProto (typically replacing the ProtoBuf namespace with LightProto and marking types partial). If something doesn’t work, please file an issue.

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
