using Benchmark;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
// BenchmarkRunner.Run<Serialize>();
// return;

//BenchmarkRunner.Run<Deserialize>();

var serialize = new Serialize();
for (int i = 0; i < 5000; i++)
{
    serialize.GoogleProtoBuf();
}
Console.WriteLine("done.");
return;

// byte[] _data = System.IO.File.ReadAllBytes("test.bin");
// //warn up
// for (int i = 0; i < 100; i++)
// {
//     var d2 = GoogleProtobuf.Database.Parser.ParseFrom(_data);
//     var d3 = Dameng.Protobuf.Serializer.Deserialize<DamengProtobuf.Database>(_data);
// }
//
// {
//     var d2 = GoogleProtobuf.Database.Parser.ParseFrom(_data);
//     var d3 = Dameng.Protobuf.Serializer.Deserialize<DamengProtobuf.Database>(_data);
// }