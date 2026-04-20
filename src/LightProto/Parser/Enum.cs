using System.Runtime.CompilerServices;

namespace LightProto.Parser
{
    public sealed class EnumProtoParser<T> : IProtoParser<T>
        where T : Enum
    {
        public static IProtoReader<T> ProtoReader { get; } = new EnumProtoReader();
        public static IProtoWriter<T> ProtoWriter { get; } = new EnumProtoWriter();

        static readonly TypeCode UnderlyingTypeCode = Type.GetTypeCode(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int EnumToInt32(T value)
        {
            switch (UnderlyingTypeCode)
            {
                case TypeCode.SByte:
                    return Unsafe.As<T, sbyte>(ref value);
                case TypeCode.Byte:
                    return Unsafe.As<T, byte>(ref value);
                case TypeCode.Int16:
                    return Unsafe.As<T, short>(ref value);
                case TypeCode.UInt16:
                    return Unsafe.As<T, ushort>(ref value);
                case TypeCode.UInt32:
                    return (int)Unsafe.As<T, uint>(ref value);
                case TypeCode.Int64:
                    return (int)Unsafe.As<T, long>(ref value);
                case TypeCode.UInt64:
                    return (int)Unsafe.As<T, ulong>(ref value);
                default:
                    return Unsafe.As<T, int>(ref value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Int32ToEnum(int value)
        {
            switch (UnderlyingTypeCode)
            {
                case TypeCode.SByte:
                {
                    var v = (sbyte)value;
                    return Unsafe.As<sbyte, T>(ref v);
                }
                case TypeCode.Byte:
                {
                    var v = (byte)value;
                    return Unsafe.As<byte, T>(ref v);
                }
                case TypeCode.Int16:
                {
                    var v = (short)value;
                    return Unsafe.As<short, T>(ref v);
                }
                case TypeCode.UInt16:
                {
                    var v = (ushort)value;
                    return Unsafe.As<ushort, T>(ref v);
                }
                case TypeCode.UInt32:
                {
                    var v = (uint)value;
                    return Unsafe.As<uint, T>(ref v);
                }
                case TypeCode.Int64:
                {
                    var v = (long)value;
                    return Unsafe.As<long, T>(ref v);
                }
                case TypeCode.UInt64:
                {
                    var v = (ulong)value;
                    return Unsafe.As<ulong, T>(ref v);
                }
                default:
                    return Unsafe.As<int, T>(ref value);
            }
        }

        sealed class EnumProtoReader : IProtoReader, IProtoReader<T>
        {
            object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T ParseFrom(ref ReaderContext input)
            {
                var value = input.ReadEnum();
                return Int32ToEnum(value);
            }
        }

        sealed class EnumProtoWriter : IProtoWriter, IProtoWriter<T>
        {
            int IProtoWriter.CalculateSize(object value) => CalculateSize((T)value);

            void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (T)value);

            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;

            long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((T)value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public int CalculateSize(T value) => (int)CalculateLongSize(value);

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public long CalculateLongSize(T value)
            {
                return CodedOutputStream.ComputeEnumSize(EnumToInt32(value));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void WriteTo(ref WriterContext output, T value)
            {
                output.WriteEnum(EnumToInt32(value));
            }
        }
    }
}
