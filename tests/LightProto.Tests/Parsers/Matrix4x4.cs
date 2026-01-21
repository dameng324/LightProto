#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class Matrix4x4Tests : BaseTests<Matrix4x4Tests.Message, Matrix4x4TestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Matrix4x4 Property { get; set; }
    }

    public override IEnumerable<Matrix4x4TestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new Matrix4x4TestsMessage()
            {
                Property = new Matrix4x4Message()
                {
                    M =
                    {
                        x.Property.M11,
                        x.Property.M12,
                        x.Property.M13,
                        x.Property.M14,
                        x.Property.M21,
                        x.Property.M22,
                        x.Property.M23,
                        x.Property.M24,
                        x.Property.M31,
                        x.Property.M32,
                        x.Property.M33,
                        x.Property.M34,
                        x.Property.M41,
                        x.Property.M42,
                        x.Property.M43,
                        x.Property.M44,
                    },
                },
            });
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Matrix4x4(-1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) };
        yield return new() { Property = new Matrix4x4(1, -2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) };
        yield return new() { Property = new Matrix4x4(-1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) };
        yield return new() { Property = new Matrix4x4(1, 2, -3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) };
        yield return new() { Property = new Matrix4x4(0, 0, 0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEqualTo(message.Property);
    }

    public override async Task AssertGoogleResult(Matrix4x4TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.M.Count).IsEqualTo(16);
        var matrix = new Matrix4x4(
            clone.Property.M[0],
            clone.Property.M[1],
            clone.Property.M[2],
            clone.Property.M[3],
            clone.Property.M[4],
            clone.Property.M[5],
            clone.Property.M[6],
            clone.Property.M[7],
            clone.Property.M[8],
            clone.Property.M[9],
            clone.Property.M[10],
            clone.Property.M[11],
            clone.Property.M[12],
            clone.Property.M[13],
            clone.Property.M[14],
            clone.Property.M[15]
        );
        await Assert.That(matrix).IsEqualTo(message.Property);
    }
}
#endif
