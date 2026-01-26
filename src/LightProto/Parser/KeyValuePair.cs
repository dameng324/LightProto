namespace LightProto.Parser
{
    public class KeyValuePairProtoReader<TKey, TValue> : IProtoReader, IProtoReader<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        public bool IsMessage => true;

        object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

        private readonly IProtoReader<TKey> _keyReader;
        private readonly IProtoReader<TValue> _valueReader;
        private readonly uint _keyTag;
        private readonly uint _keyTag2;
        private readonly uint _valueTag;
        private readonly uint _valueTag2;

        public KeyValuePairProtoReader(IProtoReader<TKey> keyReader, IProtoReader<TValue> valueReader)
        {
            _keyReader = keyReader;
            _valueReader = valueReader;
            if (keyReader is ICollectionReader keyCollectionReader)
            {
                _keyTag = WireFormat.MakeTag(1, keyCollectionReader.ItemWireType);
                _keyTag2 = WireFormat.MakeTag(1, WireFormat.WireType.LengthDelimited);
            }
            else
            {
                _keyTag = WireFormat.MakeTag(1, keyReader.WireType);
                _keyTag2 = _keyTag;
            }

            if (valueReader is ICollectionReader valueCollectionReader)
            {
                _valueTag = WireFormat.MakeTag(2, valueCollectionReader.ItemWireType);
                _valueTag2 = WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited);
            }
            else
            {
                _valueTag = WireFormat.MakeTag(2, valueReader.WireType);
                _valueTag2 = _valueTag;
            }
        }

        public KeyValuePair<TKey, TValue> ParseFrom(ref ReaderContext ctx)
        {
            TKey key = default!;
            TValue value = default!;
            uint tag;
            while ((tag = ctx.ReadTag()) != 0)
            {
                if ((tag & 7) == 4)
                {
                    break;
                }

                if (tag == _keyTag || tag == _keyTag2)
                {
                    key = _keyReader is ICollectionReader ? _keyReader.ParseFrom(ref ctx) : _keyReader.ParseMessageFrom(ref ctx);
                }
                else if (tag == _valueTag || tag == _valueTag2)
                {
                    value = _valueReader is ICollectionReader ? _valueReader.ParseFrom(ref ctx) : _valueReader.ParseMessageFrom(ref ctx);
                }
                else
                {
                    continue;
                }
            }

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }

    public class KeyValuePairProtoWriter<TKey, TValue> : IProtoWriter, IProtoWriter<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        public bool IsMessage => true;
        public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;

        int IProtoWriter.CalculateSize(object value) => CalculateSize((KeyValuePair<TKey, TValue>)value);

        long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((KeyValuePair<TKey, TValue>)value);

        void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (KeyValuePair<TKey, TValue>)value);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int CalculateSize(KeyValuePair<TKey, TValue> value)
        {
            var longSize = CalculateLongSize(value);
            if (longSize > int.MaxValue)
            {
                throw new OverflowException("Calculated size exceeds Int32.MaxValue");
            }
            return (int)longSize;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public long CalculateLongSize(KeyValuePair<TKey, TValue> value)
        {
            long size = 0;
            if (_keyWriter is ICollectionWriter)
            {
                size += _keyWriter.CalculateLongSize(value.Key);
            }
            else
            {
                size += CodedOutputStream.ComputeRawVarint32Size(_keyTag);
                size += _keyWriter.CalculateLongMessageSize(value.Key);
            }

            if (_valueWriter is ICollectionWriter)
            {
                size += _valueWriter.CalculateLongSize(value.Value);
            }
            else
            {
                size += CodedOutputStream.ComputeRawVarint32Size(_valueTag);
                size += _valueWriter.CalculateLongMessageSize(value.Value);
            }

            return size;
        }

        public void WriteTo(ref WriterContext output, KeyValuePair<TKey, TValue> pair)
        {
            if (_keyWriter is ICollectionWriter)
            {
                _keyWriter.WriteTo(ref output, pair.Key);
            }
            else
            {
                output.WriteTag(_keyTag);
                _keyWriter.WriteMessageTo(ref output, pair.Key);
            }

            if (_valueWriter is ICollectionWriter)
            {
                _valueWriter.WriteTo(ref output, pair.Value);
            }
            else
            {
                output.WriteTag(_valueTag);
                _valueWriter.WriteMessageTo(ref output, pair.Value);
            }
        }

        private readonly IProtoWriter<TKey> _keyWriter;
        private readonly IProtoWriter<TValue> _valueWriter;
        private readonly uint _keyTag;
        private readonly uint _valueTag;

        public KeyValuePairProtoWriter(IProtoWriter<TKey> keyWriter, IProtoWriter<TValue> valueWriter)
        {
            _keyWriter = keyWriter;
            _valueWriter = valueWriter;
            _keyTag = WireFormat.MakeTag(1, keyWriter.WireType);
            _valueTag = WireFormat.MakeTag(2, valueWriter.WireType);
        }
    }
}
