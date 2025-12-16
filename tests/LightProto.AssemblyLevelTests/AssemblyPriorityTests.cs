using System.Numerics;
using Person = LightProto.Tests.CustomParser.CustomPriorityTests.Person;
using StructPersonProtoParsers = LightProto.Tests.CustomParser.CustomPriorityTests.StructPersonProtoParsers;

[assembly: LightProto.ProtoParserTypeMap(
    typeof(LightProto.Tests.CustomParser.CustomPriorityTests.Person),
    typeof(LightProto.AssemblyLevelTests.AssemblyPriorityTests.AssemblyLevelPersonProtoParser)
)]

namespace LightProto.AssemblyLevelTests;

public partial class AssemblyPriorityTests
{
    [ProtoContract]
    public partial class AssemblyParserPersonContract
    {
        [ProtoMember(1)]
        public Person? Person { get; set; }
    }

    public class AssemblyLevelPersonProtoParser : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;

        public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
    }

    [Test]
    public async Task AssemblyParserTest()
    {
        var message = new AssemblyParserPersonContract() { Person = new Person() { Id = 37 } };
        var cloned = Serializer.DeepClone(message);

        await Assert
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(AssemblyLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person.Id);
    }
}
