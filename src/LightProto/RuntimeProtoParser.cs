using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using LightProto.Parser;

namespace LightProto;

internal class RuntimeProtoParser<T>
{
    public RuntimeProtoReader<T> ProtoReader { get; }
    public RuntimeProtoWriter<T> ProtoWriter { get; }

    public RuntimeProtoParser(Func<T> factory)
    {
        ProtoReader = new RuntimeProtoReader<T>(factory);
        ProtoWriter = new RuntimeProtoWriter<T>();
    }

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember<
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        TValue>(int fieldNumber, Func<T, TValue> getter, Action<T, TValue> setter) =>
        AddMember(fieldNumber, getter, setter, Serializer.GetProtoReader<TValue>(), Serializer.GetProtoWriter<TValue>());

#if NET7_0_OR_GREATER
    [RequiresDynamicCode(Serializer.AOTWarning)]
#endif
    public void AddMember(
#if NET7_0_OR_GREATER
        [DynamicallyAccessedMembers(Serializer.LightProtoRequiredMembers)]
#endif
        Type type,
        int fieldNumber,
        Func<T, object> getter,
        Action<T, object> setter
    ) => AddMember(type, fieldNumber, getter, setter, Serializer.GetProtoReader(type), Serializer.GetProtoWriter(type));

    public void AddMember<TValue>(
        int fieldNumber,
        Func<T, TValue> getter,
        Action<T, TValue> setter,
        IProtoReader<TValue> reader,
        IProtoWriter<TValue> writer
    )
    {
        AddMember(
            typeof(TValue),
            fieldNumber,
            x => getter(x)!,
            (x, v) => setter(x, (TValue)v!),
            (IProtoReader)reader,
            (IProtoWriter)writer
        );
    }

    public void AddMember(
        Type type,
        int fieldNumber,
        Func<T, object> getter,
        Action<T, object> setter,
        IProtoReader reader,
        IProtoWriter writer
    )
    {
        ProtoReader.AddMember(type, fieldNumber, setter, reader);
        ProtoWriter.AddMember(type, fieldNumber, getter, writer);
    }
}
