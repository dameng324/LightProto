using System.Collections;
using System.Runtime.CompilerServices;

namespace LightProto.Parser;

[ProtoContract]
[ProtoSurrogateFor<BitArray>]
public partial struct BitArrayProtoParser
{
    [ProtoMember(1, IsPacked = true)]
    internal bool[] bits { get; set; }

    public static implicit operator BitArray(BitArrayProtoParser proxy)
    {
        return new BitArray(proxy.bits);
    }

    public static implicit operator BitArrayProtoParser(BitArray value)
    {
        bool[] bits = new bool[value.Count];
        value.CopyTo(bits, 0);

        return new BitArrayProtoParser { bits = bits };
    }
}
