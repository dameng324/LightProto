using CSharpier.Core.CSharp;
using Google.Protobuf.Reflection;
using LightProto.ProtoGen;
using ProtoBuf.Reflection;

namespace LightProto.ProtoGen.Tests;

public class CaseStyleRuleTests
{
    private static string Generate(string protoSchema, GeneratorOptions? opts = null)
    {
        var set = new FileDescriptorSet();
        using (var reader = new StringReader(protoSchema))
        {
            set.Add("test.proto", source: reader);
        }
        set.Process();
        var errors = set.GetErrors();
        if (errors.Length > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors.Select(e => e.ToString())));

        return new LightProtoCSharpGenerator(opts).Generate(set.Files.First(), "TestNs");
    }

    private static async Task AssertCodeEqual(string generatedCode, string expectedCode)
    {
        var fmtGenerated = (await CSharpFormatter.FormatAsync(generatedCode, new CSharpier.Core.CodeFormatterOptions())).Code?.TrimEnd();
        var fmtExpected = (await CSharpFormatter.FormatAsync(expectedCode, new CSharpier.Core.CodeFormatterOptions())).Code?.TrimEnd();
        await Assert.That(fmtGenerated).IsEqualTo(fmtExpected);
    }

    [Test]
    public async Task DoubleStar_CanMatchZeroSegments()
    {
        var resolver = new CaseStyleResolver(CaseStyle.Pascal, [new CaseStyleRule("pkg.**.FieldA", CaseStyle.Camel)]);
        await Assert.That(resolver.Resolve("pkg.FieldA")).IsEqualTo(CaseStyle.Camel);
    }

    [Test]
    public async Task DoubleStar_CanMatchAcrossMultipleSegments()
    {
        var resolver = new CaseStyleResolver(CaseStyle.Pascal, [new CaseStyleRule("pkg.**.FieldA", CaseStyle.Camel)]);
        await Assert.That(resolver.Resolve("pkg.A.B.C.FieldA")).IsEqualTo(CaseStyle.Camel);
    }

    [Test]
    public async Task SingleStar_MatchesExactlyOneSegment()
    {
        var resolver = new CaseStyleResolver(CaseStyle.Pascal, [new CaseStyleRule("pkg.*.FieldA", CaseStyle.Camel)]);
        await Assert.That(resolver.Resolve("pkg.A.FieldA")).IsEqualTo(CaseStyle.Camel);
        await Assert.That(resolver.Resolve("pkg.A.B.FieldA")).IsEqualTo(CaseStyle.Pascal);
    }

    [Test]
    public async Task SegmentGlob_SupportsStarWithinSegment()
    {
        var resolver = new CaseStyleResolver(CaseStyle.Pascal, [new CaseStyleRule("package*.*", CaseStyle.Preserve)]);
        await Assert.That(resolver.Resolve("packageA.Message")).IsEqualTo(CaseStyle.Preserve);
        await Assert.That(resolver.Resolve("packageB.Message")).IsEqualTo(CaseStyle.Preserve);
        await Assert.That(resolver.Resolve("pkg.Message")).IsEqualTo(CaseStyle.Pascal);
    }

    [Test]
    public async Task Specificity_PrefersMoreSpecificRule()
    {
        var resolver = new CaseStyleResolver(
            CaseStyle.Pascal,
            [
                new CaseStyleRule("pkg.**", CaseStyle.Camel),
                new CaseStyleRule("pkg.*.FieldA", CaseStyle.Preserve),
                new CaseStyleRule("pkg.Message.*", CaseStyle.Pascal),
            ]
        );

        await Assert.That(resolver.Resolve("pkg.Message.FieldA")).IsEqualTo(CaseStyle.Pascal);
    }

    [Test]
    public async Task Specificity_PrefersSegmentGlobOverSingleStar()
    {
        var resolver = new CaseStyleResolver(
            CaseStyle.Pascal,
            [new CaseStyleRule("pkg.*.FieldA", CaseStyle.Camel), new CaseStyleRule("pkg.pack*.FieldA", CaseStyle.Preserve)]
        );

        await Assert.That(resolver.Resolve("pkg.packageA.FieldA")).IsEqualTo(CaseStyle.Preserve);
    }

    [Test]
    public async Task SameSpecificity_LaterRuleOverridesEarlierRule()
    {
        var resolver = new CaseStyleResolver(
            CaseStyle.Pascal,
            [new CaseStyleRule("pkg.Message.FieldA", CaseStyle.Camel), new CaseStyleRule("pkg.Message.FieldA", CaseStyle.Preserve)]
        );

        await Assert.That(resolver.Resolve("pkg.Message.FieldA")).IsEqualTo(CaseStyle.Preserve);
    }

    [Test]
    public async Task NoRule_UsesDefaultStyle()
    {
        var resolver = new CaseStyleResolver(CaseStyle.Camel, []);
        await Assert.That(resolver.Resolve("pkg.Message.FieldA")).IsEqualTo(CaseStyle.Camel);
    }

    [Test]
    public async Task InvalidDoubleStarSegment_ThrowsFriendlyError()
    {
        await Assert
            .That(() => CaseStyleRuleParser.ParseAssignment("pkg.ab**cd=Pascal"))
            .Throws<ArgumentException>()
            .WithMessage("Invalid pattern 'pkg.ab**cd': '**' must occupy an entire segment (for example 'pkg.**.Type').");
    }

    [Test]
    public async Task Explain_ReturnsWinnerAndCandidateList()
    {
        var resolver = new CaseStyleResolver(
            CaseStyle.Pascal,
            [new CaseStyleRule("pkg.**", CaseStyle.Camel), new CaseStyleRule("pkg.Message.*", CaseStyle.Preserve)]
        );

        var explanation = resolver.Explain("pkg.Message.FieldA");
        await Assert.That(explanation.SelectedPattern).IsEqualTo("pkg.Message.*");
        await Assert.That(explanation.Candidates.Count).IsEqualTo(2);
        await Assert.That(explanation.Candidates.Count(c => c.IsWinner)).IsEqualTo(1);
    }

    [Test]
    public async Task Generator_AppliesCaseRulesByProtoFullName()
    {
        var opts = new GeneratorOptions { DefaultCaseStyle = CaseStyle.Pascal };
        opts.CaseStyleRules.Add(new CaseStyleRule("market.exchange.ExchangeType.SSE", CaseStyle.Preserve));
        opts.CaseStyleRules.Add(new CaseStyleRule("market.exchange.Trade.exchange_type", CaseStyle.Camel));

        var code = Generate(
            """
            syntax = "proto3";
            package market.exchange;
            enum ExchangeType {
              EXCHANGE_UNKNOWN = 0;
              SSE = 1;
            }
            message Trade {
              ExchangeType exchange_type = 1;
            }
            """,
            opts
        );

        const string expected = """
            // <auto-generated/>
            // This file was generated by lightproto-gen. Do not edit manually.
            #nullable enable

            namespace TestNs
            {
                public enum ExchangeType
                {
                    ExchangeUnknown = 0,
                    SSE = 1,
                }

                [global::LightProto.ProtoContract]
                public partial class Trade
                {
                    [global::LightProto.ProtoMember(1)]
                    public ExchangeType? exchangeType { get; set; }
                }
            }
            """;

        await AssertCodeEqual(code, expected);
    }

    [Test]
    public async Task Generator_EscapesCSharpKeywordIdentifiers()
    {
        var opts = new GeneratorOptions { DefaultCaseStyle = CaseStyle.Preserve };
        opts.CaseStyleRules.Add(new CaseStyleRule("demo.Trade.event", CaseStyle.Camel));

        var code = Generate(
            """
            syntax = "proto3";
            package demo;
            message Trade {
              int32 event = 1;
            }
            """,
            opts
        );

        const string expected = """
            // <auto-generated/>
            // This file was generated by lightproto-gen. Do not edit manually.
            #nullable enable

            namespace TestNs
            {
                [global::LightProto.ProtoContract]
                public partial class Trade
                {
                    [global::LightProto.ProtoMember(1)]
                    public int? @event { get; set; }
                }
            }
            """;

        await AssertCodeEqual(code, expected);
    }

    [Test]
    public async Task Generator_UsesPackageQualifiedTypeRuleForReferencedType()
    {
        var opts = new GeneratorOptions { DefaultCaseStyle = CaseStyle.Pascal };
        opts.CaseStyleRules.Add(new CaseStyleRule("market.exchange.ExchangeType", CaseStyle.Camel));

        var code = Generate(
            """
            syntax = "proto3";
            package market.exchange;
            enum ExchangeType {
              UNKNOWN = 0;
            }
            message Trade {
              ExchangeType exchange_type = 1;
            }
            """,
            opts
        );

        const string expected = """
            // <auto-generated/>
            // This file was generated by lightproto-gen. Do not edit manually.
            #nullable enable

            namespace TestNs
            {
                public enum exchangeType
                {
                    Unknown = 0,
                }

                [global::LightProto.ProtoContract]
                public partial class Trade
                {
                    [global::LightProto.ProtoMember(1)]
                    public exchangeType? ExchangeType { get; set; }
                }
            }
            """;

        await AssertCodeEqual(code, expected);
    }
}
