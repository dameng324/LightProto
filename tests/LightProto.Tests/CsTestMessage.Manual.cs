#pragma warning disable 1591, 0612, 3021, 8981

using Google.Protobuf.WellKnownTypes;
using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;

namespace Dameng.Protobuf.Extension.Tests
{
    [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
    public sealed partial class CsTestMessage : pb::IMessage<CsTestMessage>, pb::IBufferMessage
    {
        private pb::UnknownFieldSet _unknownFields;
        public static pb::MessageParser<CsTestMessage> Parser { get; } =
            new pb::MessageParser<CsTestMessage>(() => new CsTestMessage());

        // public static pbr::MessageDescriptor Descriptor {
        //   // get { return PackageReflection.Descriptor.MessageTypes[0]; }
        // }
        pbr::MessageDescriptor pb::IMessage.Descriptor
        {
            get { return null; }
        }

        public CsTestMessage()
        {
            StringField = string.Empty;
            Int32Field = default(int);
            Int32ArrayField = new();
            StringArrayField = new();
            BytesField = pb.ByteString.Empty;
            BoolField = default(bool);
            DoubleField = default(double);
            FloatField = default(float);
            Int64Field = default(long);
            UInt32Field = default(uint);
            UInt64Field = default(ulong);
            SInt32Field = default(int);
            SInt64Field = default(long);
            Fixed32Field = default(uint);
            Fixed64Field = default(ulong);
            SFixed32Field = default(int);
            SFixed64Field = default(long);
            MapField = new();
            EnumField = default(CsTestEnum);
            EnumArrayField = new();
            NestedMessageField = null;
            NestedMessageArrayField = new();
            TimestampField = null;
            DurationField = null;
            MapField2 = new();
            MapField3 = new();
            _unknownFields = null;
        }

        partial void OnConstruction();

        public CsTestMessage(CsTestMessage other)
            : this()
        {
            StringField = other.StringField;
            Int32Field = other.Int32Field;
            Int32ArrayField = other.Int32ArrayField.Clone();
            StringArrayField = other.StringArrayField.Clone();
            BytesField = other.BytesField;
            BoolField = other.BoolField;
            DoubleField = other.DoubleField;
            FloatField = other.FloatField;
            Int64Field = other.Int64Field;
            UInt32Field = other.UInt32Field;
            UInt64Field = other.UInt64Field;
            SInt32Field = other.SInt32Field;
            SInt64Field = other.SInt64Field;
            Fixed32Field = other.Fixed32Field;
            Fixed64Field = other.Fixed64Field;
            SFixed32Field = other.SFixed32Field;
            SFixed64Field = other.SFixed64Field;
            MapField = other.MapField.Clone();
            EnumField = other.EnumField;
            EnumArrayField = other.EnumArrayField.Clone();
            NestedMessageField =
                other.NestedMessageField != null ? other.NestedMessageField.Clone() : null;
            NestedMessageArrayField = other.NestedMessageArrayField.Clone();
            TimestampField = other.TimestampField != null ? other.TimestampField.Clone() : null;
            DurationField = other.DurationField != null ? other.DurationField.Clone() : null;
            MapField2 = other.MapField2.Clone();
            MapField3 = other.MapField3.Clone();
            _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        public CsTestMessage Clone()
        {
            return new CsTestMessage(this);
        }

        public override bool Equals(object other)
        {
            return Equals(other as CsTestMessage);
        }

        public bool Equals(CsTestMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (StringField != other.StringField)
                return false;
            if (Int32Field != other.Int32Field)
                return false;
            if (!Int32ArrayField.Equals(other.Int32ArrayField))
                return false;
            if (!StringArrayField.Equals(other.StringArrayField))
                return false;
            if (BytesField != other.BytesField)
                return false;
            if (BoolField != other.BoolField)
                return false;
            if (
                !pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(
                    DoubleField,
                    other.DoubleField
                )
            )
                return false;
            if (
                !pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(
                    FloatField,
                    other.FloatField
                )
            )
                return false;
            if (Int64Field != other.Int64Field)
                return false;
            if (UInt32Field != other.UInt32Field)
                return false;
            if (UInt64Field != other.UInt64Field)
                return false;
            if (SInt32Field != other.SInt32Field)
                return false;
            if (SInt64Field != other.SInt64Field)
                return false;
            if (Fixed32Field != other.Fixed32Field)
                return false;
            if (Fixed64Field != other.Fixed64Field)
                return false;
            if (SFixed32Field != other.SFixed32Field)
                return false;
            if (SFixed64Field != other.SFixed64Field)
                return false;
            if (!MapField.Equals(other.MapField))
                return false;
            if (EnumField != other.EnumField)
                return false;
            if (!EnumArrayField.Equals(other.EnumArrayField))
                return false;
            if (!object.Equals(NestedMessageField, other.NestedMessageField))
                return false;
            if (!NestedMessageArrayField.Equals(other.NestedMessageArrayField))
                return false;
            if (!object.Equals(TimestampField, other.TimestampField))
                return false;
            if (!object.Equals(DurationField, other.DurationField))
                return false;
            if (!MapField2.Equals(other.MapField2))
                return false;
            if (!MapField3.Equals(other.MapField3))
                return false;
            return Equals(_unknownFields, other._unknownFields);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            if (StringField.Length != 0)
                hash ^= StringField.GetHashCode();
            if (Int32Field != 0)
                hash ^= Int32Field.GetHashCode();
            hash ^= Int32ArrayField.GetHashCode();
            hash ^= StringArrayField.GetHashCode();
            if (BytesField.Length != 0)
                hash ^= BytesField.GetHashCode();
            if (BoolField != false)
                hash ^= BoolField.GetHashCode();
            if (DoubleField != 0D)
                hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(
                    DoubleField
                );
            if (FloatField != 0F)
                hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(
                    FloatField
                );
            if (Int64Field != 0L)
                hash ^= Int64Field.GetHashCode();
            if (UInt32Field != 0)
                hash ^= UInt32Field.GetHashCode();
            if (UInt64Field != 0UL)
                hash ^= UInt64Field.GetHashCode();
            if (SInt32Field != 0)
                hash ^= SInt32Field.GetHashCode();
            if (SInt64Field != 0L)
                hash ^= SInt64Field.GetHashCode();
            if (Fixed32Field != 0)
                hash ^= Fixed32Field.GetHashCode();
            if (Fixed64Field != 0UL)
                hash ^= Fixed64Field.GetHashCode();
            if (SFixed32Field != 0)
                hash ^= SFixed32Field.GetHashCode();
            if (SFixed64Field != 0L)
                hash ^= SFixed64Field.GetHashCode();
            hash ^= MapField.GetHashCode();
            if (EnumField != CsTestEnum.None)
                hash ^= EnumField.GetHashCode();
            hash ^= EnumArrayField.GetHashCode();
            if (NestedMessageField != null)
                hash ^= NestedMessageField.GetHashCode();
            hash ^= NestedMessageArrayField.GetHashCode();
            if (TimestampField != null)
                hash ^= TimestampField.GetHashCode();
            if (DurationField != null)
                hash ^= DurationField.GetHashCode();
            hash ^= MapField2.GetHashCode();
            hash ^= MapField3.GetHashCode();
            if (_unknownFields != null)
            {
                hash ^= _unknownFields.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return pb::JsonFormatter.ToDiagnosticString(this);
        }

        private static readonly pb::FieldCodec<int> _repeated_int32ArrayField_codec =
            pb::FieldCodec.ForInt32(26);
        private static readonly pb::FieldCodec<string> _repeated_StringArrayFieldcodec =
            pb::FieldCodec.ForString(34);
        private static readonly pb::FieldCodec<CsTestMessage> _repeated_NestedMessageArrayFieldcodec =
            pb::FieldCodec.ForMessage(178, CsTestMessage.Parser);
        private static readonly pbc::MapField<string, string>.Codec _map_MapFieldcodec =
            new pbc::MapField<string, string>.Codec(
                pb::FieldCodec.ForString(10, ""),
                pb::FieldCodec.ForString(18, ""),
                146
            );
        private static readonly pbc::MapField<string, string>.Codec _map_MapField2codec =
            new pbc::MapField<string, string>.Codec(
                pb::FieldCodec.ForString(10, ""),
                pb::FieldCodec.ForString(18, ""),
                234
            );

        private static readonly pbc::MapField<string, string>.Codec _map_MapField3codec =
            new pbc::MapField<string, string>.Codec(
                pb::FieldCodec.ForString(10, ""),
                pb::FieldCodec.ForString(18, ""),
                242
            );

        private static readonly pb::FieldCodec<CsTestEnum> _repeated_EnumArrayFieldcodec =
            pb::FieldCodec.ForEnum(162, x => (int)x, x => (CsTestEnum)x);

        public void WriteTo(pb::CodedOutputStream output)
        {
            output.WriteRawMessage(this);
        }

        void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
        {
            if (StringField.Length > 0)
            {
                output.WriteRawTag(10);
                output.WriteString(StringField);
            }
            if (Int32Field != 0)
            {
                output.WriteRawTag(16);
                output.WriteInt32(Int32Field);
            }
            Int32ArrayField.WriteTo(ref output, _repeated_int32ArrayField_codec);
            StringArrayField.WriteTo(ref output, _repeated_StringArrayFieldcodec);
            if (BytesField.Length > 0)
            {
                output.WriteRawTag(42);
                output.WriteBytes(BytesField);
            }
            if (BoolField != false)
            {
                output.WriteRawTag(48);
                output.WriteBool(BoolField);
            }
            if (DoubleField != 0D)
            {
                output.WriteRawTag(57);
                output.WriteDouble(DoubleField);
            }
            if (FloatField != 0F)
            {
                output.WriteRawTag(69);
                output.WriteFloat(FloatField);
            }
            if (Int64Field != 0L)
            {
                output.WriteRawTag(72);
                output.WriteInt64(Int64Field);
            }
            if (UInt32Field != 0)
            {
                output.WriteRawTag(80);
                output.WriteUInt32(UInt32Field);
            }
            if (UInt64Field != 0UL)
            {
                output.WriteRawTag(88);
                output.WriteUInt64(UInt64Field);
            }
            if (SInt32Field != 0)
            {
                output.WriteRawTag(96);
                output.WriteSInt32(SInt32Field);
            }
            if (SInt64Field != 0L)
            {
                output.WriteRawTag(104);
                output.WriteSInt64(SInt64Field);
            }
            if (Fixed32Field != 0)
            {
                output.WriteRawTag(117);
                output.WriteFixed32(Fixed32Field);
            }
            if (Fixed64Field != 0UL)
            {
                output.WriteRawTag(121);
                output.WriteFixed64(Fixed64Field);
            }
            if (SFixed32Field != 0)
            {
                output.WriteRawTag(133, 1);
                output.WriteSFixed32(SFixed32Field);
            }
            if (SFixed64Field != 0L)
            {
                output.WriteRawTag(137, 1);
                output.WriteSFixed64(SFixed64Field);
            }
            MapField?.WriteTo(ref output, _map_MapFieldcodec);
            if (EnumField != CsTestEnum.None)
            {
                output.WriteRawTag(152, 1);
                output.WriteEnum((int)EnumField);
            }
            EnumArrayField.WriteTo(ref output, _repeated_EnumArrayFieldcodec);
            if (NestedMessageField != null)
            {
                output.WriteRawTag(170, 1);
                output.WriteMessage(NestedMessageField);
            }
            NestedMessageArrayField.WriteTo(ref output, _repeated_NestedMessageArrayFieldcodec);
            if (TimestampField != null)
            {
                output.WriteRawTag(218, 1);
                output.WriteMessage(TimestampField);
            }
            if (DurationField != null)
            {
                output.WriteRawTag(226, 1);
                output.WriteMessage(DurationField);
            }
            MapField2?.WriteTo(ref output, _map_MapField2codec);
            MapField3?.WriteTo(ref output, _map_MapField3codec);
            if (_unknownFields != null)
            {
                _unknownFields.WriteTo(ref output);
            }
        }

        public int CalculateSize()
        {
            int size = 0;
            if (StringField.Length > 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(StringField);
            }
            if (Int32Field != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt32Size(Int32Field);
            }
            size += Int32ArrayField.CalculateSize(_repeated_int32ArrayField_codec);
            size += StringArrayField.CalculateSize(_repeated_StringArrayFieldcodec);
            if (BytesField.Length > 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeBytesSize(BytesField);
            }
            if (BoolField != false)
            {
                size += 1 + 1;
            }
            if (DoubleField != 0D)
            {
                size += 1 + 8;
            }
            if (FloatField != 0F)
            {
                size += 1 + 4;
            }
            if (Int64Field != 0L)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(Int64Field);
            }
            if (UInt32Field != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeUInt32Size(UInt32Field);
            }
            if (UInt64Field != 0UL)
            {
                size += 1 + pb::CodedOutputStream.ComputeUInt64Size(UInt64Field);
            }
            if (SInt32Field != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeSInt32Size(SInt32Field);
            }
            if (SInt64Field != 0L)
            {
                size += 1 + pb::CodedOutputStream.ComputeSInt64Size(SInt64Field);
            }
            if (Fixed32Field != 0)
            {
                size += 1 + 4;
            }
            if (Fixed64Field != 0UL)
            {
                size += 1 + 8;
            }
            if (SFixed32Field != 0)
            {
                size += 2 + 4;
            }
            if (SFixed64Field != 0L)
            {
                size += 2 + 8;
            }
            size += MapField.CalculateSize(_map_MapFieldcodec);
            if (EnumField != CsTestEnum.None)
            {
                size += 2 + pb::CodedOutputStream.ComputeEnumSize((int)EnumField);
            }
            size += EnumArrayField.CalculateSize(_repeated_EnumArrayFieldcodec);
            if (NestedMessageField != null)
            {
                size += 2 + pb::CodedOutputStream.ComputeMessageSize(NestedMessageField);
            }
            size += NestedMessageArrayField.CalculateSize(_repeated_NestedMessageArrayFieldcodec);
            if (TimestampField != null)
            {
                size += 2 + pb::CodedOutputStream.ComputeMessageSize(TimestampField);
            }
            if (DurationField != null)
            {
                size += 2 + pb::CodedOutputStream.ComputeMessageSize(DurationField);
            }
            size += MapField2.CalculateSize(_map_MapField2codec);
            size += MapField3.CalculateSize(_map_MapField3codec);
            if (_unknownFields != null)
            {
                size += _unknownFields.CalculateSize();
            }
            return size;
        }

        public void MergeFrom(CsTestMessage other)
        {
            if (other == null)
            {
                return;
            }
            if (other.StringField.Length != 0)
            {
                StringField = other.StringField;
            }
            if (other.Int32Field != 0)
            {
                Int32Field = other.Int32Field;
            }
            Int32ArrayField.Add(other.Int32ArrayField);
            StringArrayField.Add(other.StringArrayField);
            if (other.BytesField.Length != 0)
            {
                BytesField = other.BytesField;
            }
            if (other.BoolField != false)
            {
                BoolField = other.BoolField;
            }
            if (other.DoubleField != 0D)
            {
                DoubleField = other.DoubleField;
            }
            if (other.FloatField != 0F)
            {
                FloatField = other.FloatField;
            }
            if (other.Int64Field != 0L)
            {
                Int64Field = other.Int64Field;
            }
            if (other.UInt32Field != 0)
            {
                UInt32Field = other.UInt32Field;
            }
            if (other.UInt64Field != 0UL)
            {
                UInt64Field = other.UInt64Field;
            }
            if (other.SInt32Field != 0)
            {
                SInt32Field = other.SInt32Field;
            }
            if (other.SInt64Field != 0L)
            {
                SInt64Field = other.SInt64Field;
            }
            if (other.Fixed32Field != 0)
            {
                Fixed32Field = other.Fixed32Field;
            }
            if (other.Fixed64Field != 0UL)
            {
                Fixed64Field = other.Fixed64Field;
            }
            if (other.SFixed32Field != 0)
            {
                SFixed32Field = other.SFixed32Field;
            }
            if (other.SFixed64Field != 0L)
            {
                SFixed64Field = other.SFixed64Field;
            }
            MapField.MergeFrom(other.MapField);
            if (other.EnumField != CsTestEnum.None)
            {
                EnumField = other.EnumField;
            }
            EnumArrayField.Add(other.EnumArrayField);
            if (other.NestedMessageField != null)
            {
                if (NestedMessageField == null)
                {
                    NestedMessageField = new CsTestMessage();
                }
                NestedMessageField.MergeFrom(other.NestedMessageField);
            }
            NestedMessageArrayField.Add(other.NestedMessageArrayField);
            if (other.TimestampField != null)
            {
                if (TimestampField == null)
                {
                    TimestampField = new global::Google.Protobuf.WellKnownTypes.Timestamp();
                }
                TimestampField.MergeFrom(other.TimestampField);
            }
            if (other.DurationField != null)
            {
                if (DurationField == null)
                {
                    DurationField = new global::Google.Protobuf.WellKnownTypes.Duration();
                }
                DurationField.MergeFrom(other.DurationField);
            }
            MapField2.MergeFrom(other.MapField2);
            MapField3.MergeFrom(other.MapField3);
            _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
        }

        public void MergeFrom(pb::CodedInputStream input)
        {
            input.ReadRawMessage(this);
        }

        void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                if ((tag & 7) == 4)
                {
                    // Abort on any end group tag.
                    return;
                }
                switch (tag)
                {
                    default:
                        _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(
                            _unknownFields,
                            ref input
                        );
                        break;
                    case 10:
                    {
                        StringField = input.ReadString();
                        break;
                    }
                    case 16:
                    {
                        Int32Field = input.ReadInt32();
                        break;
                    }
                    case 26:
                    case 24:
                    {
                        Int32ArrayField ??= new();
                        Int32ArrayField.AddEntriesFrom(ref input, _repeated_int32ArrayField_codec);
                        break;
                    }
                    case 34:
                    {
                        StringArrayField ??= new();
                        StringArrayField.AddEntriesFrom(ref input, _repeated_StringArrayFieldcodec);
                        break;
                    }
                    case 42:
                    {
                        BytesField = input.ReadBytes();
                        break;
                    }
                    case 48:
                    {
                        BoolField = input.ReadBool();
                        break;
                    }
                    case 57:
                    {
                        DoubleField = input.ReadDouble();
                        break;
                    }
                    case 69:
                    {
                        FloatField = input.ReadFloat();
                        break;
                    }
                    case 72:
                    {
                        Int64Field = input.ReadInt64();
                        break;
                    }
                    case 80:
                    {
                        UInt32Field = input.ReadUInt32();
                        break;
                    }
                    case 88:
                    {
                        UInt64Field = input.ReadUInt64();
                        break;
                    }
                    case 96:
                    {
                        SInt32Field = input.ReadSInt32();
                        break;
                    }
                    case 104:
                    {
                        SInt64Field = input.ReadSInt64();
                        break;
                    }
                    case 117:
                    {
                        Fixed32Field = input.ReadFixed32();
                        break;
                    }
                    case 121:
                    {
                        Fixed64Field = input.ReadFixed64();
                        break;
                    }
                    case 133:
                    {
                        SFixed32Field = input.ReadSFixed32();
                        break;
                    }
                    case 137:
                    {
                        SFixed64Field = input.ReadSFixed64();
                        break;
                    }
                    case 146:
                    {
                        MapField ??= new();
                        MapField.AddEntriesFrom(ref input, _map_MapFieldcodec);
                        break;
                    }
                    case 152:
                    {
                        EnumField = (CsTestEnum)input.ReadEnum();
                        break;
                    }
                    case 162:
                    case 160:
                    {
                        EnumArrayField ??= new();
                        EnumArrayField.AddEntriesFrom(ref input, _repeated_EnumArrayFieldcodec);
                        break;
                    }
                    case 170:
                    {
                        if (NestedMessageField == null)
                        {
                            NestedMessageField = new CsTestMessage();
                        }
                        input.ReadMessage(NestedMessageField);
                        break;
                    }
                    case 178:
                    {
                        NestedMessageArrayField ??= new();
                        NestedMessageArrayField.AddEntriesFrom(
                            ref input,
                            _repeated_NestedMessageArrayFieldcodec
                        );
                        break;
                    }
                    case 218:
                    {
                        if (TimestampField == null)
                        {
                            TimestampField = new global::Google.Protobuf.WellKnownTypes.Timestamp();
                        }
                        input.ReadMessage(TimestampField);
                        break;
                    }
                    case 226:
                    {
                        if (DurationField == null)
                        {
                            DurationField = new global::Google.Protobuf.WellKnownTypes.Duration();
                        }
                        input.ReadMessage(DurationField);
                        break;
                    }
                    case 234:
                    {
                        MapField2 ??= new();
                        MapField2.AddEntriesFrom(ref input, _map_MapField2codec);
                        break;
                    }
                    case 242:
                    {
                        MapField3 ??= new();
                        MapField3.AddEntriesFrom(ref input, _map_MapField3codec);
                        break;
                    }
                }
            }
        }
    }
}
