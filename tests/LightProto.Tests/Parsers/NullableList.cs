using LightProto;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class NullableListTests : BaseTests<NullableListTests.Message, NullableListTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public List<int?> Property { get; set; } = [];
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
        yield return new() { Property = [] };
        yield return new() { Property = [0] };
        yield return new() { Property = [1, 2, 3] };
        yield return new() { Property = [-1, 0, 1] };
    }

    public override IEnumerable<NullableListTestsMessage> GetGoogleMessages()
    {
        return GetMessages().Select(o => new NullableListTestsMessage() { Property = { o.Property.Select(p => p ?? 0).ToArray() } });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.Count).IsEquivalentTo(message.Property.Count);
        for (var i = 0; i < clone.Property.Count; i++)
        {
            var cloneItem = clone.Property[i];
            var messageItem = message.Property[i];
            await Assert.That(cloneItem.HasValue).IsEquivalentTo(messageItem.HasValue);
            if (cloneItem.HasValue && messageItem.HasValue)
                await Assert.That(cloneItem.Value).IsEquivalentTo(messageItem.Value);
        }
    }

    public override async Task AssertGoogleResult(NullableListTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property.Count).IsEquivalentTo(message.Property.Count);
        for (var i = 0; i < clone.Property.Count; i++)
        {
            var cloneItem = clone.Property[i];
            var messageItem = message.Property[i];
            await Assert.That(messageItem ?? 0).IsEquivalentTo(cloneItem);
        }
    }
}

file static class Extension
{
    public static ProtoBuf.Bcl.DateTime ToProtobuf(this DateTime dt)
    {
        return new ProtoBuf.Bcl.DateTime()
        {
            Value = dt.Ticks - EpochOriginsTicks[(int)dt.Kind],
            Scale = ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Ticks,
            Kind = (ProtoBuf.Bcl.DateTime.Types.DateTimeKind)dt.Kind,
        };
    }

    internal static readonly long[] EpochOriginsTicks =
    [
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).Ticks,
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks,
        new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local).Ticks,
    ];

    public static DateTime ToDateTime(this ProtoBuf.Bcl.DateTime proxy)
    {
        long ticks;
        switch (proxy.Scale)
        {
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Days:
                ticks = proxy.Value * TimeSpan.TicksPerDay;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Hours:
                ticks = proxy.Value * TimeSpan.TicksPerHour;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minutes:
                ticks = proxy.Value * TimeSpan.TicksPerMinute;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Seconds:
                ticks = proxy.Value * TimeSpan.TicksPerSecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Milliseconds:
                ticks = proxy.Value * TimeSpan.TicksPerMillisecond;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Ticks:
                ticks = proxy.Value;
                break;
            case global::ProtoBuf.Bcl.DateTime.Types.TimeSpanScale.Minmax:
                if (proxy.Value == -1)
                    return DateTime.MinValue;
                else if (proxy.Value == 1)
                    return DateTime.MaxValue;
                else
                    throw new ArgumentOutOfRangeException(nameof(proxy.Value), $"Invalid ticks for MINMAX scale: {proxy.Value}");
            default:
                throw new ArgumentOutOfRangeException(nameof(proxy.Scale), $"Unknown scale: {proxy.Scale}");
        }
        return new DateTime(ticks: ticks + EpochOriginsTicks[(int)proxy.Kind], kind: (System.DateTimeKind)proxy.Kind);
    }
}
