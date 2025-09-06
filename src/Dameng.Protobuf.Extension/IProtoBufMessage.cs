namespace Dameng.Protobuf.Extension;

#pragma warning disable 1591, 0612, 3021, 8981, CS9035
public interface IProtoBufMessage
{
    public int CalculateSize();
    public void WriteTo(ref Google.Protobuf.WriteContext output);
}
public interface IProtoBufMessage<out T> : IProtoBufMessage where T : IProtoBufMessage<T>
{
    public static abstract T ParseFrom(ref Google.Protobuf.ParseContext input);
}