using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Dameng.Protobuf.Tests;

public class SkipAotAttribute : SkipAttribute
{
    public SkipAotAttribute() : base("Do not exec this test in NativeAOT environment")
    {
    }

    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        if (RuntimeFeature.IsDynamicCodeSupported) // It is false when nativeAOT
        {
            return Task.FromResult(false);
        }
        else
        {
            return Task.FromResult(true);
        }
    }
}