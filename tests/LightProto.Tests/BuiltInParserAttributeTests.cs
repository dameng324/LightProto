using System.Reflection;

namespace LightProto.Tests;

public class BuiltInParserAttributeTests
{
    // Generic attributes need C# 11 runtime support. The netstandard2.0 build is loaded by runtimes
    // without it (Mono, Unity), which abort when they read one, so the built-in parsers must not carry any.
    [Test]
    [SkipAot]
    public async Task BuiltInParsers_HaveNoGenericAttributes()
    {
        var offenders = typeof(Serializer)
            .Assembly.GetTypes()
            .Where(t => t.Namespace == "LightProto.Parser")
            .SelectMany(t => t.GetCustomAttributesData().Select(a => (Type: t, Attribute: a.AttributeType)))
            .Where(x => x.Attribute.IsGenericType)
            .Select(x => $"{x.Type.Name}: {x.Attribute.Name}")
            .ToList();

        await Assert.That(offenders).IsEmpty();
    }
}
