# Diagnostics

## LIGHT_PROTO_W001

Member has default value which may break deserialization.

```csharp
[ProtoContract]
public class MyClass
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
