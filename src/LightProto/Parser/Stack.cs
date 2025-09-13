using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LightProto.Parser;

public sealed class StackProtoWriter<T> : IEnumerableProtoWriter<Stack<T>, T>
{
    public StackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize,bool isPacked)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize,isPacked) { }
}

public sealed class StackProtoReader<T> : IEnumerableProtoReader<Stack<T>, T>
{
    public StackProtoReader(IProtoReader<T> itemReader, uint tag, int itemFixedSize,bool isPacked)
        : base(
            itemReader,
            tag,
            static (size) => new Stack<T>(size),
            static (collection,item) =>
        {
             collection.Push(item);
             return collection;
        },
            itemFixedSize,
            isPacked,
            ReverseStack
        ) { }

#if NET9_0_OR_GREATER
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
#else
    private static readonly Func<Stack<T>, T[]> _getArray;
    private static readonly Func<Stack<T>, int> _getSize;
    [DynamicDependency("_array", "System.Collections.Generic.Stack`1", "System.Collections")]
    [DynamicDependency("_size", "System.Collections.Generic.Stack`1", "System.Collections")]
    static StackProtoReader()
    {
        var stackType = typeof(Stack<T>);
        // 获取 _array 字段
        var arrayField = stackType.GetField(
            "_array",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        var sizeField = stackType.GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance);

        if (arrayField == null || sizeField == null)
            throw new MissingFieldException("Stack<T> 内部结构可能变更，找不到 _array 或 _size");

        // 参数
        var param = Expression.Parameter(stackType, "stack");

        // _getArray 委托
        _getArray = Expression
            .Lambda<Func<Stack<T>, T[]>>(Expression.Field(param, arrayField), param)
            .Compile();

        // _getSize 委托
        _getSize = Expression
            .Lambda<Func<Stack<T>, int>>(Expression.Field(param, sizeField), param)
            .Compile();
    }

    public static Stack<T> ReverseStack(Stack<T> stack)
    {
        var arr = _getArray(stack);
        var size = _getSize(stack);
        Array.Reverse(arr, 0, size);
        return stack;
    }
#endif
}
