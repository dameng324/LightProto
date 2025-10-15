namespace LightProto.Tests;

public partial class MissingFieldTests
{
    [ProtoContract]
    public partial record MessageV1
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(4)]
        public string Name2 { get; set; } = string.Empty;
    }

    [ProtoContract]
    public partial record MessageV2
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; } = string.Empty;

        [ProtoMember(4)]
        public string Name2 { get; set; } = string.Empty;
    }

    [Test]
    public async Task MissingFieldTest()
    {
        MessageV1 original = new()
        {
            Id = 1,
            Name = "Test",
            Name2 = "Test2",
        };
        var bytes = original.ToByteArray(MessageV1.ProtoWriter);
        var parsed = Serializer.Deserialize<MessageV2>(bytes, MessageV2.ProtoReader);
        await Assert.That(parsed.Id).IsEqualTo(original.Id);
        await Assert.That(parsed.Name).IsEqualTo(string.Empty);
        await Assert.That(parsed.Name2).IsEqualTo(original.Name2);
    }
}
