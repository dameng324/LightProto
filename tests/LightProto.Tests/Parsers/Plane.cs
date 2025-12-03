#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class PlaneTests : BaseTests<PlaneTests.Message, PlaneTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Plane Property { get; set; }
    }

    public override IEnumerable<PlaneTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new PlaneTestsMessage()
            {
                Property = new PlaneMessage()
                {
                    M =
                    {
                        x.Property.Normal.X,
                        x.Property.Normal.Y,
                        x.Property.Normal.Z,
                        x.Property.D,
                    },
                },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Plane(-1, -2, 3, 4) };
        yield return new() { Property = new Plane(1, -2, 3, 4) };
        yield return new() { Property = new Plane(-1, 2, 3, 4) };
        yield return new() { Property = new Plane(1, 2, -3, 4) };
        yield return new() { Property = new Plane(0, 0, 0, 4) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Normal).IsEqualTo(message.Property.Normal);
        await Assert.That(clone.Property.D).IsEqualTo(message.Property.D);
    }

    public override async Task AssertGoogleResult(PlaneTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.M.Count).IsEqualTo(4);
        await Assert.That(clone.Property.M[0]).IsEqualTo(message.Property.Normal.X);
        await Assert.That(clone.Property.M[1]).IsEqualTo(message.Property.Normal.Y);
        await Assert.That(clone.Property.M[2]).IsEqualTo(message.Property.Normal.Z);
        await Assert.That(clone.Property.M[3]).IsEqualTo(message.Property.D);
    }

    [Test]
    public async Task NullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new PlaneProtoParser() { Floats = null };

        Plane result = protoParser;
        await Assert.That(result).IsEqualTo(default(Plane));
    }

    [Test]
    public async Task FloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new PlaneProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Plane result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert
            .That(exception!.Message)
            .Contains("Input array must contain 4 elements for Plane conversion.");
    }
}
#endif
