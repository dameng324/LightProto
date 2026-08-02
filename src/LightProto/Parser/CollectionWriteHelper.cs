namespace LightProto.Parser
{
    internal static class CollectionWriteHelper<T>
    {
        private static readonly bool ElementCanBeNull = default(T) is null;

        public static long CalculateAllItemSize(ReadOnlySpan<T> values, IProtoWriter<T> itemWriter)
        {
            long size = 0;
            foreach (var item in values)
            {
                ThrowIfNull(item);
                size += itemWriter.CalculateLongMessageSize(item);
            }
            return size;
        }

        public static void WritePacked(ref WriterContext output, ReadOnlySpan<T> values, IProtoWriter<T> itemWriter)
        {
            foreach (var item in values)
            {
                itemWriter.WriteMessageTo(ref output, item);
            }
        }

        public static void WriteUnpacked(ref WriterContext output, ReadOnlySpan<T> values, uint tag, IProtoWriter<T> itemWriter)
        {
            foreach (var item in values)
            {
                ThrowIfNull(item);
                output.WriteTag(tag);
                itemWriter.WriteMessageTo(ref output, item);
            }
        }

        public static void ThrowIfNull(T item)
        {
            if (ElementCanBeNull && item is null)
                throw new InvalidOperationException("Sequence contained null element");
        }
    }
}
