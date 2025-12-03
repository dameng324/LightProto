#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Vector2Tests : BaseTests<Vector2Tests.Message, Vector2TestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Vector2 Property { get; set; }
    }

    public override IEnumerable<Vector2TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new Vector2TestsMessage()
            {
                Property = new Vector2Message() { X = x.Property.X, Y = x.Property.Y },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Vector2(-1, -2) };
        yield return new() { Property = new Vector2(1, -2) };
        yield return new() { Property = new Vector2(-1, 2) };
        yield return new() { Property = new Vector2(1, 2) };
        yield return new() { Property = new Vector2(0, 0) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.X).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.Y).IsEqualTo(message.Property.Y);
    }

    public override async Task AssertGoogleResult(Vector2TestsMessage clone, Message message)
    {
        clone.Property ??= new Vector2Message();
        await Assert.That(clone.Property.X).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.Y).IsEqualTo(message.Property.Y);
    }
}
#endif
