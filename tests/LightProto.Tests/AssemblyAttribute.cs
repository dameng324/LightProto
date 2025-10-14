using LightProto;
using LightProto.Tests;
using LightProto.Tests.CustomParser;

[module: ProtoParserTypeMap(
    typeof(CustomPriorityTests.Person),
    typeof(AssemblyLevelPersonProtoParser)
)]
