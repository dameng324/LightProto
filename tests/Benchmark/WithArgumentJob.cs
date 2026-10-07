using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

public class WithArgumentJob : ManualConfig
{
    public WithArgumentJob()
    {
        var job = Job.Default.WithRuntime(CoreRuntime.Core10_0).WithWarmupCount(3).WithIterationCount(10);

        AddJob(
            job.WithId("LightProto-1.4.0")
                .WithArguments([new MsBuildArgument("/p:LightProtoBenchmarkVersion=1.4.0")])
                .WithEnvironmentVariable("LIGHTPROTO_BENCHMARK_REFERENCE", "1.4.0")
                .AsBaseline()
        );
        AddJob(
            job.WithId("ProjectReference")
                .WithArguments([new MsBuildArgument("/p:LightProtoBenchmarkVersion=ProjectReference")])
                .WithEnvironmentVariable("LIGHTPROTO_BENCHMARK_REFERENCE", "ProjectReference")
        );
    }
}
