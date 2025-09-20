# LightProto

[![NuGet](https://img.shields.io/nuget/v/LightProto.svg)](https://www.nuget.org/packages/LightProto/)
[![License](https://img.shields.io/github/license/dameng324/LightProto.svg)](LICENSE)

A high-performance, NativeAOT-compatible Protocol Buffers implementation for C#/.NET, designed as a native aot solution
for [Protobuf-net](https://github.com/protobuf-net/protobuf-net).

## Why LightProto?

Protobuf-net is a popular choice for Protocol Buffers in .NET,However it has limitations when it comes to NativeAOT
scenarios. LightProto addresses these issues by providing a fully NativeAOT-compatible solution with zero runtime
overhead.

So, LightProto has three main goals:

1. **NativeAOT Compatibility**: Ensure full support for ahead-of-time compilation scenarios without any runtime
   reflection or code generation.
2. **Protobuf-net Compatibility**: implement the most features of Protobuf-net, making it easy to migrate existing
   Protobuf-net codebases to LightProto. but unfortunately, some features may not be supported due to NativeAOT limitations or rarely used features.
3. **Performance**: Performance should be better or at least same as Protobuf-net.

## Performance & Benchmarks

The following benchmarks compare serialization performance between LightProto, Protobuf-net, and Google.Protobuf.

You can reproduce these benchmarks by cloning the repository and running the `tests/Benchmark` project.

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

## ⚠️ Development Status

**This project is under active development and may introduce breaking changes. use it in production at your own risk.**

## Quick Start

```bash
dotnet add package LightProto
```

### 1. Configure your ProtoContracts

```cs
using LightProto;
[ProtoContract]
public partial class Person 
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]  
    public int Age { get; set; }
}
var person = new Person { Name = "Alice", Age = 30 };
// Serialization
var bytes = person.ToByteArray();
// or through API in protobuf-net style
var stream = new MemoryStream();
Serializer.Serialize(stream, person);
var bytes = stream.ToArray();

// Deserialization  
var obj = Serializer.Deserialize<Person>(bytes);
```

## Migration from Protobuf-net

Change the namespace from `ProtoBuf` to `LightProto` and ensure your classes marked as `partial`.

```diff
-using ProtoBuf;
+using LightProto;
[ProtoContract]
-public class Person
+public partial class Person
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    [ProtoMember(2)]  
    public int Age { get; set; }
}
var myObject = new MyClass { /* Initialize properties */ };
// Serialization
var stream = new MemoryStream();
Serializer.Serialize(stream, myObject);
byte[] data = stream.ToArray();

// Deserialization  
var obj = Serializer.Deserialize<MyClass>(new MemoryStream(data));
```

## Known Differences with Protobuf-net

Here are some known differences between LightProto and Protobuf-net. The goal is to minimize these differences, but if you found any other different behavior with Protobuf-net, please report them via GitHub issues.

### Partial Classes

There is no need to mark your classes as `partial` in Protobuf-net, but LightProto requires it to generate the necessary
serialization code at compile time.

### Generic Serialize / Deserialize Methods

LightProto does not support `Serialize<int>`,`Deserialize<int>` generic methods. The type parameter must be IProto,

```diff

### IExtensible

LightProto does not support the `IExtensible` interface, which is used in Protobuf-net to allow for dynamic
extensions. Instead, LightProto encourages the use of well-defined schemas.

These interface are defined in LightProto for compatibility, but they are no effect.

### Surrogate

Protobuf-net allows registering a surrogate type at runtime using `RuntimeTypeModel`. LightProto requires the use of
`[ProtoProxy]` and `ProxyFor<T>` attributes to define surrogate types at compile time.

## .proto files

LightProto does not include a built-in tool for generating C# classes from `.proto` files. However, you can use the
`protobuf-net` tool to generate the classes and then modify them to be compatible with LightProto(just replace `ProtoBuf` to `LightProto` should work, if not, fire an issue please).

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

**Before contributing:**

1. Check existing issues and discussions
2. Follow the coding standards used in the project
3. Add tests for new functionality
4. Update documentation as needed

**Development Setup:**

```bash
git clone https://github.com/dameng324/LightProto.git
cd LightProto
dotnet restore
dotnet build
dotnet test
```