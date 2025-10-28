using System;
using System.Collections.Concurrent;

namespace LightProto.Internal;

internal class SimpleObjectPool<T>
    where T : class
{
    private struct Element
    {
        internal T? Value;
    }

    // Storage for the pool objects. The first item is stored in a dedicated field because we
    // expect to be able to satisfy most requests from it.
    private T? m_firstItem;
    private readonly Element[] m_items;
    private readonly Func<T> _objectFactory;
    private readonly Action<T>? _resetAction;

    public SimpleObjectPool(Func<T> objectFactory)
        : this(objectFactory, null, Environment.ProcessorCount * 2) { }

    public SimpleObjectPool(Func<T> objectFactory, Action<T>? resetAction)
        : this(objectFactory, resetAction, Environment.ProcessorCount * 2) { }

    public SimpleObjectPool(Func<T> objectFactory, Action<T>? resetAction, int size)
    {
        _objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
        _resetAction = resetAction;

        m_items = new Element[size - 1];
    }

    public T Get()
    {
        // PERF: Examine the first element. If that fails, AllocateSlow will look at the remaining elements.
        // Note that the initial read is optimistically not synchronized. That is intentional.
        // We will interlock only when we have a candidate. in a worst case we may miss some
        // recently returned objects. Not a big deal.
        T? inst = m_firstItem;
        if (inst == null || inst != Interlocked.CompareExchange(ref m_firstItem, null, inst))
        {
            inst = GetSlow();
        }

        return inst;
    }

    private T GetSlow()
    {
        var items = m_items;
        for (int i = 0; i < items.Length; i++)
        {
            // Note that the initial read is optimistically not synchronized. That is intentional.
            // We will interlock only when we have a candidate. in a worst case we may miss some
            // recently returned objects. Not a big deal.
            T? inst = items[i].Value;
            if (inst != null)
            {
                if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
                {
                    return inst;
                }
            }
        }

        return _objectFactory();
    }

    /// <summary>
    /// Returns objects to the pool.
    /// </summary>
    /// <remarks>
    /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
    /// Note that Free will try to store recycled objects close to the start thus statistically
    /// reducing how far we will typically search in Allocate.
    /// </remarks>
    public void Return(T item)
    {
        if (m_firstItem == null)
        {
            // Intentionally not using interlocked here.
            // In a worst case scenario two objects may be stored into same slot.
            // It is very unlikely to happen and will only mean that one of the objects will get collected.
            _resetAction?.Invoke(item);
            m_firstItem = item;
        }
        else
        {
            FreeSlow(item);
        }
    }

    private void FreeSlow(T item)
    {
        var items = m_items;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].Value == null)
            {
                // Intentionally not using interlocked here.
                // In a worst case scenario two objects may be stored into same slot.
                // It is very unlikely to happen and will only mean that one of the objects will get collected.
                _resetAction?.Invoke(item);
                items[i].Value = item;
                break;
            }
        }
    }
}
