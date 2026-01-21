# Diagnostics

## LIGHT_PROTO_W001

Member has default value which may break deserialization.

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)]
    public int MyValue { get; set; } = 42; // Warning: LIGHT_PROTO_W001
}

var instance = new MyClass(){ MyValue = 0 };
var cloned = Serializer.DeepClone(instance);
Console.WriteLine(cloned.MyValue); // Output: 42 which may be unexpected
```

The cause of this issue is that when `MyValue = 0`, the serializer does not write it to the stream because it is the default value. During deserialization, since the value of `MyValue` is not read from the stream, it is not set, causing it to retain its default value of `42`.

This behavior is consistent with **protobuf-net**.

**Fixes:**

1. Use `[ProtoContract(SkipConstructor = true)]` to avoid invoking the default constructor.
2. Avoid assigning default values to members.

## LIGHT_PROTO_W002

Member is marked as IsPacked but is not a collection type

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1, IsPacked = true)] // Warning: LIGHT_PROTO_W002
    public int MyValue { get; set; }
}
```

The cause of this issue is that the `IsPacked` option is only applicable to collection types (e.g., arrays, lists). Applying it to a non-collection type like `int` is incorrect and will be ignored.

## LIGHT_PROTO_W003

Member is marked as IsPacked but the item type does not support packed encoding

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1, IsPacked = true)] // Warning: LIGHT_PROTO_W003
    public List<string> MyStrings { get; set; }
}
```
The cause of this issue is that the `IsPacked` option is only applicable to numeric types (e.g., int, float). Applying it to non-numeric types like `string` is incorrect and will be ignored.

Supported packed types include:

- Boolean(`bool`)
- Int16(`short`)
- UInt16(`ushort`)
- Int32(`int`)
- UInt32(`uint`)
- Int64(`long`)
- UInt64(`ulong`)
- Byte(`byte`)
- SByte(`sbyte`)
- Single(`float`)
- Double(`double`)
- Char(`char`)
- Enum(`enum`)

## LIGHT_PROTO_W004

StringInternAttribute applied to non-string type

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1), StringIntern] // Warning: LIGHT_PROTO_W004
    public int MyValue { get; set; }
}
```
The cause of this issue is that the `StringIntern` attribute is only applicable to string types. Applying it to non-string types like `int` is incorrect and will be ignored.

## LIGHT_PROTO_W005

Member has CompatibilityLevel.Level240 but the type does not support it.

```csharp
[ProtoContract()]
public partial class MyClass
{
    [ProtoMember(1)]
    [CompatibilityLevel(CompatibilityLevel.Level240)]
    public Guid MyGuid { get; set; } // Warning: LIGHT_PROTO_W005
}

```

The cause of this issue is that when using `CompatibilityLevel.Level240`, only `DateTime` and `TimeSpan` are supported as well-known types. Using other types like `Guid` is not supported and will be ignored.

DateTime will be mapped to `google.protobuf.Timestamp` and TimeSpan will be mapped to `google.protobuf.Duration` in this compatibility level.

## LIGHT_PROTO_W006

Member has CompatibilityLevel.Level300 but the type does not support it

```csharp
[ProtoContract()]
public partial class MyClass
{
    [ProtoMember(1)]
    [CompatibilityLevel(CompatibilityLevel.Level300)]
    public string MyString { get; set; } // Warning: LIGHT_PROTO_W006
}
```

The cause of this issue is that when using `CompatibilityLevel.Level300`, only `Guid` and `decimal` and `CompatibilityLevel.Level240` supported types are supported as well-known types. Using other types like `string` is not supported and will be ignored.

`Guid` will be mapped to `string` and `decimal` will be mapped to `string` in this compatibility level.

When using `CompatibilityLevel.Level300` on `CompatibilityLevel.Level240` supported types, `CompatibilityLevel.Level240` will affect the serialization instead.

## LIGHT_PROTO_W007

Member has DataFormat.ZigZag but the type does not support it
```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1, DataFormat = DataFormat.ZigZag)] // Warning: LIGHT_PROTO_W007
    public uint MyValue { get; set; }
}
```
The cause of this issue is that the `DataFormat.ZigZag` option is only applicable to signed integer types (e.g., `int`, `long`, `short`). Applying it to unsigned types like `uint` is incorrect and will be ignored.

