namespace LightProto.Generator;

public static class DisposableExtensions
{
    public static T AddTo<T>(this T disposable, StackDisposable stack)
        where T : IDisposable
    {
        stack.Add(disposable);
        return disposable;
    }
}
