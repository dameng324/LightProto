# Contributing to LightProto

This document explains how to contribute to LightProto: what you can change, what you should avoid, and how to run tests and benchmarks.
For architecture details, see `ARCHITECTURE.md` at the repository root.

---

## 1. Overall Contribution Workflow

1. **Understand the existing design first**
   - Read: `README.md`, `ARCHITECTURE.md`, key files under `src/LightProto/` and `src/LightProto.Generator/`, and relevant tests under `tests/`.
2. **Discuss non-trivial changes before implementing**
   - For new features or behavior changes, open an issue or discussion describing your proposal, and reference it from your PR.
3. **Keep PRs focused and minimal**
   - One PR should address one logical change (e.g. "add a new built-in parser", "fix encoding of a specific field", "add tests for feature X").
   - Avoid large-scale reformatting, renaming, or unrelated refactors.
4. **Update or add tests**
   - Any behavior change must be covered by updated or new tests.
   - Bug fixes should include a regression test demonstrating the fixed behavior.
5. **Run tests locally**
   - At minimum, run `dotnet test` (see [Section 4](#4-tests-and-benchmarks)) and ensure relevant tests pass.

---

## 2. What You May Change vs. Should Avoid

### 2.1 Recommended contribution areas

- **Bug fixes**
  - Incorrect serialization/deserialization, unclear or wrong exceptions, edge cases, AOT-related issues, etc.
- **New built-in parsers / type support**
  - Add support for common types in the `LightProto.Parser` namespace (and wire them into the generator mapping logic).
- **Source generator behavior improvements**
  - Improve tag inference, collection/dictionary handling, inheritance and `ProtoInclude`, `ProtoParserTypeMap` resolution, etc.
- **Performance optimizations**
  - Optimize hot paths in parsers, `Serializer`, `CodedInputStream`/`CodedOutputStream`, etc., validated via `tests/Benchmark`.
- **Tests and documentation**
  - Add or improve unit and integration tests.
  - Update documentation (`README.md`, `ARCHITECTURE.md`, `CONTRIBUTING.md`, etc.) when directly related to your changes.

### 2.2 Changes to avoid or be very careful with

1. **Do not introduce runtime reflection-based configuration paths**
   - LightProto is designed to be *Native AOT friendly*; serialization behavior must be driven by compile-time attributes and the source generator.
   - Avoid designs that walk types with reflection at runtime to dynamically decide fields/tags.

2. **Do not silently change existing wire format semantics**
   - Do not change how existing parsers encode the same type (e.g. ZigZag rules for `int`, compatibility-level-specific encodings for `Guid`/`decimal`).
   - Do not change or reuse existing `[ProtoMember(tag)]` numeric tags for different fields.

3. **Be conservative with public API changes**
   - Public types in the `LightProto` namespace (e.g. `Serializer`, `IProtoParser<T>`, attributes) should not change signature or semantics without prior discussion.
   - If a breaking change is truly needed, mark it clearly in the PR description and describe the migration path.

4. **Do not remove existing tests or benchmarks casually**
   - Only remove tests when they are clearly invalid or tied to code that is legitimately being deleted or fundamentally reworked.
   - The benchmark project (`tests/Benchmark`) is a long-term performance reference and should not lose existing scenarios without strong justification.

5. **Keep multi-targeting and AOT-related settings intact**
   - Do not remove existing `TargetFrameworks` (e.g. `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`) without broad consensus and clear justification.
   - Do not remove `<IsAotCompatible>true</IsAotCompatible>` or similar AOT-related settings from project files.

6. **Avoid large-scale style-only refactors**
   - Do not reformat the entire codebase or enforce a new style across all files.
   - Follow the "local style" rule: keep new code consistent with the style already used in the file.

---

## 3. Code Layout and Where to Put Changes (Short Version)

> For detailed architecture, see `ARCHITECTURE.md`.

- **Runtime library**: `src/LightProto/`
  - Public API: `Serializer.*.cs`, `Attributes.cs`, `IProtoParser.cs`, `WireFormat.cs`, `CodedInputStream.cs`, `CodedOutputStream.cs`, etc.
  - Built-in parsers: `src/LightProto/Parser/*.cs`.
  - If your change is:
    - Adding support for a primitive/collection type: implement/extend a parser under `Parser/` and hook it up in the generator mapping.
    - Adjusting serialization behavior: change the relevant parser and/or `Serializer` helper methods.

- **Source generator**: `src/LightProto.Generator/`
  - Entry point: `LightProtoGenerator.cs`.
  - Core modeling logic: inner types like `ProtoContract` and `ProtoMember`, and methods such as `GetProtoMembers` / `GetProtoParser`.
  - If your change is:
    - Supporting new attributes, implicit field rules, inheritance/interface scenarios, or parser resolution rules, this is the right place.

- **Test projects**:
  - `tests/LightProto.Tests/` – primary location for behavior and regression tests.
  - `tests/LightProto.AssemblyLevelTests/` – tests for assembly-level `[ProtoParserTypeMap]` and cross-assembly behavior.
  - `tests/TestAot/` – AOT sample; usually only adjusted for AOT-specific regressions.
  - `tests/Benchmark/` – performance benchmarks.

---

## 4. Tests and Benchmarks

### 4.1 Test framework and style

- The test framework is **[TUnit](https://github.com/thomhurst/TUnit)** on top of the `Microsoft.Testing` platform.
- Typical style (see `tests/LightProto.Tests/SerializerTests.cs`):

  ```csharp
  namespace LightProto.Tests;

  public partial class SerializerTests
  {
      [Test]
      public async Task TestBufferWriter()
      {
          ArrayBufferWriter<byte> bufferWriter = new();
          var obj = CreateTestContract();
          Serializer.Serialize(bufferWriter, obj);

          var data = bufferWriter.WrittenSpan.ToArray();
          var parsed = Serializer.Deserialize<TestContract>(data);

          await Assert.That(parsed.Name).IsEquivalentTo(obj.Name);
      }
  }
  ```

- Common TUnit attributes:
  - `[Test]` – regular test.
  - `[Arguments(...)]` – parameterized tests (see `DeserializeItems` and related tests).

### 4.2 Running all tests

From the repository root:

```bash
dotnet test
```

This builds and runs all test projects, including:

- `tests/LightProto.Tests`
- `tests/LightProto.AssemblyLevelTests`

To run just one test project:

```bash
cd tests/LightProto.Tests
dotnet test
```

### 4.3 Running benchmarks (optional)

> Benchmarks are mainly for evaluating performance impact; they are usually not required in CI but are recommended for perf-related changes.

From the benchmark project directory:

```bash
cd tests/Benchmark
dotnet run -c Release
```

- This uses BenchmarkDotNet and will automatically run jobs for multiple target frameworks.
- To run a specific benchmark type or method, use BenchmarkDotNet filters (see its documentation).

### 4.4 AOT validation (optional)

`tests/TestAot` is a small `net9.0` AOT console app using Google.Protobuf-generated types:

```bash
cd tests/TestAot
# Example publish command; adjust runtime identifier as needed
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

- If you changed AOT-related code paths (e.g. reflection usage, trimming-sensitive APIs, AOT compatibility flags), it is recommended to ensure this project still publishes and runs.

---

## 5. Coding Style and Practices

### 5.1 Formatting and git hooks

- To format the codebase consistently, use the local dotnet tool:
  ```bash
  dotnet tool run csharpier format .
  ```
- To enable local pre-commit checks (if you have [lefthook](https://github.com/evilmartians/lefthook) installed), install the repo hooks:
  ```bash
  lefthook install
  ```

### 5.2 C# language features

- You can use modern C# features (records, pattern matching, `using` declarations, etc.), but:
  - Keep style consistent with the surrounding file.
  - Be mindful of target frameworks (especially the generator project which targets only `netstandard2.0`).

### 5.3 Error handling and diagnostics

- In the source generator, prefer using `LightProtoGeneratorException` to report structured diagnostics:
  - Give each diagnostic a stable Id (e.g. `LIGHT_PROTO_00X`).
  - Provide actionable, clear messages that help users fix their code.
- In runtime code, avoid swallowing exceptions; use appropriate exception types such as `InvalidProtocolBufferException` or `ArgumentException` for protocol and API misuse.

### 5.4 Performance considerations

- Core `Serializer` and parser paths are performance-sensitive:
  - Avoid unnecessary allocations (especially LINQ, `ToArray()`, excessive string concatenation).
  - Prefer `Span<T>`, `ReadOnlySpan<T>`, `ReadOnlySequence<T>`, and `ArrayPool<T>` where appropriate.
- For performance-affecting changes, it is recommended to:
  - Add or adjust benchmarks in `tests/Benchmark`.
  - Include a brief before/after summary from BenchmarkDotNet output in the PR description.

---

## 6. Suggested PR Checklist

Before opening a PR, consider checking the following:

1. [ ] Scope is clear and focused (one PR does one thing).
2. [ ] Code changes are placed in appropriate projects/files (runtime vs generator vs tests).
3. [ ] Behavior changes are covered by updated or new tests.
4. [ ] You have run locally at least:
   - [ ] `dotnet tool run csharpier format .` (or install lefthook pre-commit hooks to automate this).
   - [ ] `dotnet test` (or the relevant individual test project).
5. [ ] No existing public APIs were removed or renamed without clearly flagging a breaking change.
6. [ ] Existing wire format semantics are unchanged, unless this is an intentional and well-discussed breaking change.
7. [ ] For performance-related changes, `tests/Benchmark` has been run and results are summarized in the PR (optional but recommended).

Thank you for contributing to LightProto!
