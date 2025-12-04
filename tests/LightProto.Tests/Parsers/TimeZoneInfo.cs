using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class TimeZoneInfoTests
    : BaseTests<TimeZoneInfoTests.Message, TimeZoneInfoTestsMessage>
{
    [ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        public required TimeZoneInfo Property { get; set; }
    }

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = TimeZoneInfo.Utc };
        yield return new() { Property = TimeZoneInfo.Local };
        if (OperatingSystem.IsWindows())
        {
            yield return new()
            {
                Property = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"),
            };
        }
        else
        {
            yield return new() { Property = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai") };
        }
    }

    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;

    public override IEnumerable<TimeZoneInfoTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o => new TimeZoneInfoTestsMessage()
            {
                Property = o.Property.ToSerializedString(),
            });
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        // https://github.com/dotnet/runtime/issues/19794 :TimeZoneInfo.ToSerializedString/FromSerializedString do not round trip on Unix
        await Assert
            .That(clone.Property.BaseUtcOffset)
            .IsEquivalentTo(message.Property.BaseUtcOffset);
        await Assert.That(clone.Property.Id).IsEquivalentTo(message.Property.Id);
        await Assert.That(clone.Property.DisplayName).IsEquivalentTo(message.Property.DisplayName);
        await Assert
            .That(clone.Property.StandardName)
            .IsEquivalentTo(message.Property.StandardName);
        await Assert
            .That(clone.Property.DaylightName)
            .IsEquivalentTo(message.Property.DaylightName);
    }

    public override async Task AssertGoogleResult(TimeZoneInfoTestsMessage clone, Message message)
    {
        await Assert.That(clone.Property).IsEquivalentTo(message.Property.ToSerializedString());
    }
}
