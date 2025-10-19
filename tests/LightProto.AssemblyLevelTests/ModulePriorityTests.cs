using System;

[assembly: LightProto.ProtoParserTypeMap(
    typeof(LightProto.AssemblyLevelTests.ModulePriorityTests.PersonX),
    typeof(LightProto.AssemblyLevelTests.ModulePriorityTests.AssemblyLevelPersonXProtoParser)
)]
[module: LightProto.ProtoParserTypeMap(
    typeof(LightProto.AssemblyLevelTests.ModulePriorityTests.PersonX),
    typeof(LightProto.AssemblyLevelTests.ModulePriorityTests.ModuleLevelPersonXProtoParser)
)]

namespace LightProto.AssemblyLevelTests;

public partial class ModulePriorityTests
{
    // Message used to validate precedence between module-level and assembly-level maps
    public class PersonX
    {
        public int Id { get; set; }
        public Type? ParserType { get; set; }
    }

    public class AssemblyLevelPersonXProtoParser : IProtoParser<PersonX>
    {
        public static IProtoReader<PersonX> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<PersonX> ProtoWriter { get; } = new LightProtoWriter();
        public class LightProtoReader : IProtoReader<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public PersonX ParseFrom(ref ReaderContext input)
                => new PersonX { Id = input.ReadInt32(), ParserType = GetType() };
        }
        public class LightProtoWriter : IProtoWriter<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public int CalculateSize(PersonX value)
                => CodedOutputStream.ComputeInt32Size(value.Id);
            public void WriteTo(ref WriterContext output, PersonX value)
                => output.WriteInt32(value.Id);
        }
    }

    public class ModuleLevelPersonXProtoParser : IProtoParser<PersonX>
    {
        public static IProtoReader<PersonX> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<PersonX> ProtoWriter { get; } = new LightProtoWriter();
        public class LightProtoReader : IProtoReader<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public PersonX ParseFrom(ref ReaderContext input)
                => new PersonX { Id = input.ReadInt32(), ParserType = GetType() };
        }
        public class LightProtoWriter : IProtoWriter<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public int CalculateSize(PersonX value)
                => CodedOutputStream.ComputeInt32Size(value.Id);
            public void WriteTo(ref WriterContext output, PersonX value)
                => output.WriteInt32(value.Id);
        }
    }

    public class ClassLevelPersonXProtoParser : IProtoParser<PersonX>
    {
        public static IProtoReader<PersonX> ProtoReader { get; } = new LightProtoReader();
        public static IProtoWriter<PersonX> ProtoWriter { get; } = new LightProtoWriter();
        public class LightProtoReader : IProtoReader<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public PersonX ParseFrom(ref ReaderContext input)
                => new PersonX { Id = input.ReadInt32(), ParserType = GetType() };
        }
        public class LightProtoWriter : IProtoWriter<PersonX>
        {
            public WireFormat.WireType WireType => WireFormat.WireType.Varint;
            public bool IsMessage => false;
            public int CalculateSize(PersonX value)
                => CodedOutputStream.ComputeInt32Size(value.Id);
            public void WriteTo(ref WriterContext output, PersonX value)
                => output.WriteInt32(value.Id);
        }
    }

    [ProtoContract]
    public partial class ModuleParserPersonXContract
    {
        [ProtoMember(1)]
        public PersonX? Person { get; set; }
    }

    [ProtoContract]
    [ProtoParserTypeMap(typeof(PersonX), typeof(ClassLevelPersonXProtoParser))]
    public partial class ClassOverridesModulePersonXContract
    {
        [ProtoMember(1)]
        public PersonX? Person { get; set; }
    }

    [Test]
    public async Task ModuleLevel_Overrides_AssemblyLevel_For_MessageType()
    {
        var message = new ModuleParserPersonXContract { Person = new PersonX { Id = 17 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            ModuleParserPersonXContract.ProtoReader,
            ModuleParserPersonXContract.ProtoWriter
        );
#endif
        await Assert
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(ModuleLevelPersonXProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person!.Id);
    }

    [Test]
    public async Task ClassLevel_Overrides_ModuleLevel()
    {
        var message = new ClassOverridesModulePersonXContract { Person = new PersonX { Id = 23 } };
#if NET5_0_OR_GREATER
        var cloned = Serializer.DeepClone(message);
#else
        var cloned = Serializer.DeepClone(
            message,
            ClassOverridesModulePersonXContract.ProtoReader,
            ClassOverridesModulePersonXContract.ProtoWriter
        );
#endif
        await Assert
            .That(cloned.Person!.ParserType)
            .IsEqualTo(typeof(ClassLevelPersonXProtoParser.LightProtoReader));
        await Assert.That(cloned.Person!.Id).IsEqualTo(message.Person!.Id);
    }
}