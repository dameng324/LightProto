namespace LightProto.Tests.CustomParser;

public partial class CustomPriorityTests
{
    public class Person
    {
        public int Id { get; set; }
    }

    [ProtoContract]
    public partial class MemberParserPersonContract
    {
        [ProtoMember(1, ParserType = typeof(MemberLevelPersonProtoParser))]
        public Person? Person { get; set; }
    }

    [ProtoContract]
    [ProtoParserTypeMap(typeof(Person), typeof(ClassLevelPersonProtoParser))]
    public partial class ClassParserPersonContract
    {
        [ProtoMember(1)]
        public Person? Person { get; set; }
    }

    [ProtoContract]
    public partial class AssemblyParserPersonContract
    {
        [ProtoMember(1)]
        public Person? Person { get; set; }
    }

    [Test]
    public async Task MemberParserTest()
    {
        await Assert
            .That(new MemberParserPersonContract.LightProtoReader().Person_ProtoReader.GetType())
            .IsEqualTo(typeof(MemberLevelPersonProtoParser.LightProtoReader));
        await Assert
            .That(new MemberParserPersonContract.LightProtoWriter().Person_ProtoWriter.GetType())
            .IsEqualTo(typeof(MemberLevelPersonProtoParser.LightProtoWriter));
    }

    [Test]
    public async Task ClassParserTest()
    {
        await Assert
            .That(new ClassParserPersonContract.LightProtoReader().Person_ProtoReader.GetType())
            .IsEqualTo(typeof(ClassLevelPersonProtoParser.LightProtoReader));
        await Assert
            .That(new ClassParserPersonContract.LightProtoWriter().Person_ProtoWriter.GetType())
            .IsEqualTo(typeof(ClassLevelPersonProtoParser.LightProtoWriter));
    }

    [Test]
    public async Task AssemblyParserTest()
    {
        await Assert
            .That(new AssemblyParserPersonContract.LightProtoReader().Person_ProtoReader.GetType())
            .IsEqualTo(typeof(AssemblyLevelPersonProtoParser.LightProtoReader));
        await Assert
            .That(new AssemblyParserPersonContract.LightProtoWriter().Person_ProtoWriter.GetType())
            .IsEqualTo(typeof(AssemblyLevelPersonProtoParser.LightProtoWriter));
    }
}
