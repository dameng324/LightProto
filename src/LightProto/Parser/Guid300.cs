using System.Buffers.Binary;

namespace LightProto.Parser
{
    public sealed class Guid300ProtoParser : IProtoParser<Guid>
    {
        public static IProtoReader<Guid> ProtoReader { get; } = new Guid300ProtoReader();
        public static IProtoWriter<Guid> ProtoWriter { get; } = new Guid300ProtoWriter();

        sealed class Guid300ProtoReader : IProtoReader, IProtoReader<Guid>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public Guid ParseFrom(ref ReaderContext input)
            {
                var str = input.ReadString();
                return Guid.Parse(str);
            }
        }

        sealed class Guid300ProtoWriter : IProtoWriter, IProtoWriter<Guid>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Guid)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (Guid)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public int CalculateSize(Guid value)
            {
                return CodedOutputStream.ComputeStringSize(value.ToString());
            }

            public void WriteTo(ref WriterContext output, Guid value)
            {
                output.WriteString(value.ToString());
            }
        }
    }
}
