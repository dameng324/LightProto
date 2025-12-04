#if NET7_0_OR_GREATER
using System.Buffers.Binary;
using System.Numerics;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ComplexTests : BaseTests<ComplexTests.Message, ComplexTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public Complex Property { get; set; }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<ComplexTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(x => new ComplexTestsMessage()
            {
                Property = new ComplexMessage()
                {
                    Real = x.Property.Real,
                    Imaginary = x.Property.Imaginary,
                },
            });
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new Complex(-1, -2) };
        yield return new() { Property = new Complex(1, -2) };
        yield return new() { Property = new Complex(-1, 2) };
        yield return new() { Property = new Complex(1, 2) };
        yield return new() { Property = new Complex(0, 0) };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Real).IsEqualTo(message.Property.Real);
        await Assert.That(clone.Property.Imaginary).IsEqualTo(message.Property.Imaginary);
    }

    public override async Task AssertGoogleResult(ComplexTestsMessage clone, Message message)
    {
        clone.Property ??= new ComplexMessage();
        await Assert.That(clone.Property.Real).IsEqualTo(message.Property.Real);
        await Assert.That(clone.Property.Imaginary).IsEqualTo(message.Property.Imaginary);
    }
}
#endif
