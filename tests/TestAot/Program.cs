using System.Diagnostics.CodeAnalysis;
using LightProto;
using MemoryPack;

{
    var list = new List<TestMessage>()
    {
        new TestMessage() { Id = 123 },
        new TestMessage() { Id = 456 },
    };
    var bytes = MemoryPackSerializer.Serialize(list);
    var hex = BitConverter.ToString(bytes);
    Console.WriteLine(hex); // "02-00-00-00-01-00-00-00-01-64-00-00-00-02-00-00-00-01-C8-00-00-00"
}

// {
//     var list =new List<string>(){"123","456"};
//     var bytes = MemoryPackSerializer.Serialize(list);
//     var hex = BitConverter.ToString(bytes);
//     Console.WriteLine(hex); // "02-00-00-00-01-00-00-00-01-64-00-00-00-02-00-00-00-01-C8-00-00-00"
// }
// {
//     Dictionary<int, string> map = new() { [1] = "1", [2] = "2" };
//     var bytes = MemoryPackSerializer.Serialize(map);
//     var hex = BitConverter.ToString(bytes);
//     Console.WriteLine(hex); // "02-00-00-00-01-00-00-00-01-64-00-00-00-02-00-00-00-01-C8-00-00-00"
// }
//
// {
//     Dictionary<int, TestMessage> map = new()
//     {
//         [1] = new TestMessage() { Id = 100 },
//         [2] = new TestMessage() { Id = 200 },
//     };
//
//     var bytes = MemoryPackSerializer.Serialize(map);
//     var hex = BitConverter.ToString(bytes);
//     //Console.WriteLine(hex); // "02-00-00-00-01-00-00-00-01-64-00-00-00-02-00-00-00-01-C8-00-00-00"
// }
//
// {
//     var hex = "02-00-00-00-01-00-00-00-01-64-00-00-00-02-00-00-00-01-C8-00-00-00";
//     var bytes = Convert.FromHexString(hex.Replace("-", ""));
//     var deserializedMap = MemoryPackSerializer.Deserialize<Dictionary<int, TestMessage>>(bytes)!;
//     foreach (var kvp in deserializedMap)
//     {
//         Console.WriteLine($"Key: {kvp.Key}, Value.Id: {kvp.Value.Id}");
//     }
// }
//
[MemoryPackable]
public partial class TestMessage
{
    public int Id { get; set; }
}
