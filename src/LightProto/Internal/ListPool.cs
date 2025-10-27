using System;
using System.Collections.Concurrent;

namespace LightProto.Internal;

internal class SimpleObjectPool<T>
    where T : class
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectFactory;
    private readonly Action<T>? _resetAction;

    public SimpleObjectPool(Func<T> objectFactory, Action<T>? resetAction = null)
    {
        _objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
        _resetAction = resetAction;

        _objects = new ConcurrentBag<T>();
    }

    public T Get()
    {
        if (_objects.TryTake(out var item))
        {
            return item;
        }

        return _objectFactory();
    }

    public void Return(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _resetAction?.Invoke(item);
        _objects.Add(item);
    }
}
