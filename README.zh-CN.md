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

面向 C#/.NET 的高性能、Native AOT 友好、生产可用的 Protocol Buffers 实现，基于源代码生成器。

## 为什么选择 LightProto? 🤔

[protobuf-net](https://github.com/protobuf-net/protobuf-net) 是 .NET 中常用的 Protocol Buffers 实现，但在某些场景（尤其是 Native AOT）会因运行时反射和动态生成而受限。LightProto 通过编译期代码生成和 protobuf-net 风格 API 解决了这些问题。

## 主要特性 ✨

- 由源代码生成器在编译期生成序列化/反序列化代码
- AOT 友好设计，无 IL 警告
- 最低 C# 9.0，兼容性更好（包括 Unity）
- 无第三方依赖
- protobuf-net 风格的 Serializer API 与熟悉的特性
- 性能约比 protobuf-net 快 20%~50%，详见下方“性能与基准测试”
- 目标框架：netstandard2.0、net8.0、net9.0、net10.0
- 支持 Stream、IBufferWriter<byte> 序列化，或使用 ToByteArray
- 支持 ReadOnlySpan<byte>/ReadOnlySequence<byte>/Stream 反序列化
- 动态与非泛型序列化/反序列化 API
- 类似 RuntimeTypeModel 的动态消息类型 API
- 支持 Surrogate

## 丰富的内置类型支持 🧰

- .NET 基元类型（`byte`,`sbyte`, `int`,`uint`,`long`,`ulong`, `bool`, `char`, `double`, etc.）
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

## 快速开始 ⚡

从 NuGet 安装：

```bash
dotnet add package LightProto
```

使用 LightProto 特性定义你的协议类型（partial 类）：

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

## 从 protobuf-net 迁移 🔁

大多数代码只需替换命名空间并将类型标记为 partial。

1. 将 ProtoBuf 替换为 LightProto。
2. 将可序列化类型标记为 partial。

示例：

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

## 序列化 API 🧩

### 泛型约束 API 🔒

`Serializer.Serialize<T>(...)` 和 `Serializer.Deserialize<T>(...)` 要求 `T` 实现 `IProtoParser<T>`（即生成的消息类型）。

**注意: ** 这些 API 在 .netstandard2.0 中不可用，因为接口不支持静态虚成员。请改用指定 IProtoParser 的 API。

### 指定 IProtoParser 的 API 🧭

`Serializer.Serialize<T>(..., IProtoWriter<T>)` 和 `Serializer.Deserialize<T>(..., IProtoReader<T>)`，其中 `T` 为标记了 `[ProtoContract]` 且生成了 `IProtoParser<T>` 的类型。

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.Serialize<Person>(bufferWriter, person, Person.ProtoWriter); // must pass writer
var bytes = person.ToByteArray(Person.ProtoWriter); // extension method
Person result = LightProto.Serializer.Deserialize<Person>(bytes, Person.ProtoReader); // must pass reader
```

### 动态 API 🌀

`Serializer.SerializeDynamically<T>(...)` 和 `Serializer.DeserializeDynamically<T>(...)` 会在运行时通过 `T.ProtoReader/Writer`、`Serializer.RegisterParser` 或反射解析 `IProtoReader/Writer`。

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.SerializeDynamically<Person>(bufferWriter, person); // dynamic API
Person result = LightProto.Serializer.DeserializeDynamically<Person>(bufferWriter.WrittenSpan); // dynamic API
```

ProtoWriter/Reader 的解析顺序：

1. 如果通过 `Serializer.RegisterParser<T>(reader, writer)` 注册了 parser，则使用已注册的 parser。
2. 如果 `T` 是基元/内置类型，则使用 `LightProto.Parser` 命名空间下的内置 parser（内部已注册）。
3. 如果 `T` 实现 `IProtoParser<T>`（通常标记为 `[ProtoContract]`），则通过反射使用 `T.ProtoWriter/Reader`。这在 AOT 下是安全的，因为泛型参数 T 标记了 `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]`。
4. 如果 `T` 是泛型容器形态（如 `List<>`、`Dictionary<,>`、`Nullable<>` 等），将递归解析元素类型的 parser。在 AOT 下可能因缺少类型元数据而在运行时失败。

### 非泛型 API 🧱

`Serializer.SerializeNonGeneric(..., object instance)` 和 `Serializer.DeserializeNonGeneric(Type type, ...)` 与动态 API 类似，但类型在运行时指定。

```csharp
Person person = new Person { Name = "Alice", Age = 30 };
ArrayBufferWriter<byte> bufferWriter = new ArrayBufferWriter<byte>();
LightProto.Serializer.SerializeNonGeneric(bufferWriter, person); // non-generic API
Person result = (Person)LightProto.Serializer.DeserializeNonGeneric(typeof(Person), bufferWriter.WrittenSpan); // non-generic API
```

ProtoWriter/Reader 的解析顺序与“动态 API”一致。

## .NET Standard 📦

在 .NET Standard 目标框架（如 .NET Framework）中，无法使用[接口中的静态虚成员](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members)来查找 `T.ProtoReader/Writer`。

因此，LightProto 要求在序列化时指定 `ProtoWriter`，在反序列化时指定 `ProtoReader`。

对标记了 `[ProtoContract]` 的消息类型，`ProtoReader/Writer` 会由 LightProto 生成，直接使用 `MessageType.ProtoReader/Writer` 即可。

对基元类型，LightProto 在 `LightProto.Parser` 命名空间中提供了预置 parser，例如 `LightProto.Parser.DateTimeParser`。

如果不需要 AOT 支持，可以使用动态 API `Serializer.SerializeDynamically<T>` 和 `Serializer.DeserializeDynamically<T>`，无需传入 ProtoReader/Writer。

### Unity 支持 🎮

LightProto 生成的代码支持 C# 9，可用于面向 .NET Standard 2.0 的 Unity 项目。按常规 .NET 项目的安装与使用方式即可。
由于 LightProto AOT 友好，因此支持 IL2CPP 构建。

## Surrogate（代理类型）🧬

protobuf-net 可以在运行时通过 RuntimeTypeModel 注册 surrogate。

LightProto 允许为 MessageType 指定自定义 ProtoParserType。
例如 MessageType 为 `Person`，自定义 ProtoParserType 为 `PersonProtoParser`，可以按以下优先级使用对应特性：

1. member level: `[ProtoMember(1,ParserType=typeof(PersonProtoParser))]`
2. class level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
3. module/assembly level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`（messageType 与 parserType 不应在同一程序集；若在同一程序集，建议使用类型级特性）
4. type level: `[ProtoParserTypeMap(typeof(Person), typeof(PersonProtoParser))]`
5. default: `global::LightProto.Parser.PersonProtoParser`

ProtoParserType 必须实现 `IProtoParser<MessageType>`。最简单的方式是定义一个带 `[ProtoContract]` 的 SurrogateType，并标记 `[ProtoSurrogateFor<MessageType>]`。
以 Person 为例（可替换为任意类型）：

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

你也可以读写原始二进制数据，但目前仅支持 WireType.LengthDelimited，因为 LightProtoGenerator 需要在编译期计算 tag，未知类型会被视为 LengthDelimited。

## 自定义 repeated 集合 🔁

当自定义的 `IEnumerable<T>` 集合需要编码为 protobuf 的 `repeated` 字段，而不是作为 Surrogate message 时，使用 `ProtoRepeatedParserType`。源代码生成器会将元素 parser、字段 tag 和元素固定长度传给适配器，因此 packed 和 unpacked 字段仍遵循标准 protobuf wire format。

```csharp
[ProtoRepeatedParserType(typeof(MyListReader<>), typeof(MyListWriter<>))]
public sealed class MyList<T> : IEnumerable<T>
{
    // 实现 IEnumerable<T>、Add(T)、Count，以及可接收 capacity 的构造函数。
}

public sealed class MyListReader<T> : IEnumerableProtoReader<MyList<T>, T>
{
    public MyListReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            static capacity => new MyList<T>(capacity),
            static (collection, item) => { collection.Add(item); return collection; },
            itemFixedSize) { }
}

