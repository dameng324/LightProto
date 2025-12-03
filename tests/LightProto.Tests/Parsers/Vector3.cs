#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Vector3Tests : BaseTests<Vector3Tests.Message, Vector3TestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Vector3 Property { get; set; }
    }

    public override IEnumerable<Vector3TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new Vector3TestsMessage()
            {
                Property = new Vector3Message()
                {
                    X = x.Property.X,
                    Y = x.Property.Y,
                    Z = x.Property.Z,
                },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Vector3(-1, -2, 3) };
        yield return new() { Property = new Vector3(1, -2, 3) };
        yield return new() { Property = new Vector3(-1, 2, 3) };
        yield return new() { Property = new Vector3(1, 2, -3) };
        yield return new() { Property = new Vector3(0, 0, 0) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.X).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.Y).IsEqualTo(message.Property.Y);
        await Assert.That(clone.Property.Z).IsEqualTo(message.Property.Z);
    }

    public override async Task AssertGoogleResult(Vector3TestsMessage clone, Message message)
    {
        clone.Property ??= new Vector3Message();
        await Assert.That(clone.Property.X).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.Y).IsEqualTo(message.Property.Y);
        await Assert.That(clone.Property.Z).IsEqualTo(message.Property.Z);
    }
}
#endif
