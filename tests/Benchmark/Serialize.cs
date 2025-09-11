using BenchmarkDotNet.Attributes;
using Google.Protobuf;

namespace Benchmark;

[MemoryDiagnoser(false)]
public class Serialize
{
    private readonly Protobuf_net.Database protobuf_net;
    private readonly GoogleProtobuf.Database google;
    private readonly DamengProtobuf.Database dameng;

    public Serialize()
    {
        var _data = System.IO.File.ReadAllBytes("test.bin");
        protobuf_net= ProtoBuf.Serializer.Deserialize<Protobuf_net.Database>(_data.AsSpan());
        google= GoogleProtobuf.Database.Parser.ParseFrom(_data);
        dameng= Dameng.Protobuf.Serializer.Deserialize<DamengProtobuf.Database>(_data);
    }

    [Benchmark]
    public void ProtoBuf_net()
    {
        using var ms = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize<Protobuf_net.Database>(ms, protobuf_net);
    }
    [Benchmark()]
    public void GoogleProtoBuf()
    {
        using var ms = new System.IO.MemoryStream();
        google.WriteTo(ms);
    }
    [Benchmark(Baseline = true)]
    public void DamengProtoBuf()
    {
        using var ms = new System.IO.MemoryStream();
        Dameng.Protobuf.Serializer.Serialize<DamengProtobuf.Database>(ms, dameng);
    }
}