public sealed class MyListWriter<T> : IEnumerableProtoWriter<MyList<T>, T>
{
    public MyListWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

[ProtoContract]
public partial class Message
{
    [ProtoMember(1, IsPacked = true)]
    public MyList<int> Values { get; set; } = new(0);
}
```

对于无法直接添加特性的第三方集合类型，可以在包含它的 contract、module 或 assembly 上使用 `ProtoRepeatedParserTypeMap`：

```csharp
[ProtoRepeatedParserTypeMap(typeof(MyList<>), typeof(MyListReader<>), typeof(MyListWriter<>))]
[ProtoContract]
public partial class Message
{
    [ProtoMember(1)]
    public MyList<int> Values { get; set; } = new(0);
}
```

也可以直接把 `ProtoRepeatedParserType` 标记在单个成员上。解析优先级依次为成员、包含该成员的 contract、module/assembly、集合类型。Reader 适配器必须接收 `(IProtoReader<TItem>, int itemFixedSize)`；Writer 适配器必须接收 `(IProtoWriter<TItem>, uint tag, int itemFixedSize)`。

## StringIntern 🧵

`[StringIntern]` 特性可用于单个字符串成员、类、模块或程序集。

## RuntimeTypeModel 🧠

LightProto 提供了一组 `RuntimeProtoWriter`、`RuntimeProtoReader`、`RuntimeProtoParser` API。

`RuntimeProtoParser<T>` 可在运行时获取 `IProtoReader<T>` 和 `IProtoWriter<T>`。

你可以用它们进行序列化/反序列化，或使用 Serializer.RegisterParser<T>(reader, writer) 进行全局注册，然后使用 Serializer.SerializeDynamically/DeserializeDynamically 或 Serializer.SerializeNonGeneric API。

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

如果你不需要同时支持序列化和反序列化，可以使用 `RuntimeProtoWriter<T>` 或 `RuntimeProtoReader<T>` 只创建 writer 或 reader。

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

`IExtensible` 仅为兼容性而定义，不会产生实际效果。

## 性能与基准测试 📊

以下基准测试比较了 LightProto、protobuf-net 与 Google.Protobuf 的序列化性能。

你可以克隆仓库并运行 tests/Benchmark 来复现。

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
| Deserialize_GoogleProtoBuf | .NET 8.0  | .NET 8.0 | 595.5 μs | 11.51 μs | 16.14 μs |  1.34 |    0.04 |  648.7 KB |        0.97 |
| Deserialize_LightProto     | .NET 8.0 | .NET 8.0 | 444.8 μs |  8.88 μs |  9.12 μs |  1.00 |    0.03 | 665.95 KB |        1.00 |
|                            |           |           |          |          |          |       |         |           |             |
| Deserialize_ProtoBuf_net   | .NET 9.0 | .NET 9.0 | 662.3 μs | 12.60 μs | 11.17 μs |  1.53 |    0.04 |    562 KB |        0.84 |
| Deserialize_GoogleProtoBuf | .NET 9.0 | .NET 9.0 | 491.7 μs |  9.64 μs | 13.52 μs |  1.14 |    0.04 |  648.7 KB |        0.97 |
| Deserialize_LightProto     | .NET 9.0 | .NET 9.0 | 431.9 μs |  8.33 μs |  9.25 μs |  1.00 |    0.03 | 665.95 KB |        1.00 |

注：结果会因硬件、运行时和数据模型而异，请在你的环境中运行基准测试以获得最相关的结果。

## 使用 .proto 文件 📄

LightProto 目前不提供 .proto → C# 生成器。你可以使用 protobuf-net（或其他工具）生成 C#，再适配到 LightProto（通常是将 ProtoBuf 命名空间替换为 LightProto，并将类型标记为 partial）。如有问题，请提交 issue。

如果你需要专用的 .proto → C# 生成器，请在此 [issue](https://github.com/dameng324/LightProto/issues/85) 投票支持。

## 贡献指南 🤝

欢迎贡献！请参阅 [CONTRIBUTING](CONTRIBUTING.md) 了解详细贡献指南。

[ARCHITECTURE.md](ARCHITECTURE.md) 描述了 LightProto 的内部设计与结构，供贡献者参考。

## 许可 📄

MIT License — 详见 LICENSE。
