

namespace Dameng.Protobuf;

public static class Extensions
{
    public static int CalculateSize<T>(this T message) where T : IProtoMessage<T>
    {
        return T.Writer.CalculateSize(message);
    }
    public static int CalculateMessageSize<T>(this T message) where T : IProtoMessage<T>
    {
        var size = T.Writer.CalculateSize(message);
        return CodedOutputStream.ComputeLengthSize(size) + size;
    }
}