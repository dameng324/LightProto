using System.Buffers;
using Google.Protobuf;
using LightProto;
using LightProto.Parser;

namespace LightProto.Tests.Parsers;

[InheritsTests]
public partial class ReadOnlySequenceBytesTests : BaseTests<ReadOnlySequenceBytesTests.Message, RepeatedBytesTestsMessage>
{
    [ProtoContract]
    [ProtoBuf.ProtoContract]
    public partial record Message
    {
        [ProtoMember(1)]
        [ProtoBuf.ProtoMember(1)]
        public ReadOnlySequence<byte> Property { get; set; } = default;

        public override string ToString()
        {
            return $"Property: {string.Join(", ", Property.ToArray())}";
        }
    }

    class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(byte[] memory)
        {
            Memory = memory;
        }

        public BufferSegment Append(byte[] memory)
        {
            var segment = new BufferSegment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = segment;
            return segment;
        }
    }

    static ReadOnlySequence<byte> CreateReadOnlySequence(params byte[][] segments)
    {
        if (segments.Length == 0)
            return default;
        var first = new BufferSegment(segments[0]);
        var last = first;
        for (int i = 1; i < segments.Length; i++)
        {
            last = last.Append(segments[i]);
        }
        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    protected override bool ProtoBuf_net_Serialize_Disabled { get; } = true;
    protected override bool ProtoBuf_net_Deserialize_Disabled { get; } = true;

    public override IEnumerable<Message> GetMessages()
    {
        yield return new() { Property = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }) };
        yield return new() { Property = CreateReadOnlySequence(new byte[] { 1, 2 }, new byte[] { 3, 4, 5 }) };
        yield return new() { Property = new ReadOnlySequence<byte>(new byte[] { 0, 0, 0, 0, 0 }) };
        yield return new() { Property = new ReadOnlySequence<byte>(new byte[] { 0 }) };
        yield return new() { Property = default };
    }

    public override IEnumerable<RepeatedBytesTestsMessage> GetGoogleMessages()
    {
        return GetMessages()
            .Select(o =>
            {
                var message = new RepeatedBytesTestsMessage();
                foreach (var segment in o.Property)
                {
                    if (segment.IsEmpty)
                        continue;
                    message.Property.Add(ByteString.CopyFrom(segment.Span));
                }
                return message;
            });
    }

    public override async Task AssertGoogleResult(RepeatedBytesTestsMessage clone, Message message)
    {
        var expected = message.Property.ToArray();
        var actual = clone.Property.SelectMany(x => x.ToByteArray()).ToArray();
        await Assert.That(actual).IsEquivalentTo(expected);
    }

    public override async Task AssertResult(Message clone, Message message)
    {
        await Assert.That(clone.Property.ToArray()).IsEquivalentTo(message.Property.ToArray());
    }

    [Test]
    public async Task EmptyTest()
    {
        byte[] bytes = [];
        var deserialized = Serializer.Deserialize(bytes, new ReadOnlySequenceProtoReader<byte>(ByteProtoParser.ProtoReader, 0));
        await Assert.That(deserialized.IsEmpty).IsTrue();
    }
}
