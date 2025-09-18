namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ImplicitFieldsAllFieldsTests
    : BaseEquivalentTypeTests<
        ImplicitFieldsAllFieldsTests.LightProtoMessage,
        ImplicitFieldsAllFieldsTests.ProtoBufMessage
    >
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public partial record LightProtoMessage
    {
        public int Property1 { get; set; }

        [LightProto.ProtoMember(10)]
        public int Property2;
        public int Property3 { get; set; }
        internal int Property4 { get; set; }
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    public partial record ProtoBufMessage
    {
        public int Property1 { get; set; }

        [ProtoBuf.ProtoMember(10)]
        public int Property2;
        public int Property3 { get; set; }
        internal int Property4 { get; set; }
    }

    public override IEnumerable<LightProtoMessage> GetLightProtoMessages()
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

    public override IEnumerable<ProtoBufMessage> GetProtoNetMessages()
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

    public override async Task AssertResult(
        LightProtoMessage lightProto,
        ProtoBufMessage protobuf,
        bool lightProtoToProtoBuf
    )
    {
        await Assert.That(lightProto.Property1).IsEqualTo(protobuf.Property1);
        await Assert.That(lightProto.Property2).IsEqualTo(protobuf.Property2);
        await Assert.That(lightProto.Property3).IsEqualTo(protobuf.Property3);
        await Assert.That(lightProto.Property4).IsEqualTo(protobuf.Property4);
    }
}
