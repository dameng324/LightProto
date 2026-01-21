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

    [Test]
    public async Task MissingFieldRuntimeParserTest()
    {
        MessageV1 original = new()
        {
            Id = 1,
            Name = "Test",
            Name2 = "Test2",
        };

        var runtimeParserV2 = new RuntimeProtoParser<MessageV2>(() => new());
        runtimeParserV2.AddMember(1, message => message.Id, (message, value) => message.Id = value);
        runtimeParserV2.AddMember(typeof(string), 3, message => message.Name, (message, value) => message.Name = (string)value);
        runtimeParserV2.AddMember(typeof(string), 4, message => message.Name2, (message, value) => message.Name2 = (string)value);

        var bytes = original.ToByteArray(MessageV1.ProtoWriter);
        var parsed = Serializer.Deserialize(bytes, runtimeParserV2.ProtoReader);
        await Assert.That(parsed.Id).IsEqualTo(original.Id);
        await Assert.That(parsed.Name).IsEqualTo(string.Empty);
        await Assert.That(parsed.Name2).IsEqualTo(original.Name2);
    }
}
