using System.Diagnostics.CodeAnalysis;
using LightProto.Parser;

namespace LightProto;

public class RuntimeProtoWriter<T> : IProtoWriter, IProtoWriter<T>
{
    private record ProtoMember
    {
        public ProtoMember(uint tag, IProtoWriter writer, Func<T, object> getter)
        {
            Tag = tag;
            Writer = writer;
            Getter = getter;
        }

        public uint Tag { get; set; }
        public IProtoWriter Writer { get; set; }
        public Func<T, object> Getter { get; }
    }

    private readonly List<ProtoMember> _members = new();

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        TValue>(int fieldNumber, Func<T, TValue> getter) => AddMember(fieldNumber, getter, Serializer.GetProtoWriter<TValue>());

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        Type type,
        int fieldNumber,
        Func<T, object> getter
    ) => AddMember(type, fieldNumber, getter, Serializer.GetProtoWriter(type));

    public void AddMember<TValue>(int fieldNumber, Func<T, TValue> getter, IProtoWriter<TValue> writer)
    {
        AddMember(typeof(TValue), fieldNumber, x => getter(x)!, (IProtoWriter)writer);
    }

    public void AddMember(Type type, int fieldNumber, Func<T, object> getter, IProtoWriter writer)
    {
        uint tag;
        if (writer is ICollectionWriter collectionWriter)
        {
            tag = collectionWriter.Tag = WireFormat.MakeTag(fieldNumber, collectionWriter.ItemWireType);
        }
        else
        {
            tag = WireFormat.MakeTag(fieldNumber, writer.WireType);
        }
        var member = new ProtoMember(tag, writer, x => getter(x)!);
        _members.Add(member);
    }

    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => true;

    public int CalculateSize(T value)
    {
        int size = 0;
        foreach (var member in _members)
        {
            if (member.Writer is not ICollectionWriter)
            {
                size += CodedOutputStream.ComputeUInt32Size(member.Tag);
            }
            size += member.Writer.CalculateSize(member.Getter(value));
        }

        return size;
    }

    public void WriteTo(ref WriterContext output, T value)
    {
        foreach (var member in _members)
        {
            if (member.Writer is not ICollectionWriter)
            {
                output.WriteTag(member.Tag);
            }
            member.Writer.WriteMessageTo(ref output, member.Getter(value));
        }
    }

    int IProtoWriter.CalculateSize(object value) => CalculateSize((T)value);

    void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (T)value);
}
