using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public interface IPbMessageParser<TSelf> : IMessage<TSelf>
    where TSelf : IPbMessageParser<TSelf>
{
    /// <summary>
    /// Gets the static parser instance for the protobuf message type.
    /// This property is automatically available on all protobuf generated classes.
    /// </summary>
    public static abstract Google.Protobuf.MessageParser<TSelf> Parser { get; }
}
