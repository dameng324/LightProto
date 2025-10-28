using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LightProto.Parser;

public sealed class StackProtoWriter<T> : IEnumerableProtoWriter<Stack<T>, T>
{
    public StackProtoWriter(IProtoWriter<T> itemWriter, uint tag, int itemFixedSize)
        : base(itemWriter, tag, static collection => collection.Count, itemFixedSize) { }
}

public sealed class StackProtoReader<T> : IEnumerableProtoReader<Stack<T>, T>
{
    public StackProtoReader(IProtoReader<T> itemReader, int itemFixedSize)
        : base(
            itemReader,
            itemFixedSize,
            static items =>
            {
                items.Reverse();
                return new(items);
            },
            new()
        ) { }
}
