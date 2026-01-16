using Microsoft.CodeAnalysis;

namespace LightProto.Generator;

internal class LightProtoGeneratorException(string message) : Exception(message)
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DiagnosticSeverity Severity { get; set; }
    public Location? Location { get; set; }
    public IEnumerable<Location>? AdditionalLocations { get; set; }

    public static LightProtoGeneratorException InitOnlyOrReadOnlyWhenSkipConstructor(string memberName, Location? location)
    {
        return new LightProtoGeneratorException(
            "Member should not be initonly or readonly when SkipConstructor as we can't assign a value to it"
        )
        {
            Id = "LIGHT_PROTO_001",
            Title = $"{memberName} is InitOnly or ReadOnly when SkipConstructor",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException ProtoInclude_Type_Invalid(Location? location)
    {
        return new LightProtoGeneratorException("ProtoInclude attribute type is not valid")
        {
            Id = "LIGHT_PROTO_003",
            Title = $"ProtoInclude attribute type is not valid",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException Member_Type_Not_Supported(ProtoMember member)
    {
        return new LightProtoGeneratorException($"Member:{member.Name} Type: {member.Type} is not supported")
        {
            Id = "LIGHT_PROTO_004",
            Title = $"MemberType is not supported",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = member.DeclarationSyntax.GetLocation(),
        };
    }

    public static LightProtoGeneratorException ProtoInclude_Tag_Invalid(Location? location)
    {
        return new LightProtoGeneratorException("ProtoInclude attribute tag is not valid")
        {
            Id = "LIGHT_PROTO_005",
            Title = $"ProtoInclude attribute tag is not valid",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException Member_Tag_Duplicated(string memberName, uint tag, Location? location)
    {
        return new LightProtoGeneratorException($"Member:{memberName} Tag:{tag} is duplicated")
        {
            Id = "LIGHT_PROTO_006",
            Title = "Tag is duplicated",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException Interface_Must_Have_ProtoInclude(Location? location)
    {
        return new LightProtoGeneratorException("Interface must have ProtoIncludeAttribute")
        {
            Id = "LIGHT_PROTO_007",
            Title = $"Interface must have ProtoIncludeAttribute",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException Member_DeclarationSyntax_Not_Found(string memberName)
    {
        return new LightProtoGeneratorException($"can not find Member:{memberName} DelclarationSyntax")
        {
            Id = "LIGHT_PROTO_008",
            Title = "DeclarationSyntaxnot found",
            Category = "Usage",
            Severity = DiagnosticSeverity.Warning,
            Location = null,
        };
    }

    public static LightProtoGeneratorException ProtoInclude_Type_Not_Inherit(
        AttributeData attribute,
        INamedTypeSymbol derivedType,
        INamedTypeSymbol targetType
    )
    {
        return new LightProtoGeneratorException(
            $"ProtoInclude attribute type {derivedType.ToDisplayString()} does not inherit from {targetType.ToDisplayString()}"
        )
        {
            Id = "LIGHT_PROTO_009",
            Title = $"ProtoInclude attribute type does not inherit from target type",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation(),
        };
    }

    public static LightProtoGeneratorException ProtoInclude_Type_Should_Be_ProtoContract(string derivedTypeName, Location? location)
    {
        return new LightProtoGeneratorException($"ProtoInclude attribute type {derivedTypeName} should mark as ProtoContract")
        {
            Id = "LIGHT_PROTO_010",
            Title = $"ProtoInclude attribute type should mark as ProtoContract",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static LightProtoGeneratorException ProtoSurrogateFor_With_ProtoInclude(Location? location)
    {
        return new LightProtoGeneratorException("ProtoSurrogateForAttribute cannot be used with ProtoInclude attribute")
        {
            Id = "LIGHT_PROTO_011",
            Title = $"ProtoSurrogateForAttribute cannot be used with ProtoInclude attribute",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static Exception ProtoInclude_Duplicate_Type(string typeName, Location location)
    {
        return new LightProtoGeneratorException($"ProtoInclude attribute type {typeName} is duplicated")
        {
            Id = "LIGHT_PROTO_012",
            Title = $"ProtoInclude attribute type is duplicated",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static Exception Duplicate_ProtoParserTypeMapAttribute(string typeString, Location[] locations)
    {
        return new LightProtoGeneratorException($"Duplicate ProtoParserTypeMapAttribute for type {typeString}")
        {
            Id = "LIGHT_PROTO_013",
            Title = $"Duplicate ProtoParserTypeMapAttribute",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = locations[0],
            AdditionalLocations = locations.Skip(1),
        };
    }

    public static Exception ProtoParserTypeMustContainProtoReaderWriter(string parserType, string messageType, Location? getLocation)
    {
        return new LightProtoGeneratorException(
            $"Proto parser type {parserType} must contain static IProtoReader<{messageType}> ProtoReader Property and static IProtoWriter<{messageType}> ProtoWriter Property"
        )
        {
            Id = "LIGHT_PROTO_014",
            Title = $"Proto parser type must contain static ProtoReader or ProtoWriter Property",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = getLocation,
        };
    }

    public static Exception MessageTypeAndParserTypeCannotInSameAssemblyWhenUsingAssemblyLevelProtoParserMapAttribute(
        string messageType,
        string parserType,
        Location? location
    )
    {
        return new LightProtoGeneratorException(
            $"message type({messageType}) and parser type({parserType}) cannot be in the same assembly when using assembly level ProtoParserMapAttribute. Consider using LightProto.ProtoParserType(Type parserType) on message type instead."
        )
        {
            Id = "LIGHT_PROTO_015",
            Title =
                $"message type({messageType}) and parser type({parserType}) cannot be in the same assembly when using assembly level ProtoParserMapAttribute.",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = location,
        };
    }

    public static Exception CannotFindReadonlyMemberFieldName(string memberName, Location getLocation)
    {
        return new LightProtoGeneratorException($"Cannot find backing field for property {memberName}. Only support auto property.")
        {
            Id = "LIGHT_PROTO_016",
            Title = $"Cannot find backing field for property {memberName}",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = getLocation,
        };
    }

    public static Exception ReadOnlyMemberCannotInitialize(string memberName, Location getLocation)
    {
        return new LightProtoGeneratorException($"ReadOnly member {memberName} cannot be initialized.")
        {
            Id = "LIGHT_PROTO_017",
            Title = $"ReadOnly member {memberName} cannot be initialized",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = getLocation,
        };
    }

    public static Exception No_Parameterless_Constructor(string contractType, Location getLocation)
    {
        return new LightProtoGeneratorException($"Type {contractType} must have a parameterless constructor when SkipConstructor is false.")
        {
            Id = "LIGHT_PROTO_018",
            Title = $"Type {contractType} must have a parameterless constructor",
            Category = "Usage",
            Severity = DiagnosticSeverity.Error,
            Location = getLocation,
        };
    }
}
