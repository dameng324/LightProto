namespace LightProto.Tests.CustomParser;

public class PersonProtoParsers : IProtoParser<CustomPriorityTests.Person>
{
    public static IProtoReader<CustomPriorityTests.Person> ProtoReader { get; } =
        new LightProtoReader();
    public static IProtoWriter<CustomPriorityTests.Person> ProtoWriter { get; } =
        new LightProtoWriter();

    public class LightProtoReader : IProtoReader<CustomPriorityTests.Person>
    {
        public WireFormat.WireType WireType { get; } = WireFormat.WireType.Varint;
        public bool IsMessage { get; } = false;

        public CustomPriorityTests.Person ParseFrom(ref ReaderContext input)
        {
            return new CustomPriorityTests.Person() { Id = input.ReadInt32() };
        }
    }

    public class LightProtoWriter : IProtoWriter<CustomPriorityTests.Person>
    {
        public WireFormat.WireType WireType { get; } = WireFormat.WireType.Varint;
        public bool IsMessage { get; } = false;

        public int CalculateSize(CustomPriorityTests.Person value)
        {
            return CodedOutputStream.ComputeInt32Size(value.Id);
        }

        public void WriteTo(ref WriterContext output, CustomPriorityTests.Person value)
        {
            output.WriteInt32(value.Id);
        }
    }
}

public class MemberLevelPersonProtoParser : IProtoParser<CustomPriorityTests.Person>
{
    public static IProtoReader<CustomPriorityTests.Person> ProtoReader { get; } =
        new LightProtoReader();
    public static IProtoWriter<CustomPriorityTests.Person> ProtoWriter { get; } =
        new LightProtoWriter();

    public class LightProtoReader : PersonProtoParsers.LightProtoReader;

    public class LightProtoWriter : PersonProtoParsers.LightProtoWriter;
}

public class ClassLevelPersonProtoParser : IProtoParser<CustomPriorityTests.Person>
{
    public static IProtoReader<CustomPriorityTests.Person> ProtoReader { get; } =
        new LightProtoReader();
    public static IProtoWriter<CustomPriorityTests.Person> ProtoWriter { get; } =
        new LightProtoWriter();

    public class LightProtoReader : PersonProtoParsers.LightProtoReader;

    public class LightProtoWriter : PersonProtoParsers.LightProtoWriter;
}

public class AssemblyLevelPersonProtoParser : IProtoParser<CustomPriorityTests.Person>
{
    public static IProtoReader<CustomPriorityTests.Person> ProtoReader { get; } =
        new LightProtoReader();
    public static IProtoWriter<CustomPriorityTests.Person> ProtoWriter { get; } =
        new LightProtoWriter();

    public class LightProtoReader : PersonProtoParsers.LightProtoReader;

    public class LightProtoWriter : PersonProtoParsers.LightProtoWriter;
}
