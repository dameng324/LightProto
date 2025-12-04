#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class QuaternionTests : BaseTests<QuaternionTests.Message, QuaternionTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Quaternion Property { get; set; }
    }

    public override IEnumerable<QuaternionTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new QuaternionTestsMessage()
            {
                Property = new QuaternionMessage()
                {
                    M = { x.Property.X, x.Property.Y, x.Property.Z, x.Property.W },
                },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Quaternion(-1, -2, 3, 4) };
        yield return new() { Property = new Quaternion(1, -2, 3, 4) };
        yield return new() { Property = new Quaternion(-1, 2, 3, 4) };
        yield return new() { Property = new Quaternion(1, 2, -3, 4) };
        yield return new() { Property = new Quaternion(0, 0, 0, 4) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.X).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.Y).IsEqualTo(message.Property.Y);
        await Assert.That(clone.Property.Z).IsEqualTo(message.Property.Z);
        await Assert.That(clone.Property.W).IsEqualTo(message.Property.W);
    }

    public override async Task AssertGoogleResult(QuaternionTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.M.Count).IsEqualTo(4);
        await Assert.That(clone.Property.M[0]).IsEqualTo(message.Property.X);
        await Assert.That(clone.Property.M[1]).IsEqualTo(message.Property.Y);
        await Assert.That(clone.Property.M[2]).IsEqualTo(message.Property.Z);
        await Assert.That(clone.Property.M[3]).IsEqualTo(message.Property.W);
    }

    [Test]
    public async Task NullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new QuaternionProtoParser() { Floats = null };

        Quaternion result = protoParser;
        await Assert.That(result).IsEqualTo(default(Quaternion));
    }

    [Test]
    public async Task FloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new QuaternionProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Quaternion result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert
            .That(exception!.Message)
            .Contains("Input array must contain 4 elements for Quaternion conversion.");
    }
}
#endif
