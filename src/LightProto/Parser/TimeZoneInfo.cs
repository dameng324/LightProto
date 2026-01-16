using System.Buffers.Binary;
using System.Globalization;

namespace LightProto.Parser
{
    public sealed class TimeZoneInfoProtoParser : IProtoParser<TimeZoneInfo>
    {
        public static IProtoReader<TimeZoneInfo> ProtoReader { get; } = new TimeZoneInfoProtoReader();
        public static IProtoWriter<TimeZoneInfo> ProtoWriter { get; } = new TimeZoneInfoProtoWriter();

        sealed class TimeZoneInfoProtoReader : IProtoReader, IProtoReader<TimeZoneInfo>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public TimeZoneInfo ParseFrom(ref ReaderContext input)
            {
                var str = input.ReadString();
                return TimeZoneInfo.FromSerializedString(str);
            }
        }

        sealed class TimeZoneInfoProtoWriter : IProtoWriter, IProtoWriter<TimeZoneInfo>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((TimeZoneInfo)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (TimeZoneInfo)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            public int CalculateSize(TimeZoneInfo value)
            {
                return CodedOutputStream.ComputeStringSize(value.ToSerializedString());
            }

            public void WriteTo(ref WriterContext output, TimeZoneInfo value)
            {
                output.WriteString(value.ToSerializedString());
            }
        }
    }
}
