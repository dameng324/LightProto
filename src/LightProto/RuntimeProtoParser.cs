using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using LightProto.Parser;

namespace LightProto;

internal class RuntimeProtoParser<T> : IProtoReader, IProtoWriter, IProtoReader<T>, IProtoWriter<T>
{
    private readonly Func<T> _factory;

    private record RuntimeProtoMember
    {
        public RuntimeProtoMember(
            int fieldNumber,
            uint tag,
            IProtoReader reader,
            IProtoWriter writer,
            Func<T, object> getter,
            Action<T, object> setter
        )
        {
            FieldNumber = fieldNumber;
            Tag = tag;
            Reader = reader;
            Writer = writer;
            Getter = getter;
            Setter = setter;
        }

        public WireFormat.WireType WireType => Reader.WireType;
        public int FieldNumber { get; set; }
        public uint Tag { get; set; }
        public IProtoReader Reader { get; set; }
        public IProtoWriter Writer { get; set; }
        public Func<T, object> Getter { get; }
        public Action<T, object> Setter { get; }
    }

    public RuntimeProtoParser(Func<T> factory)
    {
        _factory = factory;
    }

    private readonly Dictionary<uint, RuntimeProtoMember> _tagMembers = new();
    private readonly List<RuntimeProtoMember> _members = new();

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        TValue>(int fieldNumber, Func<T, TValue> getter, Action<T, TValue> setter) =>
        AddMember(fieldNumber, getter, setter, Serializer.GetProtoReader<TValue>(), Serializer.GetProtoWriter<TValue>());

    public void AddMember<TValue>(
        int fieldNumber,
        Func<T, TValue> getter,
        Action<T, TValue> setter,
        IProtoReader<TValue> reader,
        IProtoWriter<TValue> writer
    )
    {
        if (writer is ICollectionWriter collectionWriter)
        {
            var tag1 = WireFormat.MakeTag(fieldNumber, collectionWriter.ItemWireType);
            var member = new RuntimeProtoMember(
                fieldNumber,
                tag1,
                (IProtoReader)reader,
                (IProtoWriter)writer,
                x => getter(x)!,
                (x, v) => setter(x, (TValue)v!)
            );
            _members.Add(member);
            collectionWriter.Tag = tag1;
            _tagMembers[tag1] = member;
            var tag2 = WireFormat.MakeTag(fieldNumber, WireFormat.WireType.LengthDelimited);
            _tagMembers[tag2] = member;
        }
        else
        {
            var tag = WireFormat.MakeTag(fieldNumber, reader.WireType);
            var member = new RuntimeProtoMember(
                fieldNumber,
                tag,
                (IProtoReader)reader,
                (IProtoWriter)writer,
                x => getter(x)!,
                (x, v) => setter(x, (TValue)v!)
            );
            _members.Add(member);
            _tagMembers[tag] = member;
        }
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

    T IProtoReader<T>.ParseFrom(ref ReaderContext input)
    {
        var parsed = _factory();
        uint tag;
        while ((tag = input.ReadTag()) != 0)
        {
            if ((tag & 7) == 4)
            {
                break;
            }
            if (_tagMembers.TryGetValue(tag, out var member))
            {
                var value = member.Reader.ParseMessageFrom(ref input);
                member.Setter(parsed, value);
            }
            else
            {
                input.SkipLastField();
            }
        }
        return parsed!;
    }

    int IProtoWriter.CalculateSize(object value) => CalculateSize((T)value);

    void IProtoWriter.WriteTo(ref WriterContext output, object value) => WriteTo(ref output, (T)value);

    object IProtoReader.ParseFrom(ref ReaderContext input) => ((IProtoReader<T>)this).ParseFrom(ref input)!;
}
