# Dameng.Protobuf.Extension

A .NET library that enables generic protobuf deserialization by providing an interface and source generator for Google.Protobuf generated classes. Designed as a NativeAOT-compatible alternative to protobuf-net for applications requiring ahead-of-time compilation.

## Overview

This library allows you to write generic deserialization functions for protobuf messages without knowing the specific type at compile time. It achieves this by adding a `IPbMessageParser<TSelf>` interface to protobuf generated classes through source generation.

**Why Choose This Over protobuf-net?**

This library is specifically designed for **NativeAOT compatibility**, making it an ideal replacement for protobuf-net in scenarios where ahead-of-time compilation is required. Unlike protobuf-net, which relies heavily on reflection and runtime code generation, this solution uses source generation to provide the same generic capabilities with full NativeAOT support.

## Features

- **Generic Deserialization**: Write type-safe generic methods for protobuf deserialization
- **Source Generator**: Automatically implements the interface for all protobuf generated classes
- **Zero Runtime Overhead**: All code generation happens at compile time
- **Easy Integration**: Just add the NuGet packages and configure your protobuf files
- **NativeAOT Compatible**: Full support for ahead-of-time compilation scenarios
- **protobuf-net Migration**: Smooth transition path from protobuf-net for NativeAOT projects

## Installation

Install both packages from NuGet:

```bash
dotnet add package Dameng.Protobuf.Extension
dotnet add package Dameng.Protobuf.Extension.Generator
```

## Migration from protobuf-net

If you're currently using protobuf-net and need NativeAOT support, this library provides a migration path to Google.Protobuf with equivalent generic capabilities.

### Why Migrate?

**protobuf-net Limitations with NativeAOT:**
- Heavy reliance on reflection and runtime code generation
- Limited compatibility with ahead-of-time compilation
- Performance overhead in NativeAOT scenarios

**Benefits of This Solution:**
- Full NativeAOT compatibility through source generation
- Better performance with ahead-of-time compilation
- Type-safe generic operations without reflection
- Maintains similar API patterns to protobuf-net

### Migration Steps

1. **Replace protobuf-net packages** with Google.Protobuf and this extension:
```bash
# Remove protobuf-net
dotnet remove package protobuf-net

# Add Google.Protobuf and this extension
dotnet add package Google.Protobuf
dotnet add package Dameng.Protobuf.Extension
dotnet add package Dameng.Protobuf.Extension.Generator
```

2. **Convert your .proto files** from protobuf-net attributes to standard protobuf:
```protobuf
// Before (protobuf-net style)
// [ProtoContract]
// public class MyMessage { [ProtoMember(1)] public string Name { get; set; } }

// After (standard .proto file)
syntax = "proto3";
message MyMessage {
  string name = 1;
}
```

3. **Update your .csproj** to generate Google.Protobuf classes:
```xml
<ItemGroup>
  <ProtoBuf Include="your-proto-file.proto">
    <GrpcServices>Client</GrpcServices>
    <Generator>MSBuild:PreCompile</Generator>
  </ProtoBuf>
</ItemGroup>
```

4. **Update your generic serialization code**:
```csharp
// Before (protobuf-net)
T Deserialize<T>(byte[] bytes) where T : class, new()
{
    using var stream = new MemoryStream(bytes);
    return ProtoBuf.Serializer.Deserialize<T>(stream);
}

// After (with this extension)
T Deserialize<T>(byte[] bytes) where T : IPbMessageParser<T>, IMessage<T>
{
    return T.Parser.ParseFrom(bytes);
}
```

5. **Update serialization calls**:
```csharp
// Before (protobuf-net)
byte[] Serialize<T>(T obj) where T : class
{
    using var stream = new MemoryStream();
    ProtoBuf.Serializer.Serialize(stream, obj);
    return stream.ToArray();
}

// After (Google.Protobuf)
byte[] Serialize<T>(T message) where T : IPbMessageParser<T>, IMessage<T>
{
    return message.ToByteArray();
}
```

### Key Differences

| Aspect | protobuf-net | This Extension + Google.Protobuf |
|--------|-------------|----------------------------------|
| **NativeAOT** | Limited support | Full support |
| **Code Generation** | Runtime | Compile-time |
| **Schema Definition** | Attributes | .proto files |
| **Performance** | Good | Better (especially NativeAOT) |
| **Reflection Usage** | Heavy | None |


## Usage

### 1. Configure Your Protobuf Files

In your `.csproj` file, ensure your protobuf files have the `MSBuild:PreCompile` generator:

```xml
<ItemGroup>
  <ProtoBuf Include="your-proto-file.proto">
    <GrpcServices>Client</GrpcServices>
    <Generator>MSBuild:PreCompile</Generator>
  </ProtoBuf>
</ItemGroup>
```

### 2. Write Generic Deserialization Methods

Once configured, you can write generic methods that work with any protobuf message type:

```csharp
using Dameng.Protobuf.Extension;
using Google.Protobuf;

// Generic deserialization method
T Deserialize<T>(byte[] bytes) where T : IPbMessageParser<T>, IMessage<T>
{
    return T.Parser.ParseFrom(bytes);
}

// Usage with any protobuf generated class
var request = Deserialize<MyRequest>(requestBytes);
var response = Deserialize<MyResponse>(responseBytes);
```

### 3. What the Source Generator Does

The source generator automatically adds the `IPbMessageParser<TSelf>` interface to all your protobuf generated classes:

```csharp
// Generated code (automatic)
using Google.Protobuf;
using Dameng.Protobuf.Extension;

namespace YourNamespace
{
    partial class MyRequest : IPbMessageParser<MyRequest> {}
}

namespace YourNamespace  
{
    partial class MyResponse : IPbMessageParser<MyResponse> {}
}
```

## Interface Definition

The core interface is simple and leverages C# generic static interface members:

```csharp
namespace Dameng.Protobuf.Extension;

/// <summary>
/// Interface that enables generic protobuf message parsing by exposing the static Parser property.
/// This interface is automatically implemented by the source generator for all protobuf generated classes.
/// </summary>
/// <typeparam name="TSelf">The protobuf message type that implements this interface</typeparam>
public interface IPbMessageParser<TSelf> where TSelf : IPbMessageParser<TSelf>, IMessage<TSelf>
{
    /// <summary>
    /// Gets the static parser instance for the protobuf message type.
    /// This property is automatically available on all protobuf generated classes.
    /// </summary>
    public static abstract Google.Protobuf.MessageParser<TSelf> Parser { get; }
}
```

## Example Scenario

Consider you're building a generic TCP communication library that needs to deserialize different message types:

```csharp
public class GenericProtobufHandler
{
    public T HandleMessage<T>(byte[] messageBytes) 
        where T : IPbMessageParser<T>, IMessage<T>
    {
        // Generic deserialization - works with any protobuf message
        return T.Parser.ParseFrom(messageBytes);
    }
    
    public byte[] SerializeMessage<T>(T message) 
        where T : IPbMessageParser<T>, IMessage<T>
    {
        return message.ToByteArray();
    }
}

// Usage
var handler = new GenericProtobufHandler();
var request = handler.HandleMessage<MyRequest>(requestBytes);
var response = handler.HandleMessage<MyResponse>(responseBytes);
```

## Requirements

- .NET 8.0 or higher
- Google.Protobuf package  
- C# 11.0 or higher (for generic static interface members)
- **NativeAOT Compatible**: Full support for ahead-of-time compilation when using `<PublishAot>true</PublishAot>`

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.