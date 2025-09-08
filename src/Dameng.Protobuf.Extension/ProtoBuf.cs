using Google.Protobuf;

namespace Dameng.Protobuf.Extension;

public static class ProtoBuf
{
    public static byte[] ToByteArray<T>(this T message)
        where T : IProtoParser<T>
    {
        var buffer = new byte[T.Writer.CalculateSize(message)];
        CodedOutputStream output = new CodedOutputStream(buffer);
        WriteContext.Initialize(output,out var ctx);
        T.Writer.WriteTo(ref ctx,message);
        return buffer;
    }
    
    public static T ParseFromBytes<T>(byte[] bytes) where T : IProtoParser<T>
    {
        Google.Protobuf.ParseContext.Initialize(new Google.Protobuf.CodedInputStream(bytes), out var ctx);
        return T.Reader.ParseFrom(ref ctx);
    }
}