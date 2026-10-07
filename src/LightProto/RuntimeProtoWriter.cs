using System.Diagnostics.CodeAnalysis;
using LightProto.Parser;

namespace LightProto;

public class RuntimeProtoWriter<T> : IProtoWriter, IProtoWriter<T>
{
    private abstract class ProtoMember
    {
        protected ProtoMember(uint tag, bool writeTag)
        {
            Tag = tag;
            WriteTag = writeTag;
        }

        public uint Tag { get; }
        public bool WriteTag { get; }
        public abstract long CalculateLongSize(T value);
        public abstract void WriteTo(ref WriterContext output, T value);
    }

    private sealed class ProtoMember<TValue> : ProtoMember
    {
        private readonly IProtoWriter<TValue> _writer;
        private readonly Func<T, TValue> _getter;

        public ProtoMember(uint tag, bool writeTag, IProtoWriter<TValue> writer, Func<T, TValue> getter)
            : base(tag, writeTag)
        {
            _writer = writer;
            _getter = getter;
        }

        public override long CalculateLongSize(T value) => _writer.CalculateLongMessageSize(_getter(value));

        public override void WriteTo(ref WriterContext output, T value)
        {
            if (WriteTag)
            {
                output.WriteTag(Tag);
            }
            _writer.WriteMessageTo(ref output, _getter(value));
        }
    }

    private sealed class ObjectProtoMember : ProtoMember
    {
        private readonly IProtoWriter _writer;
        private readonly Func<T, object> _getter;

        public ObjectProtoMember(uint tag, bool writeTag, IProtoWriter writer, Func<T, object> getter)
            : base(tag, writeTag)
        {
            _writer = writer;
            _getter = getter;
        }

        public override long CalculateLongSize(T value) => _writer.CalculateLongMessageSize(_getter(value));

        public override void WriteTo(ref WriterContext output, T value)
        {
            if (WriteTag)
            {
                output.WriteTag(Tag);
            }
            _writer.WriteMessageTo(ref output, _getter(value));
        }
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
        uint tag = writer is ICollectionWriter collectionWriter
            ? collectionWriter.Tag = WireFormat.MakeTag(fieldNumber, collectionWriter.ItemWireType)
            : WireFormat.MakeTag(fieldNumber, writer.WireType);
        _members.Add(new ProtoMember<TValue>(tag, writer is not ICollectionWriter, writer, getter));
    }

    public void AddMember(Type type, int fieldNumber, Func<T, object> getter, IProtoWriter writer)
    {
        uint tag = writer is ICollectionWriter collectionWriter
            ? collectionWriter.Tag = WireFormat.MakeTag(fieldNumber, collectionWriter.ItemWireType)
            : WireFormat.MakeTag(fieldNumber, writer.WireType);
        var member = new ObjectProtoMember(tag, writer is not ICollectionWriter, writer, getter);
        _members.Add(member);
    }

    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => true;

    public int CalculateSize(T value)
    {
        var longSize = CalculateLongSize(value);
        if (longSize > int.MaxValue)
        {
            throw new OverflowException("Calculated size exceeds Int32.MaxValue");
        }
        return (int)longSize;
    }

    public long CalculateLongSize(T value)
    {
        long size = 0;
        foreach (var member in _members)
        {
            if (member.WriteTag)
            {
                size += CodedOutputStream.ComputeUInt32Size(member.Tag);
            }
            size += member.CalculateLongSize(value);
        }

        return size;
    }

    public void WriteTo(ref WriterContext output, T value)
    {
        foreach (var member in _members)
        {
            member.WriteTo(ref output, value);
        }
    }

    int IProtoWriter.CalculateSize(object value) => CalculateSize((T)value);

    long IProtoWriter.CalculateLongSize(object value) => CalculateLongSize((T)value);

    void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (T)value);
}
