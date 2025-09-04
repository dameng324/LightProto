using Google.Protobuf;
using Google.Protobuf.Collections;

namespace Dameng.Protobuf.Extension;

public static class Extensions
{
    public static RepeatedField<T> ToRepeatedField<T>(this IEnumerable<T> source)
    {
        return new RepeatedField<T> { source };
    }

    public static ByteString ToByteString(this Span<byte> source)
    {
        return ByteString.CopyFrom(source);
    }
}