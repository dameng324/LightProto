# LightProto version comparison

Run the same Serialize and Deserialize methods against the published LightProto
**1.4.0** package and the current **ProjectReference**:

```sh
dotnet run -c Release -f net10.0 --project tests/Benchmark -- --filter '*VersionComparison*' --exporters github --artifacts ./BenchmarkVersionResults
```

`WithArgumentJob` uses `Job.WithArguments([new MsBuildArgument(...)])` to switch
`LightProtoBenchmarkVersion` between `1.4.0` and `ProjectReference`. The package
job uses the analyzer shipped in that package; the project job uses the current
runtime and generator projects. Setup checks the loaded assembly version.

Both jobs use .NET 10, three warmup iterations and ten measurement iterations.
LightProto 1.4.0 is the baseline, so a Ratio below 1 means the current version is
faster. MemoryDiagnoser also reports allocations for each version.

The Database case uses the existing nested-message `test.bin` fixture. PackedLists
uses 1,024 items per field, covering packed varint integers, packed fixed32 bytes,
packed booleans, and unpacked strings. Its input is generated with protobuf-net
before measurement, then validated after LightProto deserialization.

Serialize measures writing to a new MemoryStream; Deserialize measures reading
the same byte-array input. Fixture creation and validation are outside the timed
methods. The existing Linux CI benchmark step includes this benchmark and publishes
the GitHub Markdown report with the other benchmark results.
The benchmark runner exits with an error if validation or any job fails, so CI
cannot silently publish an empty or failed version comparison.

This comparison measures the total change since 1.4.0, including the optimizations
already merged into main, as well as the collection changes in this PR.
