using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public static class ProtoBuf
{
    public static byte[] ToByteArray<T>(this T message)
        where T : IProtoBufMessage<T>
    {
        var buffer = new byte[message.CalculateSize()];
        CodedOutputStream output = new CodedOutputStream(buffer);
        WriteContext.Initialize(output,out var ctx);
        message.WriteTo(ref ctx);
        return buffer;
    }
    
    public static T ParseFromBytes<T>(byte[] bytes) where T : IProtoBufMessage<T>
    {
        Google.Protobuf.ParseContext.Initialize(new Google.Protobuf.CodedInputStream(bytes), out var ctx);
        return T.ParseFrom(ref ctx);
    }
}