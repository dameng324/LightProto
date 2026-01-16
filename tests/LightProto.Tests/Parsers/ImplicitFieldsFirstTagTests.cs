namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ImplicitFieldsFirstTagTests
    : BaseEquivalentTypeTests<ImplicitFieldsFirstTagTests.LightProtoMessage, ImplicitFieldsFirstTagTests.ProtoBufMessage>
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic, ImplicitFirstTag = 5)]
    public partial record LightProtoMessage
    {
        public int Property1 { get; set; }

        [LightProto.ProtoMember(10)]
        public int Property2;
        public int Property3 { get; set; }
        private int Property4 { get; set; }

        public int GetProperty4() => Property4;

        public static IEnumerable<LightProtoMessage> GetMessages()
        {
            yield return new()
            {
                Property1 = 0,
                Property2 = 0,
                Property3 = 0,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = 1,
                Property2 = 2,
                Property3 = 3,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = -1,
                Property2 = -2,
                Property3 = -3,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = int.MaxValue,
                Property2 = int.MinValue,
                Property3 = int.MaxValue,
                Property4 = 10,
            };
        }
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic, ImplicitFirstTag = 5)]
    public partial class ProtoBufMessage
    {
        public int Property1 { get; set; }

        [ProtoBuf.ProtoMember(10)]
        public int Property2;
        public int Property3 { get; set; }
        private int Property4 { get; set; }

        public int GetProperty4() => Property4;

        public static IEnumerable<ProtoBufMessage> GetMessages()
        {
            yield return new()
            {
                Property1 = 0,
                Property2 = 0,
                Property3 = 0,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = 1,
                Property2 = 2,
                Property3 = 3,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = -1,
                Property2 = -2,
                Property3 = -3,
                Property4 = 4,
            };
            yield return new()
            {
                Property1 = int.MaxValue,
                Property2 = int.MinValue,
                Property3 = int.MaxValue,
                Property4 = 10,
            };
        }
    }

    public override IEnumerable<LightProtoMessage> GetLightProtoMessages()
    {
        return LightProtoMessage.GetMessages();
    }

    public override IEnumerable<ProtoBufMessage> GetProtoNetMessages()
    {
        return ProtoBufMessage.GetMessages();
    }

    public override async Task AssertResult(LightProtoMessage lightProto, ProtoBufMessage protobuf, bool lightProtoToProtoBuf)
    {
        await Assert.That(lightProto.Property1).IsEqualTo(protobuf.Property1);
        await Assert.That(lightProto.Property2).IsEqualTo(protobuf.Property2);
        await Assert.That(lightProto.Property3).IsEqualTo(protobuf.Property3);
        if (lightProtoToProtoBuf)
        {
            await Assert.That(protobuf.GetProperty4()).IsEqualTo(0);
        }
        else
        {
            await Assert.That(lightProto.GetProperty4()).IsEqualTo(0);
        }
    }
}
