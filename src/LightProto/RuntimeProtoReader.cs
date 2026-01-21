using System.Diagnostics.CodeAnalysis;
using LightProto.Parser;

namespace LightProto;

public class RuntimeProtoReader<T> : IProtoReader, IProtoReader<T>
{
    private readonly Func<T> _factory;

    private record ProtoMember(IProtoReader Reader, Action<T, object> Setter)
    {
        public IProtoReader Reader { get; } = Reader;
        public Action<T, object> Setter { get; } = Setter;
    }

    public RuntimeProtoReader(Func<T> factory)
    {
        _factory = factory;
    }

    private readonly Dictionary<uint, ProtoMember> _tagMembers = new();

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        TValue>(int fieldNumber, Action<T, TValue> setter) => AddMember(fieldNumber, setter, Serializer.GetProtoReader<TValue>());

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        Type type,
        int fieldNumber,
        Action<T, object> setter
    ) => AddMember(type, fieldNumber, setter, Serializer.GetProtoReader(type));

    public void AddMember<TValue>(int fieldNumber, Action<T, TValue> setter, IProtoReader<TValue> reader)
    {
        AddMember(typeof(TValue), fieldNumber, (x, v) => setter(x, (TValue)v!), (IProtoReader)reader);
    }

    public void AddMember(Type type, int fieldNumber, Action<T, object> setter, IProtoReader reader)
    {
        if (reader is ICollectionReader collectionReader)
        {
            var tag1 = WireFormat.MakeTag(fieldNumber, collectionReader.ItemWireType);
            var member = new ProtoMember(reader, (x, v) => setter(x, v!));
            _tagMembers[tag1] = member;
            var tag2 = WireFormat.MakeTag(fieldNumber, WireFormat.WireType.LengthDelimited);
            _tagMembers[tag2] = member;
        }
        else
        {
            var tag = WireFormat.MakeTag(fieldNumber, reader.WireType);
            var member = new ProtoMember(reader, (x, v) => setter(x, v!));
            _tagMembers[tag] = member;
        }
    }

    public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;
    public bool IsMessage => true;

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

    object IProtoReader.ParseFrom(ref ReaderContext input) => ((IProtoReader<T>)this).ParseFrom(ref input)!;
}
