using Google.Protobuf.WellKnownTypes;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;
[InheritsTests]
public partial class TimeSpan240Tests: BaseTests<TimeSpan240Tests.Message,TimeSpan240TestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        [CompatibilityLevel(CompatibilityLevel.Level240)]
        [ProtoBuf.CompatibilityLevel(ProtoBuf.CompatibilityLevel.Level240)]
        public TimeSpan Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {

        yield return new () { Property = TimeSpan.Zero };
        yield return new () { Property = DateTime.Now.TimeOfDay };
    }

    public override IEnumerable<TimeSpan240TestsMessage> GetGoogleMessages()
    {
        yield return new () { Property = DateTime.Now.TimeOfDay.ToDuration() };
        yield return new () { Property = TimeSpan.Zero.ToDuration() };
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property);
    }

    public override async Task AssertGoogleResult(TimeSpan240TestsMessage clone, Message message)
    {
        await Assert.That(clone.Property?.ToTimeSpan()??default).IsEquivalentTo(message.Property);
    }
}