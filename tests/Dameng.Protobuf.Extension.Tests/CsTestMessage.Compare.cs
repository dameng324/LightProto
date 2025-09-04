
    
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) 
      {
          if ((tag & 7) == 4) {
            // Abort on any end group tag.
            return;
          }
          switch(tag) 
          {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
              break;
            
                case 10:{StringField = input.ReadString();break;}
            case 16:{Int32Field = input.ReadInt32();break;}
            case 26:{Int32ArrayField.AddEntriesFrom(ref input,_Int32ArrayField_codec);break;}
            case 34:{StringArrayField.AddEntriesFrom(ref input,_StringArrayField_codec);break;}
            case 42:{BytesField = input.ReadBytes();break;}
            case 48:{BoolField = input.ReadBool();break;}
            case 57:{DoubleField = input.ReadDouble();break;}
            case 69:{FloatField = input.ReadFloat();break;}
            case 72:{Int64Field = input.ReadInt64();break;}
            case 80:{UInt32Field = input.ReadUInt32();break;}
            case 88:{UInt64Field = input.ReadUInt64();break;}
            case 96:{SInt32Field = input.ReadSInt32();break;}
            case 104:{SInt64Field = input.ReadSInt64();break;}
            case 117:{Fixed32Field = input.ReadFixed32();break;}
            case 121:{Fixed64Field = input.ReadFixed64();break;}
            case 133:{SFixed32Field = input.ReadSFixed32();break;}
            case 137:{SFixed64Field = input.ReadSFixed64();break;}
            case 146:{MapField.AddEntriesFrom(ref input,_MapField_codec);break;}
            case 152:{EnumField = (Dameng.Protobuf.Extension.Tests.CsTestEnum)input.ReadEnum();break;}
            case 162:{EnumArrayField.AddEntriesFrom(ref input,_EnumArrayField_codec);break;}
            case 170:{if(NestedMessageField==null) NestedMessageField=new Dameng.Protobuf.Extension.Tests.CsTestMessage(); input.ReadMessage(NestedMessageField);break;}
            case 178:{NestedMessageArrayField.AddEntriesFrom(ref input,_NestedMessageArrayField_codec);break;}
            case 218:{if(TimestampField==null) TimestampField=new Google.Protobuf.WellKnownTypes.Timestamp(); input.ReadMessage(TimestampField);break;}
            case 226:{if(DurationField==null) DurationField=new Google.Protobuf.WellKnownTypes.Duration(); input.ReadMessage(DurationField);break;}
            case 234:{MapField2.AddEntriesFrom(ref input,_MapField2_codec);break;}
            case 242:{MapField3.AddEntriesFrom(ref input,_MapField3_codec);break;}
          }
      }
    }
}
