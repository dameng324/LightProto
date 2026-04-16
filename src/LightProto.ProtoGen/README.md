# lightproto-gen

A dotnet tool that generates LightProto `[ProtoContract]` C# classes from `.proto` files.

## Installation

```bash
dotnet tool install -g LightProto.ProtoGen
```

## Usage

### File mode

```
lightproto-gen --proto <pattern> [--output <dir>] [options]
```

### Pipe mode

When `--proto` is omitted the tool reads the proto schema from **stdin** and writes the
generated C# to **stdout** (unless `--output` is specified):

```bash
cat messages.proto | lightproto-gen --namespace MyApp.Models
```

When `--output` is also omitted, every file mode invocation writes to **stdout** too:

```bash
lightproto-gen --proto messages.proto
```

## Options

| Option | Description |
|--------|-------------|
| `--proto <pattern>` | Path(s) to `.proto` file(s). Supports glob patterns (`*.proto`, `**/*.proto`). Repeatable. If omitted, proto schema is read from stdin. |
| `--output <dir>` | Output directory for generated `.cs` files. If omitted, generated code is written to stdout. |
| `--namespace <ns>` | Override the C# namespace for all generated files. If omitted, the `csharp_namespace` option from the `.proto` file is used; otherwise the file name is used. |
| `--type-shape <value>` | C# type shape for generated message types. Values: `Default` (partial class, default), `Record` (partial record), `Struct` (partial struct), `RecordStruct` (partial record struct). |
| `--nullability <value>` | Nullability inference rule for non-repeated, non-map fields. Values: `Default` (all fields nullable, default), `StrictOptional` (only `optional`-keyword fields are nullable). |
| `--oneof <value>` | How `oneof` groups are translated to C# members. Values: `Default` (nullable properties, default), `ProtoInclude` (all-message oneofs become `[ProtoInclude]` attributes). |
| `--default-case-style <style>` | Default style for all generated identifiers when no rule matches. Values: `Pascal` (default), `Camel`, `Preserve`. |
| `--case-style "<pattern>=<style>"` | Repeatable case-style override rule matched against proto FullName (`package + nested types + member`). Glob supports `*` (single segment), `**` (0..N segments), and segment globs like `package*`. |

## Examples

```bash
# Generate from a single file (output written to stdout)
lightproto-gen --proto messages.proto

# Generate from a single file to a directory
lightproto-gen --proto messages.proto --output ./Generated

# Generate from all .proto files recursively
lightproto-gen --proto "**/*.proto" --output ./Generated --namespace MyApp.Models

# Generate with strict-optional nullability and record type shape
lightproto-gen --proto api.proto --output ./Generated --nullability StrictOptional --type-shape Record

# Generate with record struct type shape
lightproto-gen --proto api.proto --output ./Generated --type-shape RecordStruct

# Promote message-only oneofs to [ProtoInclude] inheritance attributes
lightproto-gen --proto api.proto --output ./Generated --oneof ProtoInclude

# Default Pascal, but preserve specific enum values and camel-case one member
lightproto-gen --proto api.proto --output ./Generated \
  --default-case-style Pascal \
  --case-style "market.exchange.ExchangeType.SSE=Preserve" \
  --case-style "market.exchange.Trade.exchange_type=Camel"

# Pipe mode: read schema from stdin, write C# to stdout
cat messages.proto | lightproto-gen --namespace MyApp.Models

# Pipe mode with file output
cat messages.proto | lightproto-gen --namespace MyApp.Models --output ./Generated
```

## Case style glob rules

- Rule format: `--case-style "<Pattern>=<Style>"` (repeatable)
- Pattern target: proto FullName with dot segments: `<package>.<Outer>.<Inner>...<Type>[.<Member>]`
- Wildcards:
  - `*` matches exactly one segment (does not cross `.`)
  - `**` matches zero to many segments (can cross `.`) and must be a standalone segment
  - Segment glob like `package*` is supported
- Matching is case-sensitive and does not support escaping
- Rule winner when multiple match:
  1. more literal segments
  2. then more segment-globs (e.g. `pack*`)
  3. then more `*`
  4. then fewer `**`
  5. then deeper path
  6. then later rule overrides earlier rule

## Type shape details

| `--type-shape` value | Emitted declaration |
|---------------------|---------------------|
| `Default` (default) | `public partial class Foo` |
| `Record` | `public partial record Foo` |
| `Struct` | `public partial struct Foo` |
| `RecordStruct` | `public partial record struct Foo` |

## Nullability details

| `--nullability` value | Scalar field behaviour |
|----------------------|------------------------|
| `Default` (default) | Every non-repeated, non-map field is nullable (`int?`, `string?`, …) |
| `StrictOptional` | Only fields with the `optional` keyword are nullable; others get their zero-value default |

## Oneof details

| `--oneof` value | Behaviour |
|----------------|-----------|
| `Default` (default) | Every `oneof` field is emitted as a nullable property |
| `ProtoInclude` | A `oneof` group where **all** fields are message types is replaced by `[ProtoInclude]` attributes on the containing class; the fields are not emitted as properties |
