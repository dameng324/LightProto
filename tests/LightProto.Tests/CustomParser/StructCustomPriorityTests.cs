using System.Reflection;

namespace LightProto.Tests.CustomParser;

public partial class StructCustomPriorityTests
{
    [ProtoParserType(typeof(TypeLevelPersonProtoParser))]
    public struct Person
    {
        public int Id { get; set; }
        public Type? ParserType { get; set; }
    }

    public class StructPersonProtoParsers : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : IProtoReader<Person>
        {
            public WireFormat.WireType WireType { get; } = WireFormat.WireType.Varint;
            public bool IsMessage { get; } = false;

            public Person ParseFrom(ref ReaderContext input)
            {
                return new Person() { Id = input.ReadInt32(), ParserType = GetType() };
            }
        }

        public class LightProtoWriter : IProtoWriter<Person>
        {
            public WireFormat.WireType WireType { get; } = WireFormat.WireType.Varint;
            public bool IsMessage { get; } = false;

            public void WriteTo(ref WriterContext output, Person value)
            {
                output.WriteInt32(value.Id);
            }
        }
    }

    public class MemberLevelPersonProtoParser : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;

        public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
    }

    public class ClassLevelPersonProtoParser : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;

        public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
    }

    public class TypeLevelPersonProtoParser : IProtoParser<Person>
    {
        public static IProtoReader<Person> ProtoReader { get; } = new LightProtoReader();

        public static IProtoWriter<Person> ProtoWriter { get; } = new LightProtoWriter();

        public class LightProtoReader : StructPersonProtoParsers.LightProtoReader;

        public class LightProtoWriter : StructPersonProtoParsers.LightProtoWriter;
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
    public partial class TypeParserPersonContract
    {
        [ProtoMember(1)]
        public Person? Person { get; set; }
    }

    [Test]
    public async Task MemberParserTest()
    {
        var message = new MemberParserPersonContract() { Person = new Person() { Id = 37 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            MemberParserPersonContract.ProtoReader,
            MemberParserPersonContract.ProtoWriter
        );
#endif

        await Assert
            .That(cloned.Person!.Value.ParserType)
            .IsEqualTo(typeof(MemberLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Value.Id).IsEqualTo(message.Person.Value.Id);
    }

    [Test]
    public async Task ClassParserTest()
    {
        var message = new ClassParserPersonContract() { Person = new Person() { Id = 37 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            ClassParserPersonContract.ProtoReader,
            ClassParserPersonContract.ProtoWriter
        );
#endif

        await Assert
            .That(cloned.Person!.Value.ParserType)
            .IsEqualTo(typeof(ClassLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Value.Id).IsEqualTo(message.Person.Value.Id);
    }

    [Test]
    public async Task TypeLevelParserTest()
    {
        var message = new TypeParserPersonContract() { Person = new Person() { Id = 37 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            TypeParserPersonContract.ProtoReader,
            TypeParserPersonContract.ProtoWriter
        );
#endif

        await Assert
            .That(cloned.Person!.Value.ParserType)
            .IsEqualTo(typeof(TypeLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Value.Id).IsEqualTo(message.Person.Value.Id);
    }
}
