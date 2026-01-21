using System.Collections;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class BitArrayTests : BaseTests<BitArrayTests.Message, BitArrayTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public BitArray Property { get; set; } = new(0);

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property.ToBoolArray())}";
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new BitArray(10, false) };
        yield return new() { Property = new BitArray(7, true) };
        yield return new() { Property = new BitArray(0) };
    }

    public override IEnumerable<BitArrayTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new BitArrayTestsMessage() { Property = new BitArrayMessage() { Bits = { o.Property.ToBoolArray() } } });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.ToBoolArray()).IsEquivalentTo(message.Property.ToBoolArray());
    }

    public override async Task AssertGoogleResult(BitArrayTestsMessage clone, Message message)
    {
        clone.Property ??= new BitArrayMessage();
        await Assert.That(clone.Property.Bits.ToArray()).IsEquivalentTo(message.Property.ToBoolArray());
    }
}

file static class BitArrayExtensions
{
    public static bool[] ToBoolArray(this BitArray bitArray)
    {
        var result = new bool[bitArray.Count];
        bitArray.CopyTo(result, 0);
        return result;
    }
}
