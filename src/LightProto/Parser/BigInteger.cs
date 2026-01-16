using System.Numerics;

namespace LightProto.Parser
{
    public sealed class BigIntegerProtoParser : IProtoParser<BigInteger>
    {
        public static IProtoReader<BigInteger> ProtoReader { get; } = new BigIntegerProtoReader();
        public static IProtoWriter<BigInteger> ProtoWriter { get; } = new BigIntegerProtoWriter();

        sealed class BigIntegerProtoReader : IProtoReader, IProtoReader<BigInteger>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public BigInteger ParseFrom(ref ReaderContext input)
            {
                var length = input.ReadLength();
                var bytes = ParsingPrimitives.ReadRawBytes(ref input.buffer, ref input.state, length);
                return new BigInteger(bytes);
            }
        }

        sealed class BigIntegerProtoWriter : IProtoWriter, IProtoWriter<BigInteger>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public int CalculateSize(object value) => CalculateSize((BigInteger)value);

            public void WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (BigInteger)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(BigInteger value)
            {
#if NET7_0_OR_GREATER
                var byteCount = value.GetByteCount();
#else
                var byteCount = value.ToByteArray().Length;
#endif
                return CodedOutputStream.ComputeLengthSize(byteCount) + byteCount;
            }

            public void WriteTo(ref WriterContext output, BigInteger value)
            {
                var bytes = value.ToByteArray();
                output.WriteLength(bytes.Length);
                WritingPrimitives.WriteRawBytes(ref output.buffer, ref output.state, bytes);
            }
        }
    }
}
