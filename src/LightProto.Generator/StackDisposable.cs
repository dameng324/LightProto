namespace LightProto.Generator;

public sealed class StackDisposable : IDisposable
{
    private readonly Stack<IDisposable> _disposables = new();

    public void Add(IDisposable writerDisposable)
    {
        _disposables.Push(writerDisposable);
    }

    public void Dispose()
    {
        while (_disposables.Count > 0)
        {
            _disposables.Pop().Dispose();
        }
    }
}
