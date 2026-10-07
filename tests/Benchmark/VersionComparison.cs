using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using LightProto;

namespace Benchmark;

[Config(typeof(WithArgumentJob))]
[MemoryDiagnoser(false)]
[CategoriesColumn]
[HideColumns("EnvironmentVariables", "Arguments")]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class VersionComparison
{
    public enum PayloadKind
    {
        Database,
        PackedLists,
    }

    [Params(PayloadKind.Database, PayloadKind.PackedLists)]
    public PayloadKind Payload { get; set; }

    private byte[] _data = null!;
    private Database _database = null!;
    private PackedListsMessage _packedLists = null!;

    [GlobalSetup]
    public void Setup()
    {
        var reference = Environment.GetEnvironmentVariable("LIGHTPROTO_BENCHMARK_REFERENCE");
        var assembly = typeof(Serializer).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        var isVersion140 = version.Split('+')[0] == "1.4.0";
        if ((reference == "1.4.0") != isVersion140)
        {
            throw new InvalidOperationException($"Expected {reference}, but loaded LightProto {version} from {assembly.Location}.");
        }

        Console.WriteLine($"LightProto reference: {reference}; version: {version}; assembly: {assembly.Location}");

        if (Payload == PayloadKind.Database)
        {
            _data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test.bin"));
            _database = Serializer.Deserialize<Database>(_data);
            return;
        }

        var expected = new PackedListsMessage
        {
            Varints = Enumerable.Range(0, 1024).Select(i => i * 31).ToList(),
            FixedBytes = Enumerable.Range(0, 1024).Select(i => (byte)i).ToList(),
            Booleans = Enumerable.Range(0, 1024).Select(i => i % 2 == 0).ToList(),
            Strings = Enumerable.Range(0, 1024).Select(i => $"item-{i}").ToList(),
        };

        // Generate the input independently of either LightProto version.
        using var stream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(stream, expected);
        _data = stream.ToArray();
        _packedLists = Serializer.Deserialize<PackedListsMessage>(_data);
        if (
            !_packedLists.Varints.SequenceEqual(expected.Varints)
            || !_packedLists.FixedBytes.SequenceEqual(expected.FixedBytes)
            || !_packedLists.Booleans.SequenceEqual(expected.Booleans)
            || !_packedLists.Strings.SequenceEqual(expected.Strings)
        )
        {
            throw new InvalidOperationException("The packed List payload did not round-trip correctly.");
        }
    }

    [Benchmark]
    [BenchmarkCategory("Serialize")]
    public long Serialize()
    {
        using var stream = new MemoryStream();
        if (Payload == PayloadKind.Database)
        {
            Serializer.Serialize(stream, _database);
        }
        else
        {
            Serializer.Serialize(stream, _packedLists);
        }

        return stream.Length;
    }

    [Benchmark]
    [BenchmarkCategory("Deserialize")]
    public object Deserialize() =>
        Payload == PayloadKind.Database ? Serializer.Deserialize<Database>(_data) : Serializer.Deserialize<PackedListsMessage>(_data);
}

[ProtoContract]
[ProtoBuf.ProtoContract]
public partial class PackedListsMessage
{
    [ProtoMember(1, IsPacked = true)]
    [ProtoBuf.ProtoMember(1, IsPacked = true)]
    public List<int> Varints { get; set; } = [];

    [ProtoMember(2, IsPacked = true, DataFormat = DataFormat.FixedSize)]
    [ProtoBuf.ProtoMember(2, IsPacked = true, DataFormat = ProtoBuf.DataFormat.FixedSize)]
    public List<byte> FixedBytes { get; set; } = [];

    [ProtoMember(3, IsPacked = true)]
    [ProtoBuf.ProtoMember(3, IsPacked = true)]
    public List<bool> Booleans { get; set; } = [];

    [ProtoMember(4, IsPacked = false)]
    [ProtoBuf.ProtoMember(4, IsPacked = false)]
    public List<string> Strings { get; set; } = [];
}
