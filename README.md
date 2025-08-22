# Dameng.Protobuf.Extension

A .NET library that enables generic protobuf deserialization by providing an interface and source generator for Google.Protobuf generated classes.

## Overview

This library allows you to write generic deserialization functions for protobuf messages without knowing the specific type at compile time. It achieves this by adding a `IPbMessageParser<TSelf>` interface to protobuf generated classes through source generation.

## Features

- **Generic Deserialization**: Write type-safe generic methods for protobuf deserialization
- **Source Generator**: Automatically implements the interface for all protobuf generated classes
- **Zero Runtime Overhead**: All code generation happens at compile time
- **Easy Integration**: Just add the NuGet packages and configure your protobuf files

## Installation

Install both packages from NuGet:

```bash
dotnet add package Dameng.Protobuf.Extension
dotnet add package Dameng.Protobuf.Extension.Generator
```

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

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.