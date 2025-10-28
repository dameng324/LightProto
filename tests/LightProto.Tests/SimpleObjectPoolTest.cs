using LightProto.Internal;

namespace LightProto.Tests;

public class SimpleObjectPoolTest
{
    [Test]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SimpleObjectPool<object>(null!));
    }

    [Test]
    public async Task Get_ReturnsNewObject_WhenPoolIsEmpty()
    {
        var pool = new SimpleObjectPool<List<int>>(() => new List<int>());
        var item = pool.Get();

        await Assert.That(item).IsNotNull();
        await Assert.That(item).IsTypeOf<List<int>>();
    }

    [Test]
    public async Task Get_ReturnsCachedObject_WhenAvailable()
    {
        var factory = new List<int>();
        var createCount = 0;
        var pool = new SimpleObjectPool<List<int>>(() =>
        {
            createCount++;
            return new List<int>();
        });

        var first = pool.Get();
        pool.Return(first);
        var second = pool.Get();

        await Assert.That(createCount).IsEqualTo(1);
        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public void Return_WithNullItem_ThrowsArgumentNullException()
    {
        var pool = new SimpleObjectPool<List<int>>(() => new List<int>());
        Assert.Throws<ArgumentNullException>(() => pool.Return(null!));
    }

    [Test]
    public async Task Return_ExecutesResetAction()
    {
        var wasReset = false;
        var pool = new SimpleObjectPool<List<int>>(
            () => new List<int>(),
            list =>
            {
                list.Clear();
                wasReset = true;
            }
        );

        var item = pool.Get();
        item.Add(1);
        pool.Return(item);

        await Assert.That(wasReset).IsTrue();
        await Assert.That(item).IsEmpty();
    }

    [Test]
    public async Task Return_RespectsMaxSize()
    {
        var maxSize = 2;
        var createCount = 0;
        var pool = new SimpleObjectPool<object>(() =>
        {
            createCount++;
            return new object();
        });

        // Get and return maxSize + 1 objects
        var objects = new List<object>();
        for (int i = 0; i < maxSize + 1; i++)
        {
            objects.Add(pool.Get());
        }

        // Return all objects
        foreach (var obj in objects)
        {
            pool.Return(obj);
        }

        // Get maxSize + 1 objects again
        var newObjects = new List<object>();
        for (int i = 0; i < maxSize + 1; i++)
        {
            newObjects.Add(pool.Get());
        }

        // We should have created maxSize + 2 objects in total
        // maxSize + 1 initially, and 1 more because one was discarded due to pool size
        await Assert.That(createCount).IsEqualTo(maxSize + 2);
    }

    [Test]
    public void MultipleThreads_CanAccessPool_Safely()
    {
        var pool = new SimpleObjectPool<List<int>>(() => new List<int>());
        var actions = new List<Action>();
        var iterations = 1000;

        for (int i = 0; i < iterations; i++)
        {
            actions.Add(() =>
            {
                var item = pool.Get();
                pool.Return(item);
            });
        }

        Parallel.ForEach(actions, action => action());
    }
}
