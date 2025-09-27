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
        foreach (var i in Enumerable.Range(0, 100))
        {
            yield return new()
            {
                BooleanValue = Random.Shared.Next() % 2 == 0,
                DoubleValue = Random.Shared.NextDouble() * 1000 - 500,
                Int32Value = Random.Shared.Next(int.MinValue, int.MaxValue),
                SFixed32Value = Random.Shared.Next(int.MinValue, int.MaxValue),
                Fixed32Value = (uint)Random.Shared.Next(int.MinValue, int.MaxValue),
                SFixed64Value = Random.Shared.NextInt64(long.MinValue, long.MaxValue),
                Fixed64Value = (ulong)Random.Shared.NextInt64(long.MinValue, long.MaxValue),
                Int64Value = Random.Shared.NextInt64(long.MinValue, long.MaxValue),
                SInt64Value = Random.Shared.NextInt64(long.MinValue, long.MaxValue),
                SInt32Value = Random.Shared.Next(int.MinValue, int.MaxValue),
                SingleValue = (float)(Random.Shared.NextDouble() * 1000 - 500),
                UInt64Value = (ulong)Random.Shared.NextInt64(long.MinValue, long.MaxValue),
                UInt32Value = (uint)Random.Shared.Next(int.MinValue, int.MaxValue),
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
    }
}
