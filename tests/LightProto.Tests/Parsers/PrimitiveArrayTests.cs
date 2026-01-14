using System.Collections.ObjectModel;
using Google.Protobuf;
using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class PrimitiveArrayTests
    : BaseTests<PrimitiveArrayTests.Message, PrimitiveArrayTestMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public bool[] BooleanValues { get; set; } = [];

        [ProtoMember(2)]
        [ProtoBuf.ProtoMember(2)]
        public double[] DoubleValues { get; set; } = [];

        [ProtoMember(3)]
        [ProtoBuf.ProtoMember(3)]
        public int[] Int32Values { get; set; } = [];

        [ProtoMember(4, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(4, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public int[] SFixed32Values { get; set; } = [];

        [ProtoMember(5, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(5, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public uint[] Fixed32Values { get; set; } = [];

        [ProtoMember(6, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(6, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public long[] SFixed64Values { get; set; } = [];

        [ProtoMember(7, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(7, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public ulong[] Fixed64Values { get; set; } = [];

        [ProtoMember(8)]
        [ProtoBuf.ProtoMember(8)]
        public long[] Int64Values { get; set; } = [];

        [ProtoMember(9, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(9, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public long[] SInt64Values { get; set; } = [];

        [ProtoMember(10, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(10, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public int[] SInt32Values { get; set; } = [];

        [ProtoMember(11)]
        [ProtoBuf.ProtoMember(11)]
        public float[] SingleValues { get; set; } = [];

        [ProtoMember(12)]
        [ProtoBuf.ProtoMember(12)]
        public ulong[] UInt64Values { get; set; } = [];

        [ProtoMember(13)]
        [ProtoBuf.ProtoMember(13)]
        public uint[] UInt32Values { get; set; } = [];

        [ProtoMember(14)]
        [ProtoBuf.ProtoMember(14)]
        public List<byte> ByteValues { get; set; } = []; //uint32

        [ProtoMember(15, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(15, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public List<byte> FixedByteValues { get; set; } = []; //fixed32

        [ProtoMember(16)]
        [ProtoBuf.ProtoMember(16)]
        public List<sbyte> SByteValues { get; set; } = []; //int32

        [ProtoMember(17, DataFormat = DataFormat.ZigZag)]
        [ProtoBuf.ProtoMember(17, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public List<sbyte> SSByteValues { get; set; } = []; //sint32

        [ProtoMember(18, DataFormat = DataFormat.FixedSize)]
        [ProtoBuf.ProtoMember(18, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public List<sbyte> SFixedByteValues { get; set; } = []; //sfixed32

        [ProtoMember(19, IsPacked = true)]
        [ProtoBuf.ProtoMember(19, IsPacked = true)]
        public List<byte> PackedByteValues { get; set; } = []; //uint32

        [ProtoMember(20, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        [ProtoBuf.ProtoMember(20, DataFormat = ProtoBuf.DataFormat.FixedSize, IsPacked = true)]
        public List<byte> PackedFixedByteValues { get; set; } = []; //fixed32

        [ProtoMember(21, IsPacked = true)]
        [ProtoBuf.ProtoMember(21, IsPacked = true)]
        public List<sbyte> PackedSByteValues { get; set; } = []; //int32

        [ProtoMember(22, DataFormat = DataFormat.ZigZag, IsPacked = true)]
        [ProtoBuf.ProtoMember(22, DataFormat = ProtoBuf.DataFormat.ZigZag)]
        public List<sbyte> PackedSSByteValues { get; set; } = []; //sint32

        [ProtoMember(23, DataFormat = DataFormat.FixedSize, IsPacked = true)]
        [ProtoBuf.ProtoMember(23, DataFormat = ProtoBuf.DataFormat.FixedSize)]
        public List<sbyte> PackedSFixedByteValues { get; set; } = []; //sfixed32
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
            var random = new Random();
            yield return new()
            {
                BooleanValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => random.Next() % 2 == 0)
                    .ToArray(),
                DoubleValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => random.NextDouble() * 1000 - 500)
                    .ToArray(),
                Int32Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                SFixed32Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                Fixed32Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (uint)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                SFixed64Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (long)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                Fixed64Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (ulong)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                Int64Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (long)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                SInt64Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (long)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                SInt32Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                SingleValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (float)(random.NextDouble() * 1000 - 500))
                    .ToArray(),
                UInt64Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (ulong)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                UInt32Values = Enumerable
                    .Range(0, 100)
                    .Select(_ => (uint)random.Next(int.MinValue, int.MaxValue))
                    .ToArray(),
                ByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (byte)random.Next(byte.MinValue, byte.MaxValue))
                    .ToList(),
                FixedByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (byte)random.Next(byte.MinValue, byte.MaxValue))
                    .ToList(),
                SByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
                SSByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
                SFixedByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
                PackedByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (byte)random.Next(byte.MinValue, byte.MaxValue))
                    .ToList(),
                PackedFixedByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (byte)random.Next(byte.MinValue, byte.MaxValue))
                    .ToList(),
                PackedSByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
                PackedSSByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
                PackedSFixedByteValues = Enumerable
                    .Range(0, 100)
                    .Select(_ => (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue))
                    .ToList(),
            };
        }
    }

    public override IEnumerable<PrimitiveArrayTestMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new PrimitiveArrayTestMessage
            {
                BooleanValues = { o.BooleanValues },
                DoubleValues = { o.DoubleValues },
                Int32Values = { o.Int32Values },
                SFixed32Values = { o.SFixed32Values },
                Fixed32Values = { o.Fixed32Values },
                SFixed64Values = { o.SFixed64Values },
                Fixed64Values = { o.Fixed64Values },
                Int64Values = { o.Int64Values },
                SInt64Values = { o.SInt64Values },
                SInt32Values = { o.SInt32Values },
                SingleValues = { o.SingleValues },
                UInt64Values = { o.UInt64Values },
                UInt32Values = { o.UInt32Values },
                ByteValues = { o.ByteValues.Select(x => (uint)x).ToArray() },
                FixedByteValues = { o.FixedByteValues.Select(x => (uint)x).ToArray() },
                SByteValues = { o.SByteValues.Select(x => (int)x).ToArray() },
                SSByteValues = { o.SSByteValues.Select(x => (int)x).ToArray() },
                SFixedByteValues = { o.SFixedByteValues.Select(x => (int)x).ToArray() },
                PackedByteValues = { o.PackedByteValues.Select(x => (uint)x).ToArray() },
                PackedFixedByteValues = { o.PackedFixedByteValues.Select(x => (uint)x).ToArray() },
                PackedSByteValues = { o.PackedSByteValues.Select(x => (int)x).ToArray() },
                PackedSSByteValues = { o.PackedSSByteValues.Select(x => (int)x).ToArray() },
                PackedSFixedByteValues = { o.PackedSFixedByteValues.Select(x => (int)x).ToArray() },
            });
    }

    public override async Task AssertGoogleResult(PrimitiveArrayTestMessage clone, Message message)
    {
        await Assert.That(clone.BooleanValues).IsEquivalentTo(message.BooleanValues);
        await Assert.That(clone.DoubleValues).IsEquivalentTo(message.DoubleValues);
        await Assert.That(clone.Int32Values).IsEquivalentTo(message.Int32Values);
        await Assert.That(clone.SFixed32Values).IsEquivalentTo(message.SFixed32Values);
        await Assert.That(clone.Fixed32Values).IsEquivalentTo(message.Fixed32Values);
        await Assert.That(clone.SFixed64Values).IsEquivalentTo(message.SFixed64Values);
        await Assert.That(clone.Fixed64Values).IsEquivalentTo(message.Fixed64Values);
        await Assert.That(clone.Int64Values).IsEquivalentTo(message.Int64Values);
        await Assert.That(clone.SInt64Values).IsEquivalentTo(message.SInt64Values);
        await Assert.That(clone.SInt32Values).IsEquivalentTo(message.SInt32Values);
        await Assert.That(clone.SingleValues).IsEquivalentTo(message.SingleValues);
        await Assert.That(clone.UInt64Values).IsEquivalentTo(message.UInt64Values);
        await Assert.That(clone.UInt32Values).IsEquivalentTo(message.UInt32Values);
        // await Assert.That(clone.ByteValues).IsEquivalentTo(message.ByteValues);
        // await Assert.That(clone.SByteValues).IsEquivalentTo(message.SByteValues);
        // await Assert.That(clone.FixedByteValues).IsEquivalentTo(message.FixedByteValues);
        // await Assert.That(clone.PackedByteValues).IsEquivalentTo(message.PackedByteValues);
        //  await Assert.That(clone.PackedSByteValues).IsEquivalentTo(message.PackedSByteValues);
        // await Assert.That(clone.PackedFixedByteValues).IsEquivalentTo(message.PackedFixedByteValues);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.BooleanValues).IsEquivalentTo(message.BooleanValues);
        await Assert.That(clone.DoubleValues).IsEquivalentTo(message.DoubleValues);
        await Assert.That(clone.Int32Values).IsEquivalentTo(message.Int32Values);
        await Assert.That(clone.SFixed32Values).IsEquivalentTo(message.SFixed32Values);
        await Assert.That(clone.Fixed32Values).IsEquivalentTo(message.Fixed32Values);
        await Assert.That(clone.SFixed64Values).IsEquivalentTo(message.SFixed64Values);
        await Assert.That(clone.Fixed64Values).IsEquivalentTo(message.Fixed64Values);
        await Assert.That(clone.Int64Values).IsEquivalentTo(message.Int64Values);
        await Assert.That(clone.SInt64Values).IsEquivalentTo(message.SInt64Values);
        await Assert.That(clone.SInt32Values).IsEquivalentTo(message.SInt32Values);
        await Assert.That(clone.SingleValues).IsEquivalentTo(message.SingleValues);
        await Assert.That(clone.UInt64Values).IsEquivalentTo(message.UInt64Values);
        await Assert.That(clone.UInt32Values).IsEquivalentTo(message.UInt32Values);
        // await Assert.That(clone.ByteValues).IsEquivalentTo(message.ByteValues);
        // await Assert.That(clone.SByteValues).IsEquivalentTo(message.SByteValues);
        // await Assert.That(clone.FixedByteValues).IsEquivalentTo(message.FixedByteValues);
        // await Assert.That(clone.PackedByteValues).IsEquivalentTo(message.PackedByteValues);
        // // await Assert.That(clone.PackedSByteValues).IsEquivalentTo(message.PackedSByteValues);
        // await Assert.That(clone.PackedFixedByteValues).IsEquivalentTo(message.PackedFixedByteValues);
    }
}
