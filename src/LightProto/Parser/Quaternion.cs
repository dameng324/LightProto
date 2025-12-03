#if NET7_0_OR_GREATER
using System.Numerics;

namespace LightProto.Parser;

[ProtoContract]
[ProtoSurrogateFor<Quaternion>]
public partial struct QuaternionProtoParser
{
    [ProtoMember(1, IsPacked = true)]
    internal float[] Floats { get; set; }

    public static implicit operator Quaternion(QuaternionProtoParser protoParser)
    {
        return new Quaternion(
            protoParser.Floats[0],
            protoParser.Floats[1],
            protoParser.Floats[2],
            protoParser.Floats[3]
        );
    }

    public static implicit operator QuaternionProtoParser(Quaternion value)
    {
        return new QuaternionProtoParser()
        {
            Floats = new[] { value.X, value.Y, value.Z, value.W },
        };
    }
}
#endif
