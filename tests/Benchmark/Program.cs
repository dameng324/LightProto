using Benchmark;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using LightProto;

var ms = new MemoryStream();
ProtoBuf.Serializer.Serialize<Message>(ms, new Message() { Property = [1, 2, null, 4] });
var bytes = ms.ToArray();
Console.WriteLine(string.Join(",", bytes));

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
return;

[ProtoBuf.ProtoContract]
public partial record Message
{
    [ProtoBuf.ProtoMember(1)]
    public List<int?> Property { get; set; }
}


// BenchmarkRunner.Run<SerializeAot>();
// return;
//
// BenchmarkRunner.Run<Deserialize>();
// return;

// var serialize = new SerializeAot();
// for (int i = 0; i < 1; i++)
// {
//     serialize.Serialize_LightProto();
// }
// Console.WriteLine("done.");
// return;

// byte[] _data = System.IO.File.ReadAllBytes("test.bin");
// //warn up
// for (int i = 0; i < 100; i++)
// {
//     var d2 = GoogleProtobuf.Database.Parser.ParseFrom(_data);
//     var d3 = LightProto.Serializer.Deserialize<LightProto.Database>(_data);
// }
//
// {
//     var d2 = GoogleProtobuf.Database.Parser.ParseFrom(_data);
//     var d3 = LightProto.Serializer.Deserialize<LightProto.Database>(_data);
// }
