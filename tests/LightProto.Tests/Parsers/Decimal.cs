using System.Runtime.InteropServices;
using LightProto;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class DecimalTests: BaseTests<DecimalTests.Message,DecimalTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public decimal Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new () { Property = Decimal.MinValue };
        yield return new () { Property = Decimal.MaxValue };
        yield return new () { Property = 0 };
        yield return new () { Property = 1000M };
        yield return new () { Property = -1000M };
    }

    public override IEnumerable<DecimalTestsMessage> GetGoogleMessages()
    {
        yield return new () { Property = Decimal.MinValue.ToProtobuf() };
        yield return new () { Property = Decimal.MaxValue.ToProtobuf() };
        yield return new () { Property = 0M.ToProtobuf() };
        yield return new () { Property = 1000M.ToProtobuf() };
        yield return new () { Property = (-1000M).ToProtobuf() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(DecimalTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.ToDecimal()).IsEquivalentTo(message.Property);
    }
}

file static class BclDecimalExtension
{
    public static ProtoBuf.Bcl.Decimal ToProtobuf(this decimal value)
    {
        var dec = new DecimalAccessor(value);
        ulong a = ((ulong)dec.Mid) << 32,
            b = ((ulong)dec.Lo) & 0xFFFFFFFFL;
        ulong low = a | b;
        uint high = (uint)dec.Hi;
        uint signScale = (uint)(((dec.Flags >> 15) & 0x01FE) | ((dec.Flags >> 31) & 0x0001));
        return new ProtoBuf.Bcl.Decimal
        {
            Lo = low,
            Hi = high,
            SignScale = signScale,
        };
    }

    public static decimal ToDecimal(this ProtoBuf.Bcl.Decimal proxy)
    {
        if (proxy is null)
        {
            return 0;
        }
        int lo = (int)(proxy.Lo & 0xFFFFFFFFL),
            mid = (int)((proxy.Lo >> 32) & 0xFFFFFFFFL),
            hi = (int)proxy.Hi;
        bool isNeg = (proxy.SignScale & 0x0001) == 0x0001;
        byte scale = (byte)((proxy.SignScale & 0x01FE) >> 1);
        return new decimal(lo, mid, hi, isNeg, scale);
    }

    /// <summary>
    /// Provides access to the inner fields of a decimal.
    /// Similar to decimal.GetBits(), but faster and avoids the int[] allocation
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    private readonly struct DecimalAccessor
    {
        [FieldOffset(0)]
        public readonly int Flags;

        [FieldOffset(4)]
        public readonly int Hi;

        [FieldOffset(8)]
        public readonly int Lo;

        [FieldOffset(12)]
        public readonly int Mid;

        [FieldOffset(0)]
        public readonly decimal Decimal;

        public DecimalAccessor(decimal value)
        {
            this = default;
            Decimal = value;
        }
    }
}