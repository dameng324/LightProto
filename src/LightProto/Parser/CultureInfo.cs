using System.Buffers.Binary;
using System.Globalization;

namespace LightProto.Parser
{
    public sealed class CultureInfoProtoParser : IProtoParser<CultureInfo>
    {
        public static IProtoReader<CultureInfo> ProtoReader { get; } = new CultureInfoProtoReader();
        public static IProtoWriter<CultureInfo> ProtoWriter { get; } = new CultureInfoProtoWriter();

        sealed class CultureInfoProtoReader : IProtoReader, IProtoReader<CultureInfo>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public CultureInfo ParseFrom(ref ReaderContext input)
            {
                var name = input.ReadString();
                return new CultureInfo(name);
            }
        }

        sealed class CultureInfoProtoWriter : IProtoWriter, IProtoWriter<CultureInfo>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((CultureInfo)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (CultureInfo)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((CultureInfo)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(CultureInfo value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(CultureInfo value)
            {
                return CodedOutputStream.ComputeStringSize(value.Name);
            }

            public void WriteTo(ref WriterContext output, CultureInfo value)
            {
                output.WriteString(value.Name);
            }
        }
    }
}
