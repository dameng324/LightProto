

namespace Dameng.Protobuf;

public interface IProtoParser<T>
{
    public static abstract IProtoReader<T> Reader { get; }
    public static abstract IProtoWriter<T> Writer { get; }
}

public interface IProtoMessage<T> : IProtoParser<T> where T : IProtoParser<T>;

public interface IProtoReader<out T>
{
    public T ParseFrom(ref ReaderContext input);
}

public interface IProtoWriter<in T>
{
    public int CalculateSize(T value);
    public void WriteTo(ref WriterContext output, T value);
}

public interface IProtoMessageReader<out T> : IProtoReader<T>
{
    public T ParseMessageFrom(ref ReaderContext input)
    {
        int length = ParsingPrimitives.ParseLength(ref input.buffer, ref input.state);
        if (input.state.recursionDepth >= input.state.recursionLimit)
        {
            throw InvalidProtocolBufferException.RecursionLimitExceeded();
        }
        int oldLimit = SegmentedBufferHelper.PushLimit(ref input.state, length);
        ++input.state.recursionDepth;
        var message = ParseFrom(ref input);
        
        if (input.state.lastTag != 0)
        {
            throw InvalidProtocolBufferException.MoreDataAvailable();
        }
        // Check that we've read exactly as much data as expected.
        if (!SegmentedBufferHelper.IsReachedLimit(ref input.state))
        {
            throw InvalidProtocolBufferException.TruncatedMessage();
        }
        --input.state.recursionDepth;
        SegmentedBufferHelper.PopLimit(ref input.state, oldLimit);
        return message;
    }
}
public interface IProtoMessageWriter<in T>: IProtoWriter<T>
{
    public int CalculateMessageSize(T value)
    {
        var size = CalculateSize(value);
        return CodedOutputStream.ComputeLengthSize(size) + size;
    }

    public void WriteMessageTo(ref WriterContext output, T value)
    {
        var size = CalculateSize(value);
        output.WriteLength(size);
        WriteTo(ref output, value);
    }
}