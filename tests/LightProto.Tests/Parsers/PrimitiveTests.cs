using Google.Protobuf;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class PrimitiveTests : BaseTests<PrimitiveTests.Message, PrimitiveTestMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public bool BooleanValue { get; set; }

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public double DoubleValue { get; set; }

        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public int Int32Value { get; set; }

        [ProtoMember(4, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(4, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public int SFixed32Value { get; set; }

        [ProtoMember(5, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(5, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public uint Fixed32Value { get; set; }

        [ProtoMember(6, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(6, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public long SFixed64Value { get; set; }

        [ProtoMember(7, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(7, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public ulong Fixed64Value { get; set; }

        [ProtoMember(8)]
        [ProtoBuf.ProtoMember(8)]
        public long Int64Value { get; set; }

        [ProtoMember(9, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(9, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public long SInt64Value { get; set; }

        [ProtoMember(10, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(10, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public int SInt32Value { get; set; }

        [ProtoMember(11)]
        [ProtoBuf.ProtoMember(11)]
        public float SingleValue { get; set; }

        [ProtoMember(12)]
        [ProtoBuf.ProtoMember(12)]
        public ulong UInt64Value { get; set; }

        [ProtoMember(13)]
        [ProtoBuf.ProtoMember(13)]
        public uint UInt32Value { get; set; }

        [ProtoMember(14)]
        [ProtoBuf.ProtoMember(14)]
        public byte ByteValue { get; set; } //uint32

        [ProtoMember(15, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(15, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public byte FixedByteValue { get; set; } //fixed32

        [ProtoMember(16)]
        [ProtoBuf.ProtoMember(16)]
        public sbyte SByteValue { get; set; } //int32

        [ProtoMember(17, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(17, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public sbyte SSByteValue { get; set; } //sint32

        [ProtoMember(18, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(18, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public sbyte SFixedByteValue { get; set; } //sfixed32

        [ProtoMember(19)]
        [ProtoBuf.ProtoMember(19)]
        public char CharValue { get; set; } //uint32

        [ProtoMember(20, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(20, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public char FixedCharValue { get; set; } //fixed32

        [ProtoMember(21)]
        [ProtoBuf.ProtoMember(21)]
        public ushort UShortValue { get; set; } //uint32

        [ProtoMember(22, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(22, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public ushort FixedUShortValue { get; set; } //fixed32

        [ProtoMember(23)]
        [ProtoBuf.ProtoMember(23)]
        public short ShortValue { get; set; } //int32

        [ProtoMember(24, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(24, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public short SShortValue { get; set; } //sint32

        [ProtoMember(25, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(25, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public short SFixedShortValue { get; set; } //sfixed32
    }

    [Test]
    [Explicit]
    public void GetProto()
    {
        var proto = ProtoBuf.Serializer.GetProto<Message>();
        Console.WriteLine(proto);
    }

    public override IEnumerable<Message> GetMessages()
    {
        ulong baseValue = 7;
        foreach (var i in Enumerable.Range(0, 20))
        {
            baseValue *= 10;
            yield return new()
            {
                BooleanValue = i % 2 == 0,
                DoubleValue = baseValue * 1.1,
                Int32Value = (int)baseValue,
                SFixed32Value = (int)baseValue,
                Fixed32Value = (uint)baseValue,
                SFixed64Value = (int)baseValue,
                Fixed64Value = baseValue,
                Int64Value = (long)(baseValue),
                SInt64Value = (long)(baseValue),
                SInt32Value = (int)baseValue,
                SingleValue = (float)(baseValue * 1.1),
                UInt64Value = baseValue,
                UInt32Value = (uint)baseValue,
                ByteValue = (byte)(baseValue % 256),
                FixedByteValue = (byte)(baseValue % 256),
                SByteValue = (sbyte)(baseValue % 256 - 128),
                SSByteValue = (sbyte)(baseValue % 256 - 128),
                SFixedByteValue = (sbyte)(baseValue % 256 - 128),
                CharValue = (char)(baseValue % char.MaxValue),
                FixedCharValue = (char)(baseValue % char.MaxValue),
                UShortValue = (ushort)baseValue,
                FixedUShortValue = (ushort)baseValue,
                ShortValue = (short)baseValue,
                SShortValue = (short)baseValue,
                SFixedShortValue = (short)baseValue,
            };
        }
    }

    public override IEnumerable<PrimitiveTestMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new PrimitiveTestMessage
            {
                BooleanValue = o.BooleanValue,
                DoubleValue = o.DoubleValue,
                Int32Value = o.Int32Value,
                SFixed32Value = o.SFixed32Value,
                Fixed32Value = o.Fixed32Value,
                SFixed64Value = o.SFixed64Value,
                Fixed64Value = o.Fixed64Value,
                Int64Value = o.Int64Value,
                SInt64Value = o.SInt64Value,
                SInt32Value = o.SInt32Value,
                SingleValue = o.SingleValue,
                UInt64Value = o.UInt64Value,
                UInt32Value = o.UInt32Value,
                ByteValue = o.ByteValue,
                FixedByteValue = o.FixedByteValue,
                SByteValue = o.SByteValue,
                SSByteValue = o.SSByteValue,
                SFixedByteValue = o.SFixedByteValue,
                CharValue = o.CharValue,
                FixedCharValue = o.FixedCharValue,
                UShortValue = o.UShortValue,
                FixedUShortValue = o.FixedUShortValue,
                ShortValue = o.ShortValue,
                SShortValue = o.SShortValue,
                SFixedShortValue = o.SFixedShortValue,
            });
    }

    public override async Task AssertGoogleResult(PrimitiveTestMessage clone, Message message)
    {
        await Assert.That(clone.BooleanValue).IsEquivalentTo(message.BooleanValue);
        await Assert.That(clone.DoubleValue).IsEquivalentTo(message.DoubleValue);
        await Assert.That(clone.Int32Value).IsEquivalentTo(message.Int32Value);
        await Assert.That(clone.SFixed32Value).IsEquivalentTo(message.SFixed32Value);
        await Assert.That(clone.Fixed32Value).IsEquivalentTo(message.Fixed32Value);
        await Assert.That(clone.SFixed64Value).IsEquivalentTo(message.SFixed64Value);
        await Assert.That(clone.Fixed64Value).IsEquivalentTo(message.Fixed64Value);
        await Assert.That(clone.Int64Value).IsEquivalentTo(message.Int64Value);
        await Assert.That(clone.SInt64Value).IsEquivalentTo(message.SInt64Value);
        await Assert.That(clone.SInt32Value).IsEquivalentTo(message.SInt32Value);
        await Assert.That(clone.SingleValue).IsEquivalentTo(message.SingleValue);
        await Assert.That(clone.UInt64Value).IsEquivalentTo(message.UInt64Value);
        await Assert.That(clone.UInt32Value).IsEquivalentTo(message.UInt32Value);
        await Assert.That((byte)clone.ByteValue).IsEquivalentTo(message.ByteValue);
        await Assert.That((byte)clone.FixedByteValue).IsEquivalentTo(message.FixedByteValue);
        await Assert.That((sbyte)clone.SByteValue).IsEquivalentTo(message.SByteValue);
        await Assert.That((sbyte)clone.SSByteValue).IsEquivalentTo(message.SSByteValue);
        await Assert.That((sbyte)clone.SFixedByteValue).IsEquivalentTo(message.SFixedByteValue);
        await Assert.That((char)clone.CharValue).IsEquivalentTo(message.CharValue);
        await Assert.That((char)clone.FixedCharValue).IsEquivalentTo(message.FixedCharValue);
        await Assert.That((ushort)clone.UShortValue).IsEquivalentTo(message.UShortValue);
        await Assert.That((ushort)clone.FixedUShortValue).IsEquivalentTo(message.FixedUShortValue);
        await Assert.That((short)clone.ShortValue).IsEquivalentTo(message.ShortValue);
        await Assert.That((short)clone.SShortValue).IsEquivalentTo(message.SShortValue);
        await Assert.That((short)clone.SFixedShortValue).IsEquivalentTo(message.SFixedShortValue);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.BooleanValue).IsEquivalentTo(message.BooleanValue);
        await Assert.That(clone.DoubleValue).IsEquivalentTo(message.DoubleValue);
        await Assert.That(clone.Int32Value).IsEquivalentTo(message.Int32Value);
        await Assert.That(clone.SFixed32Value).IsEquivalentTo(message.SFixed32Value);
        await Assert.That(clone.Fixed32Value).IsEquivalentTo(message.Fixed32Value);
        await Assert.That(clone.SFixed64Value).IsEquivalentTo(message.SFixed64Value);
        await Assert.That(clone.Fixed64Value).IsEquivalentTo(message.Fixed64Value);
        await Assert.That(clone.Int64Value).IsEquivalentTo(message.Int64Value);
        await Assert.That(clone.SInt64Value).IsEquivalentTo(message.SInt64Value);
        await Assert.That(clone.SInt32Value).IsEquivalentTo(message.SInt32Value);
        await Assert.That(clone.SingleValue).IsEquivalentTo(message.SingleValue);
        await Assert.That(clone.UInt64Value).IsEquivalentTo(message.UInt64Value);
        await Assert.That(clone.UInt32Value).IsEquivalentTo(message.UInt32Value);
        await Assert.That(clone.ByteValue).IsEquivalentTo(message.ByteValue);
        await Assert.That(clone.FixedByteValue).IsEquivalentTo(message.FixedByteValue);
        await Assert.That(clone.SByteValue).IsEquivalentTo(message.SByteValue);
        await Assert.That(clone.SSByteValue).IsEquivalentTo(message.SSByteValue);
        await Assert.That(clone.SFixedByteValue).IsEquivalentTo(message.SFixedByteValue);
        await Assert.That(clone.CharValue).IsEquivalentTo(message.CharValue);
        await Assert.That(clone.FixedCharValue).IsEquivalentTo(message.FixedCharValue);
        await Assert.That(clone.UShortValue).IsEquivalentTo(message.UShortValue);
        await Assert.That(clone.FixedUShortValue).IsEquivalentTo(message.FixedUShortValue);
        await Assert.That(clone.ShortValue).IsEquivalentTo(message.ShortValue);
        await Assert.That(clone.SShortValue).IsEquivalentTo(message.SShortValue);
        await Assert.That(clone.SFixedShortValue).IsEquivalentTo(message.SFixedShortValue);
    }
}