Supported ZigZag types include:
- SByte(`sbyte`)
- Int16(`short`)
- Int32(`int`)
- Int64(`long`)

## LIGHT_PROTO_W008

Member has DataFormat.FixedSize but the type does not support it

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)] // Warning: LIGHT_PROTO_W008
    public string MyValue { get; set; }
}
```
The cause of this issue is that the `DataFormat.FixedSize` option is only applicable to fixed-size types (e.g., `float`, `double`, `int`, `long`). Applying it to non-fixed-size types like `string` is incorrect and will be ignored.

Supported FixedSize types include:
- SByte(`sbyte`)
- Int16(`short`)
- Int32(`int`)
- Int64(`long`)
- Byte(`byte`)
- UInt16(`ushort`)
- UInt32(`uint`)
- UInt64(`ulong`)

`float` and `double` are not checked because they always use fixed-size encoding.

## LIGHT_PROTO_W009

Member has ProtoMapAttribute but is not a dictionary type

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)] 
    [ProtoMap(KeyFormat = DataFormat.ZigZag, ValueFormat = DataFormat.FixedSize)] // Warning: LIGHT_PROTO_W009
    public List<int> MyValues { get; set; }
}
```

The cause of this issue is that the `ProtoMap` attribute is only applicable to dictionary types (e.g., `Dictionary<TKey, TValue>`). Applying it to non-dictionary types like `List<int>` is incorrect and will be ignored.

## LIGHT_PROTO_W010

Member has ProtoMapAttribute with DataFormat.ZigZag on key but the key type does not support it
```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)] 
    [ProtoMap(KeyFormat = DataFormat.ZigZag, ValueFormat = DataFormat.Default)] // Warning: LIGHT_PROTO_W010
    public Dictionary<uint, string> MyMap { get; set; }
}
```
The cause of this issue is that the `DataFormat.ZigZag` option on the key is only applicable to signed integer types (e.g., `int`, `long`, `short`). Applying it to unsigned types like `uint` is incorrect and will be ignored.

Supported ZigZag types can be found in the [LIGHT_PROTO_W007](#light_proto_w007) section.

## LIGHT_PROTO_W011

Member has ProtoMapAttribute with DataFormat.FixedSize on key but the key type does not support it
```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)] 
    [ProtoMap(KeyFormat = DataFormat.FixedSize, ValueFormat = DataFormat.Default)] // Warning: LIGHT_PROTO_W011
    public Dictionary<string, int> MyMap { get; set; }
}
```
The cause of this issue is that the `DataFormat.FixedSize` option on the key is only applicable to fixed-size types (e.g., `float`, `double`, `int`, `long`). Applying it to non-fixed-size types like `string` is incorrect and will be ignored.

Supported FixedSize types can be found in the [LIGHT_PROTO_W008](#light_proto_w008) section.

## LIGHT_PROTO_W012

Member has ProtoMapAttribute with DataFormat.ZigZag on value but the value type does not support it

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)] 
    [ProtoMap(KeyFormat = DataFormat.Default, ValueFormat = DataFormat.ZigZag)] // Warning: LIGHT_PROTO_W012
    public Dictionary<string, uint> MyMap { get; set; }
}
```
The cause of this issue is that the `DataFormat.ZigZag` option on the value is only applicable to signed integer types (e.g., `int`, `long`, `short`). Applying it to unsigned types like `uint` is incorrect and will be ignored.

Supported ZigZag types can be found in the [LIGHT_PROTO_W007](#light_proto_w007) section.

## LIGHT_PROTO_W013

Member has ProtoMapAttribute with DataFormat.FixedSize on value but the value type does not support it

```csharp
[ProtoContract]
public partial class MyClass
{
    [ProtoMember(1)] 
    [ProtoMap(KeyFormat = DataFormat.Default, ValueFormat = DataFormat.FixedSize)] // Warning: LIGHT_PROTO_W013
    public Dictionary<string, string> MyMap { get; set; }
}
```
The cause of this issue is that the `DataFormat.FixedSize` option on the value is only applicable to fixed-size types (e.g., `float`, `double`, `int`, `long`). Applying it to non-fixed-size types like `string` is incorrect and will be ignored.

Supported FixedSize types can be found in the [LIGHT_PROTO_W008](#light_proto_w008) section.