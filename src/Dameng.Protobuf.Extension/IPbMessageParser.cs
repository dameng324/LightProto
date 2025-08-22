using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public interface IPbMessageParser<TSelf> where TSelf : IPbMessageParser<TSelf>, IMessage<TSelf>
{
    public static abstract Google.Protobuf.MessageParser<TSelf> Parser { get; }
}