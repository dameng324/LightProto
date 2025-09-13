namespace LightProto.Tests.Parsers;

public partial class Inheritance
{
    [ProtoContract]
    public partial class Base
    {
        [ProtoMember(1)]
        public string BaseValue { get; set; } = "";
    }

    [ProtoContract]
    public partial class Message : Base
    {
        [ProtoMember(2)]
        public string Value { get; set; } = "";
    }
    
}