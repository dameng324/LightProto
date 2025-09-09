using System.Runtime.CompilerServices;

namespace Dameng.Protobuf.Extension;

public class StackProtoWriter<T> : IEnumerableProtoWriter<Stack<T>, T>
{
    public StackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public class StackProtoReader<T> : IEnumerableProtoReader<Stack<T>, T>
{
    public StackProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize)
        : base(
            itemReader,
            tag,
            static (size) => new Stack<T>(size),
            static (collection, item) => collection.Push(item),
            itemFixedSize,
            ReverseStack
        ) { }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    static extern ref T[] GetArray(Stack<T> stack);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
    static extern ref int GetSize(Stack<T> stack);

    static Stack<T> ReverseStack(Stack<T> stack)
    {
        var arr = GetArray(stack);
        var size = GetSize(stack);
        Array.Reverse(arr, 0, size);
        return stack;
    }
}
