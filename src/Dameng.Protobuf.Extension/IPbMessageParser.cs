using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

/// <summary>
/// Interface that enables generic protobuf message parsing by exposing the static Parser property.
/// This interface is automatically implemented by the source generator for all protobuf generated classes.
/// </summary>
/// <typeparam name="TSelf">The protobuf message type that implements this interface</typeparam>
public interface IPbMessageParser<TSelf> where TSelf : IPbMessageParser<TSelf>, IMessage<TSelf>
{
    /// <summary>
    /// Gets the static parser instance for the protobuf message type.
    /// This property is automatically available on all protobuf generated classes.
    /// </summary>
    public static abstract Google.Protobuf.MessageParser<TSelf> Parser { get; }
}