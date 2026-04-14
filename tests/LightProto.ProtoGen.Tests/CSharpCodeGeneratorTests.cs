using Google.Protobuf.Reflection;
using LightProto.ProtoGen;
using ProtoBuf.Reflection;

namespace LightProto.ProtoGen.Tests;

public class LightProtoCSharpGeneratorTests
{
    private static string Generate(string protoSchema, string? ns = null)
    {
        var set = new FileDescriptorSet();
        set.Add("test.proto", source: new System.IO.StringReader(protoSchema));
        set.Process();
        var errors = set.GetErrors();
        if (errors.Length > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(e => e.ToString())));

        var file = set.Files.First();
        return new LightProtoCSharpGenerator().Generate(file, ns);
    }

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
    public async Task GeneratesScalarFields()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Scalars {
              int32   i32  = 1;
              int64   i64  = 2;
              uint32  u32  = 3;
              uint64  u64  = 4;
              bool    flag = 5;
              string  text = 6;
              bytes   data = 7;
              float   f32  = 8;
              double  f64  = 9;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public int I32 { get; set; }");
        await Assert.That(code).Contains("public long I64 { get; set; }");
        await Assert.That(code).Contains("public uint U32 { get; set; }");
        await Assert.That(code).Contains("public ulong U64 { get; set; }");
        await Assert.That(code).Contains("public bool Flag { get; set; }");
        await Assert.That(code).Contains("public string Text { get; set; }");
        await Assert.That(code).Contains("public byte[] Data { get; set; }");
        await Assert.That(code).Contains("public float F32 { get; set; }");
        await Assert.That(code).Contains("public double F64 { get; set; }");
    }

    [Test]
    public async Task GeneratesZigZagDataFormatForSint()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              sint32 a = 1;
              sint64 b = 2;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("DataFormat = global::LightProto.DataFormat.ZigZag");
    }

    [Test]
    public async Task GeneratesFixedSizeDataFormatForFixed()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              fixed32  a = 1;
              fixed64  b = 2;
              sfixed32 c = 3;
              sfixed64 d = 4;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("DataFormat = global::LightProto.DataFormat.FixedSize");
    }

    [Test]
    public async Task GeneratesRepeatedFieldAsList()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              repeated string tags = 1;
              repeated int32  ids  = 2;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("List<string>");
        await Assert.That(code).Contains("List<int>");
    }

    [Test]
    public async Task GeneratesMapFieldAsDictionary()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              map<string, int32> counts = 1;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("[global::LightProto.ProtoMap]");
        await Assert.That(code).Contains("Dictionary<string, int>");
    }

    [Test]
    public async Task GeneratesMapFieldWithDataFormats()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              map<sfixed32, sint64> fancy = 1;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("KeyFormat = global::LightProto.DataFormat.FixedSize");
        await Assert.That(code).Contains("ValueFormat = global::LightProto.DataFormat.ZigZag");
        await Assert.That(code).Contains("Dictionary<int, long>");
    }

    [Test]
    public async Task GeneratesTopLevelEnum()
    {
        var code = Generate(
            """
            syntax = "proto3";
            enum Status {
              UNKNOWN = 0;
              ACTIVE  = 1;
              DELETED = 2;
            }
            message Msg { Status state = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public enum Status");
        await Assert.That(code).Contains("Unknown = 0,");
        await Assert.That(code).Contains("Active = 1,");
        await Assert.That(code).Contains("Deleted = 2,");
    }

    [Test]
    public async Task GeneratesNestedMessage()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Outer {
              message Inner {
                int32 value = 1;
              }
              Inner child = 1;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("public partial class Outer");
        await Assert.That(code).Contains("public partial class Inner");
        await Assert.That(code).Contains("[global::LightProto.ProtoMember(1)]");
    }

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

    [Test]
    public async Task GeneratesProtoMemberTagNumbers()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg {
              string first  = 1;
              int32  second = 42;
            }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("[global::LightProto.ProtoMember(1)]");
        await Assert.That(code).Contains("[global::LightProto.ProtoMember(42)]");
    }

    [Test]
    public async Task ToPascalCase_ConvertsSnakeCase()
    {
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("my_field_name")).IsEqualTo("MyFieldName");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("order_id")).IsEqualTo("OrderId");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("simple")).IsEqualTo("Simple");
        await Assert.That(LightProtoCSharpGenerator.ToPascalCase("ALL_CAPS")).IsEqualTo("AllCaps");
    }

    [Test]
    public async Task GeneratesMessageReferenceField()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Address { string city = 1; }
            message Person  { Address addr = 1; }
            """,
            "TestNs"
        );

        // Message-typed fields should be nullable reference types
        await Assert.That(code).Contains("Address? Addr");
    }

    [Test]
    public async Task GeneratesEnumReferenceField()
    {
        var code = Generate(
            """
            syntax = "proto3";
            enum Color { RED = 0; GREEN = 1; BLUE = 2; }
            message Pixel { Color color = 1; }
            """,
            "TestNs"
        );

        // Enum-typed fields are value types - not nullable
        await Assert.That(code).Contains("public Color Color { get; set; }");
    }

    [Test]
    public async Task GeneratesStringDefaultEmptyInitializer()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { string name = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("= string.Empty;");
    }

    [Test]
    public async Task GeneratesBytesDefaultEmptyArrayInitializer()
    {
        var code = Generate(
            """
            syntax = "proto3";
            message Msg { bytes data = 1; }
            """,
            "TestNs"
        );

        await Assert.That(code).Contains("= global::System.Array.Empty<byte>();");
    }
}
