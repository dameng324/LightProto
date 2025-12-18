using LightProto;

// var ms = new MemoryStream();
// LightProto.Serializer.SerializeDynamically(ms, new TestMessage() { Id = 12345 });
// Console.WriteLine(Convert.ToBase64String(ms.ToArray()));

var bytes = Convert.FromBase64String("CLlg");
var message = Serializer.Deserialize<TestMessage>(bytes);
Console.WriteLine(message.Id);

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public partial class TestMessage
{
    public int Id { get; set; }
}
