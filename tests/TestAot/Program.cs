using LightProto;
using MemoryPack;

// var ms = new MemoryStream();
// LightProto.Serializer.SerializeDynamically(ms, new TestMessage() { Id = 12345 });
// Console.WriteLine(Convert.ToBase64String(ms.ToArray()));

var bytes = Convert.FromBase64String("CLlg");
var message = Serializer.Deserialize<TestMessage>(bytes);
Console.WriteLine(message.Id);

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[MemoryPackable]
public partial class TestMessage
{
    [ProtoMember(1)]
    public int Id { get; set; }
}

public partial class TestMessage
{
    [ProtoMember(2)]
    public int Message { get; set; }
}

public partial class TestMessage
{
    [ProtoMember(3)]
    public int Message2 { get; set; }
}
