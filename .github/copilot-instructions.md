# GitHub Copilot Instructions for LightProto

This document provides specific guidelines for GitHub Copilot when working in the LightProto repository.

## Repository Overview

LightProto is a high-performance, Native AOT-friendly Protocol Buffers implementation for C#/.NET, powered by source generators.

- **Target Frameworks**: netstandard2.0, net8.0, net9.0, net10.0
- **Test Framework**: [TUnit](https://github.com/thomhurst/TUnit) on Microsoft.Testing platform
- **Language**: C# (minimum C# 9.0)

## Critical Requirements

### Code Formatting

**Before every commit**, you MUST format the code using:

```bash
dotnet tool run csharpier format .
```

This is non-negotiable. The repository uses CSharpier for consistent code formatting.

### Testing Requirements

When validating code changes, you MUST test the **net48 target framework** of the `LightProto.Tests` project:

```bash
cd tests/LightProto.Tests
dotnet test --framework net48
```

- Use **Mono** on non-Windows platforms
- Use native **.NET Framework** on Windows platforms

For general testing across all frameworks:

```bash
dotnet test
```

## Code Guidelines

### AOT-Friendly Design

- **Never introduce runtime reflection** for configuration or type discovery
- All serialization behavior must be driven by **compile-time attributes** and the **source generator**
- Avoid designs that dynamically walk types at runtime
- Keep `<IsAotCompatible>true</IsAotCompatible>` intact in project files

### Wire Format Compatibility

- **Do not change existing wire format semantics** without explicit discussion
- Do not modify how existing parsers encode types (e.g., ZigZag encoding, compatibility levels)
- Maintain backward compatibility with existing Protocol Buffer encodings

### Multi-Targeting

- Do not remove existing `TargetFrameworks` without consensus
- Be mindful of `netstandard2.0` limitations (e.g., no static virtual interface members)
- Test changes across all target frameworks

### Public API Stability

- Avoid breaking changes to public APIs in the `LightProto` namespace
- Changes to `Serializer`, `IProtoParser<T>`, attributes, etc., require discussion
- If breaking changes are necessary, clearly document migration paths

## Project Structure

### Runtime Library (`src/LightProto/`)

- **Public APIs**: `Serializer.*.cs`, `Attributes.cs`, `IProtoParser.cs`, `WireFormat.cs`, `CodedInputStream.cs`, `CodedOutputStream.cs`
- **Built-in Parsers**: `src/LightProto/Parser/*.cs`
- Add new type support here and hook it into generator mapping

### Source Generator (`src/LightProto.Generator/`)

- **Entry Point**: `LightProtoGenerator.cs`
- **Core Logic**: `ProtoContract`, `ProtoMember`, `GetProtoMembers`, `GetProtoParser`
- Modify for attribute support, field rules, inheritance, parser resolution

### Test Projects

- `tests/LightProto.Tests/` – primary behavior and regression tests
- `tests/LightProto.AssemblyLevelTests/` – assembly-level attribute tests
- `tests/TestAot/` – AOT validation (net9.0)
- `tests/Benchmark/` – performance benchmarks

## Testing Workflow

### Standard Test Run

```bash
# From repository root
dotnet test

# For specific project
cd tests/LightProto.Tests
dotnet test

# For net48 framework specifically
dotnet test --framework net48
```

### Benchmark Execution (Optional)

```bash
cd tests/Benchmark
dotnet run -c Release
```

### AOT Validation (Optional)

```bash
cd tests/TestAot
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

## Coding Style

### Formatting

- Always use `dotnet tool run csharpier format .` before committing
- Consider installing lefthook for automatic pre-commit formatting: `lefthook install`

### Language Features

- Use modern C# features (records, pattern matching, `using` declarations)
- Maintain consistency with surrounding code style
- Be mindful of netstandard2.0 limitations in the generator project

### Error Handling

- **Generator**: Use `LightProtoGeneratorException` with stable diagnostic IDs (e.g., `LIGHT_PROTO_00X`)
- **Runtime**: Use appropriate exceptions (`InvalidProtocolBufferException`, `ArgumentException`)
- Provide clear, actionable error messages

### Performance Considerations

- Avoid unnecessary allocations in hot paths
- Prefer `Span<T>`, `ReadOnlySpan<T>`, `ReadOnlySequence<T>`, `ArrayPool<T>`
- Avoid LINQ, `ToArray()`, excessive string concatenation in performance-critical code
- For performance changes, run benchmarks and document results

## Common Contribution Areas

### Recommended Changes

- Bug fixes (serialization, deserialization, edge cases, AOT issues)
- New built-in parsers in `LightProto.Parser` namespace
- Source generator improvements (tag inference, collections, inheritance, `ProtoParserTypeMap`)
- Performance optimizations (validated with `tests/Benchmark`)
- Tests and documentation updates

### Changes to Avoid

- Runtime reflection-based configuration
- Silent wire format changes
- Breaking public API changes without discussion
- Removing existing tests or benchmarks
- Removing target frameworks or AOT settings
- Large-scale style-only refactors

## Pre-Commit Checklist

Before submitting changes:

1. ✅ **Format code**: `dotnet tool run csharpier format .`
2. ✅ **Run tests**: `dotnet test` (including net48 tests)
3. ✅ Ensure changes are focused and minimal
4. ✅ Add/update tests for behavior changes
5. ✅ Verify no breaking API changes
6. ✅ Confirm wire format compatibility
7. ✅ Run benchmarks if performance-related (optional)

## Additional Resources

- [README.md](../README.md) – Project overview and quick start
- [ARCHITECTURE.md](../ARCHITECTURE.md) – Internal design and structure
- [CONTRIBUTING.md](../CONTRIBUTING.md) – Detailed contribution guidelines

---

**Remember**: Always format with `dotnet tool run csharpier format .` before committing, and test net48 framework!
