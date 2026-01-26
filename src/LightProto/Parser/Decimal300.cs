using System.Buffers.Binary;
using System.Globalization;

namespace LightProto.Parser
{
    public sealed class Decimal300ProtoParser : IProtoParser<Decimal>
    {
        public static IProtoReader<Decimal> ProtoReader { get; } = new Decimal300ProtoReader();
        public static IProtoWriter<Decimal> ProtoWriter { get; } = new Decimal300ProtoWriter();

        sealed class Decimal300ProtoReader : IProtoReader, IProtoReader<Decimal>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public Decimal ParseFrom(ref ReaderContext input)
            {
                var str = input.ReadString();
                return Decimal.Parse(str);
            }
        }

        sealed class Decimal300ProtoWriter : IProtoWriter, IProtoWriter<Decimal>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Decimal)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Decimal)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((Decimal)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(Decimal value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(Decimal value)
            {
                return CodedOutputStream.ComputeStringSize(value.ToString("G"));
            }

            public void WriteTo(ref WriterContext output, Decimal value)
            {
                output.WriteString(value.ToString("G"));
            }
        }
    }
}
