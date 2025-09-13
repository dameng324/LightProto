namespace LightProto.Parser;

struct MessageWrapper<T>
{
    public MessageWrapper(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public struct ProtoWriter : IProtoWriter<MessageWrapper<T>>
    {
        private readonly uint tag;
        public bool IsMessage => true;
        private IProtoWriter<T> ItemWriter;

        public ProtoWriter(uint tag, IProtoWriter<T> itemWriter)
        {
            this.tag = tag;
            ItemWriter = itemWriter;
        }

        public void WriteTo(ref WriterContext output, MessageWrapper<T> value)
        {
            if (ItemWriter is not ICollectionWriter)
            {
                output.WriteTag(tag);
            }

            ItemWriter.WriteTo(ref output, value.Value);
        }

        public int CalculateSize(MessageWrapper<T> value)
        {
            int size = 0;
            if (ItemWriter is not ICollectionWriter)
            {
                size += CodedOutputStream.ComputeRawVarint32Size(tag);
            }
            size += ItemWriter.CalculateSize(value.Value);
            return size;
        }
    }

    public struct ProtoReader : IProtoReader<MessageWrapper<T>>
    {
        public uint Tag { get; }

        public ProtoReader(uint tag, IProtoReader<T> itemReader)
        {
            Tag = tag;
            ItemReader = itemReader;
        }

        public bool IsMessage => true;
        private IProtoReader<T> ItemReader;

        public MessageWrapper<T> ParseFrom(ref ReaderContext input)
        {
            T _Property = default(T)!;
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                if ((tag & 7) == 4)
                {
                    break;
                }

                if (tag == this.Tag)
                {
                    _Property = ItemReader.ParseFrom(ref input);
                }
            }

            return new MessageWrapper<T>(_Property);
        }
    }
}