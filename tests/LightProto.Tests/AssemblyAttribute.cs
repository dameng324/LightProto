using LightProto;
using LightProto.Tests;
using LightProto.Tests.CustomParser;

[assembly: ProtoParserTypeMap(
    typeof(CustomPriorityTests.Person),
    typeof(AssemblyLevelPersonProtoParser)
)]
