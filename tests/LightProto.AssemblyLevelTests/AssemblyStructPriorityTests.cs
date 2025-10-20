using Person = LightProto.Tests.CustomParser.StructCustomPriorityTests.Person;
using StructPersonProtoParsers = LightProto.Tests.CustomParser.StructCustomPriorityTests.StructPersonProtoParsers;

[assembly: LightProto.ProtoParserTypeMap(
    typeof(LightProto.Tests.CustomParser.StructCustomPriorityTests.Person),
    typeof(LightProto.AssemblyLevelTests.AssemblyStructPriorityTests.AssemblyLevelPersonProtoParser)
)]

namespace LightProto.AssemblyLevelTests;

public partial class AssemblyStructPriorityTests
{
    [ProtoContract]
    public partial class AssemblyParserPersonContract
    {
        [ProtoMember(1)]
        public Person? Person { get; set; }
    }

    [Test]
    public async Task AssemblyParserTest()
    {
        var message = new AssemblyParserPersonContract() { Person = new Person() { Id = 37 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            AssemblyParserPersonContract.ProtoReader,
            AssemblyParserPersonContract.ProtoWriter
        );
#endif

        await Assert
            .That(cloned.Person!.Value.ParserType)
            .IsEqualTo(typeof(AssemblyLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Value.Id).IsEqualTo(message.Person.Value.Id);
    }

    public class AssemblyLevelPersonProtoParser : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;

        public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
    }
}
