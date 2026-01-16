#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Matrix3x2Tests : BaseTests<Matrix3x2Tests.Message, Matrix3x2TestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Matrix3x2 Property { get; set; }
    }

    public override IEnumerable<Matrix3x2TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new Matrix3x2TestsMessage()
            {
                Property = new Matrix3x2Message()
                {
                    M = { x.Property.M11, x.Property.M12, x.Property.M21, x.Property.M22, x.Property.M31, x.Property.M32 },
                },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Matrix3x2(-1, -2, 3, 4, 5, 6) };
        yield return new() { Property = new Matrix3x2(1, -2, 3, 4, 5, 6) };
        yield return new() { Property = new Matrix3x2(-1, 2, 3, 4, 5, 6) };
        yield return new() { Property = new Matrix3x2(1, 2, -3, 4, 5, 6) };
        yield return new() { Property = new Matrix3x2(0, 0, 0, 4, 5, 6) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property);
    }

    public override async Task AssertGoogleResult(Matrix3x2TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.M.Count).IsEqualTo(6);
        var matrix = new Matrix3x2(
            clone.Property.M[0],
            clone.Property.M[1],
            clone.Property.M[2],
            clone.Property.M[3],
            clone.Property.M[4],
            clone.Property.M[5]
        );
        await Assert.That(matrix).IsEqualTo(message.Property);
    }

    [Test]
    public async Task NullFloatsArray_Should_ParseToDefault()
    {
        var protoParser = new Matrix3x2ProtoParser() { Floats = null! };

        Matrix3x2 result = protoParser;
        await Assert.That(result).IsEqualTo(default(Matrix3x2));
    }

    [Test]
    public async Task FloatsArrayWithInvalidLength_Should_ThrowException()
    {
        var protoParser = new Matrix3x2ProtoParser()
        {
            Floats = new float[] { 1, 2, 3 }, // Invalid length
        };

        var exception = await Assert
            .That(() =>
            {
                Matrix3x2 result = protoParser;
            })
            .Throws<ArgumentException>();
        await Assert.That(exception!.Message).Contains("Input array must contain 6 elements for Matrix3x2 conversion.");
    }
}
#endif
