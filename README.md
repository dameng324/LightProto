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

## Migration & Usage

### Quick Start

Add the required packages:
```bash
dotnet add package Google.Protobuf
dotnet add package Dameng.Protobuf.Extension
dotnet add package Dameng.Protobuf.Extension.Generator
```

Configure your `.csproj` with protobuf files:
```xml
<ItemGroup>
  <ProtoBuf Include="your-proto-file.proto">
    <GrpcServices>Client</GrpcServices>
    <Generator>MSBuild:PreCompile</Generator>
  </ProtoBuf>
</ItemGroup>
```

Write generic methods that work with any protobuf message:
```csharp
using Dameng.Protobuf.Extension;
using Google.Protobuf;

// Generic deserialization
T Deserialize<T>(byte[] bytes) where T : IPbMessageParser<T>, IMessage<T>
{
    return T.Parser.ParseFrom(bytes);
}

// Generic serialization  
byte[] Serialize<T>(T message) where T : IPbMessageParser<T>, IMessage<T>
{
    return message.ToByteArray();
}

// Usage with any protobuf generated class
var request = Deserialize<MyRequest>(requestBytes);
var response = Deserialize<MyResponse>(responseBytes);
```

### Migrating from protobuf-net

**Why migrate?** protobuf-net has limited NativeAOT support due to heavy reflection usage, while this library provides full compatibility through compile-time source generation.

**Migration steps:**
1. Replace `protobuf-net` with `Google.Protobuf` + this extension
2. Convert protobuf-net attributes to standard `.proto` files:
   ```protobuf
   // Standard .proto file
   syntax = "proto3";
   message MyMessage {
     string name = 1;
   }
   ```
3. Update generic code constraints from `where T : class, new()` to `where T : IPbMessageParser<T>, IMessage<T>`

**Key differences:**

| Aspect | protobuf-net | This Extension |
|--------|-------------|----------------|
| **NativeAOT** | Limited | Full support |
| **Code Generation** | Runtime | Compile-time |
| **Reflection** | Heavy usage | None |

### How It Works

The source generator automatically implements `IPbMessageParser<TSelf>` on all protobuf generated classes, exposing their static `Parser` property for generic access:

```csharp
// Auto-generated for each protobuf class
partial class MyRequest : IPbMessageParser<MyRequest> {}
```

## Example: Generic Communication Handler

```csharp
public class GenericProtobufHandler
{
    public T HandleMessage<T>(byte[] messageBytes) 
        where T : IPbMessageParser<T>, IMessage<T>
    {
        return T.Parser.ParseFrom(messageBytes);
    }
    
    public byte[] SerializeMessage<T>(T message) 
        where T : IPbMessageParser<T>, IMessage<T>
    {
        return message.ToByteArray();
    }
}
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