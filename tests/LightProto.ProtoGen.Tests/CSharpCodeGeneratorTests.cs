using CSharpier.Core.CSharp;
using Google.Protobuf.Reflection;
using LightProto.ProtoGen;
using ProtoBuf.Reflection;

namespace LightProto.ProtoGen.Tests;

/// <summary>
/// Tests for <see cref="LightProtoCSharpGenerator"/>.
///
/// Assertions use CSharpier to normalize whitespace before comparing, so trivial
/// formatting differences (spaces, newlines) do not cause spurious failures.
/// </summary>
public class LightProtoCSharpGeneratorTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Parses <paramref name="protoSchema"/> and generates C# code with the given options.
    /// </summary>
    private static string Generate(string protoSchema, string? ns = null, GeneratorOptions? opts = null)
    {
        var set = new FileDescriptorSet();
        using (var reader = new System.IO.StringReader(protoSchema))
        {
            set.Add("test.proto", source: reader);
        }
        set.Process();
        var errors = set.GetErrors();
        if (errors.Length > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(e => e.ToString())));

        var file = set.Files.First();
        return new LightProtoCSharpGenerator(opts).Generate(file, ns);
    }

    /// <summary>
    /// Formats a C# code string with CSharpier so that whitespace differences are ignored.
    /// Returns the formatted code, or the original if CSharpier reports an error.
    /// </summary>
    private static string Fmt(string code)
    {
        // CSharpier.Core.CSharpFormatter.FormatAsync is async; run synchronously in tests.
        var task = CSharpFormatter.FormatAsync(code, new CSharpier.Core.CodeFormatterOptions());
        task.Wait();
        var result = task.Result;
        // If CSharpier couldn't parse, return the original to surface the diff clearly.
        return result.Code ?? code;
    }

    /// <summary>
    /// Normalizes C# code by formatting with CSharpier then stripping leading whitespace from
    /// each line. This allows containment checks that are insensitive to indentation level.
    /// </summary>
    private static string Normalize(string code)
    {
        var formatted = Fmt(code);
        var lines = formatted.Split('\n').Select(l => l.TrimStart());
        return string.Join('\n', lines);
    }

    /// <summary>
    /// Asserts that the generated code, once formatted and normalized by CSharpier, contains the
    /// CSharpier-formatted and normalized expected snippet.
    /// </summary>
    private static async Task AssertContainsFormatted(string generatedCode, string expectedSnippet)
    {
        var normalizedGenerated = Normalize(generatedCode);
        var normalizedExpected = Normalize(expectedSnippet).Trim();
        await Assert.That(normalizedGenerated).Contains(normalizedExpected);
    }

    // -------------------------------------------------------------------------
    // Baseline: attributes and class declaration
    // -------------------------------------------------------------------------

    [Test]
    public async Task GeneratesProtoContractAttribute()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message MyMessage { int32 id = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("[global::LightProto.ProtoContract]");
        await Assert.That(code).Contains("public partial class MyMessage");
    }

    [Test]
    public async Task GeneratesProtoMemberTagNumbers()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { string first = 1; int32 second = 42; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("[global::LightProto.ProtoMember(1)]");
        await Assert.That(code).Contains("[global::LightProto.ProtoMember(42)]");
    }

    // -------------------------------------------------------------------------
    // Scalar types – nullable by default (item 7)
    // -------------------------------------------------------------------------

    [Test]
    public async Task DefaultModeAllScalarsAreNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              int32  a = 1;
              int64  b = 2;
              uint32 c = 3;
              uint64 d = 4;
              bool   e = 5;
              float  f = 6;
              double g = 7;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("int? A");
        await Assert.That(code).Contains("long? B");
        await Assert.That(code).Contains("uint? C");
        await Assert.That(code).Contains("ulong? D");
        await Assert.That(code).Contains("bool? E");
        await Assert.That(code).Contains("float? F");
        await Assert.That(code).Contains("double? G");
    }

    [Test]
    public async Task DefaultModeStringIsNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { string name = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("string? Name");
    }

    [Test]
    public async Task DefaultModeBytesIsNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { bytes data = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("byte[]? Data");
    }

    [Test]
    public async Task DefaultModeEnumIsNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            enum Color { RED = 0; GREEN = 1; }
            message Msg { Color color = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("Color? Color");
    }

    [Test]
    public async Task DefaultModeMessageRefIsNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Inner { int32 x = 1; }
            message Outer { Inner child = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("Inner? Child");
    }

    // -------------------------------------------------------------------------
    // Strict-optional mode (item 7 – opt-out of nullable-by-default)
    // -------------------------------------------------------------------------

    [Test]
    public async Task StrictOptionalMode_NonOptionalScalarsAreNotNullable()
    {
        var opts = new GeneratorOptions { StrictOptional = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              int32          mandatory = 1;
              optional int32 explicitly_optional = 2;
            }
            """,
            "TestNs",
            opts
        );

        // Non-optional field: plain int
        await Assert.That(code).Contains("public int Mandatory");
        // Explicit optional: int?
        await Assert.That(code).Contains("public int? ExplicitlyOptional");
    }

    [Test]
    public async Task StrictOptionalMode_NonOptionalStringHasEmptyDefault()
    {
        var opts = new GeneratorOptions { StrictOptional = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { string name = 1; }
            """,
            "TestNs",
            opts
        );

        await Assert.That(code).Contains("= string.Empty;");
    }

    [Test]
    public async Task StrictOptionalMode_NonOptionalBytesHasEmptyDefault()
    {
        var opts = new GeneratorOptions { StrictOptional = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { bytes data = 1; }
            """,
            "TestNs",
            opts
        );

        await Assert.That(code).Contains("= global::System.Array.Empty<byte>();");
    }

    // -------------------------------------------------------------------------
    // Repeated and map fields (unaffected by nullable mode)
    // -------------------------------------------------------------------------

    [Test]
    public async Task RepeatedFieldIsListNotNullable()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { repeated string tags = 1; repeated int32 ids = 2; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("List<string>");
        await Assert.That(code).Contains("List<int>");
        // Lists should not be nullable
        await Assert.That(code).DoesNotContain("List<string>?");
        await Assert.That(code).DoesNotContain("List<int>?");
    }

    [Test]
    public async Task MapFieldIsDictionaryWithProtoMap()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { map<string, int32> counts = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("[global::LightProto.ProtoMap]");
        await Assert.That(code).Contains("Dictionary<string, int>");
        await Assert.That(code).DoesNotContain("Dictionary<string, int>?");
    }

    [Test]
    public async Task MapFieldWithDataFormats()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { map<sfixed32, sint64> fancy = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("KeyFormat = global::LightProto.DataFormat.FixedSize");
        await Assert.That(code).Contains("ValueFormat = global::LightProto.DataFormat.ZigZag");
        await Assert.That(code).Contains("Dictionary<int, long>");
    }

    // -------------------------------------------------------------------------
    // DataFormat attributes
    // -------------------------------------------------------------------------

    [Test]
    public async Task ZigZagDataFormatForSint()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { sint32 a = 1; sint64 b = 2; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("DataFormat = global::LightProto.DataFormat.ZigZag");
    }

    [Test]
    public async Task FixedSizeDataFormatForFixed()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { fixed32 a = 1; fixed64 b = 2; sfixed32 c = 3; sfixed64 d = 4; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("DataFormat = global::LightProto.DataFormat.FixedSize");
    }

    // -------------------------------------------------------------------------
    // Enum generation
    // -------------------------------------------------------------------------

    [Test]
    public async Task GeneratesTopLevelEnum()
    {
        var code = Generate(
            """
            syntax = "proto3";
            enum Status { UNKNOWN = 0; ACTIVE = 1; DELETED = 2; }
            message Msg { Status state = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public enum Status");
        await Assert.That(code).Contains("Unknown = 0,");
        await Assert.That(code).Contains("Active = 1,");
        await Assert.That(code).Contains("Deleted = 2,");
    }

    // -------------------------------------------------------------------------
    // Nested messages
    // -------------------------------------------------------------------------

    [Test]
    public async Task GeneratesNestedMessage()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Outer {
              message Inner { int32 value = 1; }
              Inner child = 1;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public partial class Outer");
        await Assert.That(code).Contains("public partial class Inner");
        await Assert.That(code).Contains("[global::LightProto.ProtoMember(1)]");
    }

    // -------------------------------------------------------------------------
    // Namespace handling
    // -------------------------------------------------------------------------

    [Test]
    public async Task UsesProtoFileNamespaceOption()
    {
        var code = Generate(
            """
            syntax = "proto3";
            option csharp_namespace = "My.App.Models";
            message Msg { string name = 1; }
            """
        );

        await Assert.That(code).Contains("namespace My.App.Models");
    }

    [Test]
    public async Task OverrideNamespaceTakesPrecedence()
    {
        var code = Generate(
            """
            syntax = "proto3";
            option csharp_namespace = "OriginalNs";
            message Msg { string name = 1; }
            """,
            "OverriddenNs"
        );

        await Assert.That(code).Contains("namespace OverriddenNs");
        await Assert.That(code).DoesNotContain("OriginalNs");
    }

    // -------------------------------------------------------------------------
    // struct / record / record struct (items 5 & 6)
    // -------------------------------------------------------------------------

    [Test]
    public async Task UseStructOptionGeneratesPartialStruct()
    {
        var opts = new GeneratorOptions { UseStruct = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { int32 x = 1; }
            """,
            "TestNs",
            opts
        );

        await Assert.That(code).Contains("public partial struct Msg");
    }

    [Test]
    public async Task UseRecordOptionGeneratesPartialRecord()
    {
        var opts = new GeneratorOptions { UseRecord = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { int32 x = 1; }
            """,
            "TestNs",
            opts
        );

        await Assert.That(code).Contains("public partial record Msg");
        await Assert.That(code).DoesNotContain("record struct");
    }

    [Test]
    public async Task UseRecordAndStructOptionGeneratesPartialRecordStruct()
    {
        var opts = new GeneratorOptions { UseRecord = true, UseStruct = true };
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { int32 x = 1; }
            """,
            "TestNs",
            opts
        );

        await Assert.That(code).Contains("public partial record struct Msg");
    }

    [Test]
    public async Task DefaultOptionGeneratesPartialClass()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { int32 x = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public partial class Msg");
    }

    // -------------------------------------------------------------------------
    // oneof → [ProtoInclude] (item 4)
    // -------------------------------------------------------------------------

    [Test]
    public async Task OneofWithAllMessageFieldsGeneratesProtoInclude()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Dog  { string breed = 1; }
            message Cat  { bool indoor = 1; }
            message Animal {
              oneof kind {
                Dog dog = 1;
                Cat cat = 2;
              }
            }
            """,
            "TestNs"
        );

        // [ProtoInclude] attributes should appear on Animal
        await Assert.That(code).Contains("[global::LightProto.ProtoInclude(1, typeof(Dog))]");
        await Assert.That(code).Contains("[global::LightProto.ProtoInclude(2, typeof(Cat))]");

        // The oneof fields themselves should NOT be emitted as properties
        await Assert.That(code).DoesNotContain("public Dog? Dog");
        await Assert.That(code).DoesNotContain("public Cat? Cat");
    }

    [Test]
    public async Task OneofWithMixedTypesEmitsNullableProperties()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              oneof mixed {
                string text = 1;
                int32  num  = 2;
              }
            }
            """,
            "TestNs"
        );

        // No ProtoInclude for non-message fields
        await Assert.That(code).DoesNotContain("[global::LightProto.ProtoInclude");

        // Fields are emitted as nullable properties
        await Assert.That(code).Contains("string? Text");
        await Assert.That(code).Contains("int? Num");
    }

    [Test]
    public async Task FieldsOutsideOneofAreNotAffectedByOneofPromotion()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Dog  { string breed = 1; }
            message Cat  { bool indoor = 1; }
            message Animal {
              string name = 3;
              oneof kind { Dog dog = 1; Cat cat = 2; }
            }
            """,
            "TestNs"
        );

        // name field must still be generated
        await Assert.That(code).Contains("[global::LightProto.ProtoMember(3)]");
        await Assert.That(code).Contains("string? Name");
    }

    // -------------------------------------------------------------------------
    // CSharpier-formatted comparisons (item 3)
    // -------------------------------------------------------------------------

    [Test]
    public async Task FormattedOutputMatchesExpectedForSimpleMessage()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Person { int32 id = 1; string name = 2; }
            """,
            "MyNs",
            new GeneratorOptions { StrictOptional = true }
        );

        // The expected snippet (whitespace-normalized via CSharpier)
        const string expected = """
            [global::LightProto.ProtoContract]
            public partial class Person
            {
                [global::LightProto.ProtoMember(1)]
                public int Id { get; set; }

                [global::LightProto.ProtoMember(2)]
                public string Name { get; set; } = string.Empty;
            }
            """;

        await AssertContainsFormatted(code, expected);
    }

    [Test]
    public async Task FormattedOutputMatchesExpectedForNullableDefaultMessage()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Pt { int32 x = 1; int32 y = 2; }
            """,
            "MyNs"
        );

        const string expected = """
            [global::LightProto.ProtoContract]
            public partial class Pt
            {
                [global::LightProto.ProtoMember(1)]
                public int? X { get; set; }

                [global::LightProto.ProtoMember(2)]
                public int? Y { get; set; }
            }
            """;

        await AssertContainsFormatted(code, expected);
    }

    // -------------------------------------------------------------------------
    // Name conversion
    // -------------------------------------------------------------------------

    [Test]
    public async Task ToPascalCase_ConvertsSnakeCase()
    {
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("my_field_name")).IsEqualTo("MyFieldName");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("order_id")).IsEqualTo("OrderId");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("simple")).IsEqualTo("Simple");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("ALL_CAPS")).IsEqualTo("AllCaps");
    }
}
