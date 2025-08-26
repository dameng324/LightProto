using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public interface IPbMessageParser<TSelf> : IMessage<TSelf>
    where TSelf : IPbMessageParser<TSelf>
{
    /// <summary>
    /// please use `dotnet add package Dameng.Protobuf.Extension.Generator` to use auto generated Parser property
    /// Gets the static parser instance for the protobuf message type.
    /// This property is automatically available on all protobuf generated classes.
    /// 
    /// </summary>
    public static abstract Google.Protobuf.MessageParser<TSelf> Parser { get; }
}
