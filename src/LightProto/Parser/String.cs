namespace LightProto.Parser
{
    public sealed class StringProtoParser : IProtoParser<string>
    {
        public static IProtoReader<string> ProtoReader { get; } = new StringProtoReader();
        public static IProtoWriter<string> ProtoWriter { get; } = new StringProtoWriter();

        sealed class StringProtoReader : IProtoReader, IProtoReader<string>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public string ParseFrom(ref ReaderContext input)
            {
                return input.ReadString();
            }
        }

        sealed class StringProtoWriter : IProtoWriter, IProtoWriter<string>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((string)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (string)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((string)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(string value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(string value)
            {
                return CodedOutputStream.ComputeStringSize(value);
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, string value)
            {
                output.WriteString(value);
            }
        }
    }
}
