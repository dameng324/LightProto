using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmark;

[MemoryDiagnoser(false)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net80)]
public class Deserialize
{
    private readonly byte[] _data;

    public Deserialize()
    {
        _data = System.IO.File.ReadAllBytes("test.bin");
    }

    [Benchmark]
    public ProtoBuf.Database Deserialize_ProtoBuf_net()
    {
        return ProtoBuf.Serializer.Deserialize<ProtoBuf.Database>(_data.AsSpan());
    }

    [Benchmark()]
    public GoogleProtobuf.Database Deserialize_GoogleProtoBuf()
    {
        return GoogleProtobuf.Database.Parser.ParseFrom(_data);
    }

    [Benchmark(Baseline = true)]
    public LightProto.Database Deserialize_LightProto()
    {
        return LightProto.Serializer.Deserialize<LightProto.Database>(_data);
    }
}

// [MemoryDiagnoser(false)]
// [SimpleJob(RuntimeMoniker.NativeAot10_0)]
// [SimpleJob(RuntimeMoniker.NativeAot90)]
// public class DeserializeAot
// {
//     private readonly byte[] _data;
//
//     public DeserializeAot()
//     {
//         _data = System.IO.File.ReadAllBytes("test.bin");
//     }
//
//     [Benchmark(Baseline = true)]
//     public LightProto.Database Deserialize_LightProto()
//     {
//         return LightProto.Serializer.Deserialize<LightProto.Database>(_data);
//     }
// }
