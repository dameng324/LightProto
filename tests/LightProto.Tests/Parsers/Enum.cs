using System.Buffers.Binary;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class EnumTests : BaseTests<EnumTests.Message, EnumTestsMessage>
{
    public enum TestByteEnum : byte
    {
        MinValue = byte.MinValue,
        Middle1 = byte.MinValue / 2,
        Zero = 0,
        Middle2 = byte.MaxValue / 2,
        MaxValue = byte.MaxValue,
    }

    public enum TestSByteEnum : sbyte
    {
        MinValue = sbyte.MinValue,

        Middle1 = sbyte.MinValue / 2,
        Zero = 0,
        Middle2 = sbyte.MaxValue / 2,
        MaxValue = sbyte.MaxValue,
    }

    public enum TestShortEnum : short
    {
        MinValue = short.MinValue,
        Middle1 = short.MinValue / 2,
        Zero = 0,
        Middle2 = short.MaxValue / 2,
        MaxValue = short.MaxValue,
    }

    public enum TestUShortEnum : ushort
    {
        MinValue = ushort.MinValue,
        Middle1 = ushort.MinValue / 2,
        Zero = 0,
        Middle2 = ushort.MaxValue / 2,
        MaxValue = ushort.MaxValue,
    }

    public enum TestIntEnum : int
    {
        MinValue = int.MinValue,
        Middle1 = int.MinValue / 2,

        Zero = 0,
        Middle2 = int.MaxValue / 2,
        MaxValue = int.MaxValue,
    }

    public enum TestUIntEnum : uint
    {
        MinValue = uint.MinValue,
        Middle1 = uint.MinValue / 2,
        Zero = 0,
        Middle2 = uint.MaxValue / 2,
        MaxValue = uint.MaxValue,
    }

    public enum TestLongEnum : long
    {
        MinValue = long.MinValue,
        Middle1 = long.MinValue / 2,
        Zero = 0,
        Middle2 = long.MaxValue / 2,
        MaxValue = long.MaxValue,
    }

    public enum TestULongEnum : ulong
    {
        MinValue = ulong.MinValue,
        Middle1 = ulong.MinValue / 2,
        Zero = 0,
        Middle2 = ulong.MaxValue / 2,
        MaxValue = ulong.MaxValue,
    }

    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [LightProto.ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public TestByteEnum ByteEnum { get; set; }

        [LightProto.ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public TestSByteEnum SByteEnum { get; set; }

        [LightProto.ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public TestShortEnum ShortEnum { get; set; }

        [LightProto.ProtoMember(4)]
        [ProtoBuf.ProtoMember(4)]
        public TestUShortEnum UShortEnum { get; set; }

        [LightProto.ProtoMember(5)]
        [ProtoBuf.ProtoMember(5)]
        public TestIntEnum IntEnum { get; set; }

        [LightProto.ProtoMember(6)]
        [ProtoBuf.ProtoMember(6)]
        public TestUIntEnum UIntEnum { get; set; }

        [LightProto.ProtoMember(7)]
        [ProtoBuf.ProtoMember(7)]
        public TestLongEnum LongEnum { get; set; }

        [LightProto.ProtoMember(8)]
        [ProtoBuf.ProtoMember(8)]
        public TestULongEnum ULongEnum { get; set; }
    }

    public override IEnumerable<EnumTestsMessage> GetGoogleMessages()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new EnumTestsMessage
            {
                ByteEnum = (uint)(byte)(Enum.GetValues(typeof(TestByteEnum)).GetValue(i)!),
                SByteEnum = (int)(sbyte)(Enum.GetValues(typeof(TestSByteEnum)).GetValue(i)!),
                ShortEnum = (int)(short)(Enum.GetValues(typeof(TestShortEnum)).GetValue(i)!),
                UShortEnum = (uint)(ushort)(Enum.GetValues(typeof(TestUShortEnum)).GetValue(i)!),
                IntEnum = (int)(Enum.GetValues(typeof(TestIntEnum)).GetValue(i)!),
                UIntEnum = (uint)(Enum.GetValues(typeof(TestUIntEnum)).GetValue(i)!),
                LongEnum = (long)(Enum.GetValues(typeof(TestLongEnum)).GetValue(i)!),
                ULongEnum = (ulong)(Enum.GetValues(typeof(TestULongEnum)).GetValue(i)!),
            };
        }
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.ByteEnum).IsEquivalentTo(message.ByteEnum);
        await Assert.That(clone.SByteEnum).IsEquivalentTo(message.SByteEnum);
        await Assert.That(clone.ShortEnum).IsEquivalentTo(message.ShortEnum);
        await Assert.That(clone.UShortEnum).IsEquivalentTo(message.UShortEnum);
        await Assert.That(clone.IntEnum).IsEquivalentTo(message.IntEnum);
        await Assert.That(clone.UIntEnum).IsEquivalentTo(message.UIntEnum);
        await Assert.That(clone.LongEnum).IsEquivalentTo(message.LongEnum);
        await Assert.That(clone.ULongEnum).IsEquivalentTo(message.ULongEnum);
    }

    public override async Task AssertGoogleResult(EnumTestsMessage clone, Message message)
    {
        await Assert.That((TestByteEnum)(byte)clone.ByteEnum).IsEquivalentTo(message.ByteEnum);
        await Assert.That((TestSByteEnum)(sbyte)clone.SByteEnum).IsEquivalentTo(message.SByteEnum);
        await Assert.That((TestShortEnum)(short)clone.ShortEnum).IsEquivalentTo(message.ShortEnum);
        await Assert.That((TestUShortEnum)(ushort)clone.UShortEnum).IsEquivalentTo(message.UShortEnum);
        await Assert.That((TestIntEnum)clone.IntEnum).IsEquivalentTo(message.IntEnum);
        await Assert.That((TestUIntEnum)clone.UIntEnum).IsEquivalentTo(message.UIntEnum);
        await Assert.That((TestLongEnum)clone.LongEnum).IsEquivalentTo(message.LongEnum);
        await Assert.That((TestULongEnum)clone.ULongEnum).IsEquivalentTo(message.ULongEnum);
    }

    public override IEnumerable<Message> GetMessages()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new Message
            {
                ByteEnum = (TestByteEnum)(Enum.GetValues(typeof(TestByteEnum)).GetValue(i)!),
                SByteEnum = (TestSByteEnum)(Enum.GetValues(typeof(TestSByteEnum)).GetValue(i)!),
                ShortEnum = (TestShortEnum)(Enum.GetValues(typeof(TestShortEnum)).GetValue(i)!),
                UShortEnum = (TestUShortEnum)(Enum.GetValues(typeof(TestUShortEnum)).GetValue(i)!),
                IntEnum = (TestIntEnum)(Enum.GetValues(typeof(TestIntEnum)).GetValue(i)!),
                UIntEnum = (TestUIntEnum)(Enum.GetValues(typeof(TestUIntEnum)).GetValue(i)!),
                LongEnum = (TestLongEnum)(Enum.GetValues(typeof(TestLongEnum)).GetValue(i)!),
                ULongEnum = (TestULongEnum)(Enum.GetValues(typeof(TestULongEnum)).GetValue(i)!),
            };
        }
    }
}
