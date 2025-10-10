﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LightProto.Tests;

public class SkipAotAttribute : SkipAttribute
{
    public SkipAotAttribute()
        : base("Do not exec this test in NativeAOT environment") { }

    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
#if NET8_0_OR_GREATER
        if (RuntimeFeature.IsDynamicCodeSupported) // It is false when nativeAOT
        {
            return Task.FromResult(false);
        }
        else
        {
            return Task.FromResult(true);
        }
#else
        return Task.FromResult(false);
#endif
    }
}
