using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

[MemoryDiagnoser(false)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net80)]
public class SerializationSize
{
    private readonly LightProto.Database _database;

    public SerializationSize()
    {
        var data = File.ReadAllBytes("test.bin");
        _database = LightProto.Serializer.Deserialize<LightProto.Database>(data);
    }

    [Benchmark]
    public int CalculateSize() => LightProto.Serializer.CalculateSize(_database);

    [Benchmark]
    public byte[] ToByteArray() => LightProto.Serializer.ToByteArray(_database);
}
