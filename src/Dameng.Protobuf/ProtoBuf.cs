

namespace Dameng.Protobuf;

public static class ProtoBuf
{
    public static byte[] ToByteArray<T>(this T message)
        where T : IProtoParser<T>
    {
        var buffer = new byte[T.Writer.CalculateSize(message)];
        CodedOutputStream output = new CodedOutputStream(buffer);
        WriterContext.Initialize(output,out var ctx);
        T.Writer.WriteTo(ref ctx,message);
        return buffer;
    }
    
    public static T ParseFrom<T>(this IProtoReader<T> reader,byte[] bytes) 
    {
        ReaderContext.Initialize(new CodedInputStream(bytes), out var ctx);
        return reader.ParseFrom(ref ctx);
    }
}