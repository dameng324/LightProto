using BenchmarkDotNet.Attributes;

namespace Benchmark;

[MemoryDiagnoser(false)]
public class Deserialize
{
    private readonly byte[] _data;

    public Deserialize()
    {
        _data = System.IO.File.ReadAllBytes("test.bin");
    }

    [Benchmark]
    public Protobuf_net.Database Deserialize_ProtoBuf_net()
    {
        return ProtoBuf.Serializer.Deserialize<Protobuf_net.Database>(_data.AsSpan());
    }
    [Benchmark()]
    public GoogleProtobuf.Database Deserialize_GoogleProtoBuf()
    {
        return GoogleProtobuf.Database.Parser.ParseFrom(_data);
    }
    [Benchmark(Baseline = true)]
    public DamengProtobuf.Database Deserialize_DamengProtoBuf()
    {
        return Dameng.Protobuf.Serializer.Deserialize<DamengProtobuf.Database>(_data);
    }
}