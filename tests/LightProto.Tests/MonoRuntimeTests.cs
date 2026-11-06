#if MONO_TESTS
using System.Runtime.Versioning;

namespace LightProto.Tests;

public class MonoRuntimeTests
{
    [Test]
    public void TestsRunOnMonoWithNetStandardLibrary()
    {
        if (Type.GetType("Mono.Runtime") is null)
        {
            throw new InvalidOperationException("Mono CI tests must execute on the Mono runtime.");
        }

        var framework = (TargetFrameworkAttribute?)
            Attribute.GetCustomAttribute(typeof(Serializer).Assembly, typeof(TargetFrameworkAttribute));
        if (framework?.FrameworkName != ".NETStandard,Version=v2.0")
        {
            throw new InvalidOperationException($"Expected the netstandard2.0 library, got {framework?.FrameworkName}.");
        }
    }
}
#endif
