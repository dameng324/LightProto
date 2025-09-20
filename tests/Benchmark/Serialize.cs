using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Google.Protobuf;

namespace Benchmark;

[MemoryDiagnoser(false)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net80)]
public class Serialize
{
    private readonly ProtoBuf.Database protobuf_net;
    private readonly GoogleProtobuf.Database google;
    private readonly LightProto.Database dameng;

    public Serialize()
    {
        var _data = System.IO.File.ReadAllBytes("test.bin");
        protobuf_net = ProtoBuf.Serializer.Deserialize<ProtoBuf.Database>(_data.AsSpan());
        google = GoogleProtobuf.Database.Parser.ParseFrom(_data);
        dameng = LightProto.Serializer.Deserialize<LightProto.Database>(_data);
    }

    [Benchmark]
    public void Serialize_ProtoBuf_net()
    {
        using var ms = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize<ProtoBuf.Database>(ms, protobuf_net);
    }

    [Benchmark()]
    public void Serialize_GoogleProtoBuf()
    {
        using var ms = new System.IO.MemoryStream();
        google.WriteTo(ms);
    }

    [Benchmark(Baseline = true)]
    public void Serialize_LightProto()
    {
        using var ms = new System.IO.MemoryStream();
        LightProto.Serializer.Serialize<LightProto.Database>(ms, dameng);
    }
}

// [SimpleJob(RuntimeMoniker.NativeAot10_0)]
// [SimpleJob(RuntimeMoniker.NativeAot90)]
// public class SerializeAot
// {
//     private readonly LightProto.Database dameng;
//
//     public SerializeAot()
//     {
//         var _data = System.IO.File.ReadAllBytes("test.bin");
//         dameng = LightProto.Serializer.Deserialize<LightProto.Database>(_data);
//     }
//     [Benchmark()]
//     public void Serialize_LightProto()
//     {
//         using var ms = new System.IO.MemoryStream();
//         LightProto.Serializer.Serialize<LightProto.Database>(ms, dameng);
//     }
// }
