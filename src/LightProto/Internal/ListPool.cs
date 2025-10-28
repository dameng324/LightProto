namespace LightProto.Internal;

internal static class ListPool<TItem>
{
    public static readonly SimpleObjectPool<List<TItem>> Default = new(
        static () => new(),
        static list => list.Clear()
    );
}
