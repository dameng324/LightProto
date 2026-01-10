namespace LightProto.Parser
{
    public sealed class UriProtoParser : IProtoParser<Uri?>
    {
        public static IProtoReader<Uri?> ProtoReader { get; } = new UriProtoReader();
        public static IProtoWriter<Uri?> ProtoWriter { get; } = new UriProtoWriter();

        sealed class UriProtoReader : IProtoReader, IProtoReader<Uri?>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input)!;

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public Uri? ParseFrom(ref ReaderContext input)
            {
                var uriString = input.ReadString();
                if (string.IsNullOrEmpty(uriString))
                {
                    return null;
                }

                if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
                {
                    return uri;
                }

                return null;
            }
        }

        sealed class UriProtoWriter : IProtoWriter, IProtoWriter<Uri?>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((Uri?)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) =>
                WriteTo(ref output, (Uri?)value);

            public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
            public bool IsMessage => false;

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public int CalculateSize(Uri? value)
            {
                return CodedOutputStream.ComputeStringSize(value?.OriginalString ?? string.Empty);
            }

            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
            )]
            public void WriteTo(ref WriterContext output, Uri? value)
            {
                output.WriteString(value?.OriginalString ?? string.Empty);
            }
        }
    }
}
