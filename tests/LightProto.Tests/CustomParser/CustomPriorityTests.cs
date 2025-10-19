using System.Reflection;

// this is not allowed as message type and parser type are in same assembly.
// [assembly:LightProto.ProtoParserTypeMap(typeof(LightProto.Tests.CustomParser.CustomPriorityTests.Person),typeof(LightProto.Tests.CustomParser.CustomPriorityTests.MemberLevelPersonProtoParser))]
namespace LightProto.Tests.CustomParser;

public partial class CustomPriorityTests
{
    [ProtoParserType(typeof(TypeLevelPersonProtoParser))]
    public class Person
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

            public int CalculateSize(Person value)
            {
                return CodedOutputStream.ComputeInt32Size(value.Id);
            }

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
    public async Task MemberLevelParserTest()
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
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(MemberLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person.Id);
    }

    [Test]
    public async Task ClassLevelParserTest()
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
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(ClassLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person.Id);
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
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(TypeLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person.Id);
    }
}
public partial class CustomPriorityTests
{
    // Member-level vs Class-level vs Type-level precedence:
    // Person already has [ProtoParserType(typeof(TypeLevelPersonProtoParser))] at type level.
    // Here we add a contract with a class-level map AND a member-level ParserType to ensure:
    // Member-level wins over Class-level and Type-level.
    [ProtoContract]
    [ProtoParserTypeMap(typeof(Person), typeof(ClassLevelPersonProtoParser))]
    public partial class MemberOverridesClassParserContract
    {
        [ProtoMember(1, ParserType = typeof(MemberLevelPersonProtoParser))]
        public Person? Person { get; set; }
    }

    [Test]
    public async Task MemberLevel_Overrides_ClassLevel_And_TypeLevel()
    {
        var message = new MemberOverridesClassParserContract { Person = new Person { Id = 99 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            MemberOverridesClassParserContract.ProtoReader,
            MemberOverridesClassParserContract.ProtoWriter
        );
#endif
        await Assert
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(MemberLevelPersonProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person!.Id);
    }

    // Self-implemented parser fallback:
    // If a type implements IProtoParser<T> and no mapping is provided,
    // the generator should select that type's parser.
    public class SelfParserModel : IProtoParser<SelfParserModel>
    {
        public int Id { get; set; }
        public Type? ParserType { get; set; }

        public static IProtoReader<SelfParserModel> ProtoReader { get; } = new Reader();
        public static IProtoWriter<SelfParserModel> ProtoWriter { get; } = new Writer();

        public class Reader : IProtoReader<SelfParserModel>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public SelfParserModel ParseFrom(ref ReaderContext input)
                => new SelfParserModel { Id = input.ReadInt32(), ParserType = GetType() };
        }

        public class Writer : IProtoWriter<SelfParserModel>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public int CalculateSize(SelfParserModel value)
                => CodedOutputStream.ComputeInt32Size(value.Id);
            public void WriteTo(ref WriterContext output, SelfParserModel value)
                => output.WriteInt32(value.Id);
        }
    }

    [ProtoContract]
    public partial class SelfParserContract
    {
        [ProtoMember(1)]
        public SelfParserModel? Value { get; set; }
    }

    [Test]
    public async Task SelfImplementedParser_Is_Selected_When_No_Mapping()
    {
        var message = new SelfParserContract { Value = new SelfParserModel { Id = 123 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            SelfParserContract.ProtoReader,
            SelfParserContract.ProtoWriter
        );
#endif
        await Assert
            .That(cloned.Value!.ParserType)
            .IsEqualTo(typeof(SelfParserModel.Reader));
        await Assert.That(cloned.Value!.Id).IsEqualTo(message.Value!.Id);
    }
}