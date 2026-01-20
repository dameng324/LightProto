using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace LightProto.Generator;

internal static class LightProtoGeneratorWarning
{
    internal static Diagnostic MemberDefaultValueMayBreakDeserialization(string memberName, ImmutableArray<Location> locations)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W001",
                title: "Member has default value which may break deserialization",
                messageFormat: "Member '{0}' has a default value which may get a different deserialization result than expected. Consider setting SkipConstructor=true on ProtoContract or removing the default value.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w001"
            ),
            locations.FirstOrDefault() ?? Location.None,
            additionalLocations: locations.Skip(1),
            memberName
        );
    }

    public static Diagnostic MemberIsPackedButNotCollection(string memberName, string typeName, ImmutableArray<Location> locations)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W002",
                title: "Member is marked as IsPacked but is not a collection type",
                messageFormat: "Member '{0}' with type '{1}'is marked as IsPacked but is not a collection type. The IsPacked option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w002"
            ),
            locations.FirstOrDefault() ?? Location.None,
            additionalLocations: locations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic MemberIsPackedButItemNotSupportPacked(string memberName, string typeName, ImmutableArray<Location> locations)
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W003",
                title: "Member is marked as IsPacked but the item type does not support packed encoding",
                messageFormat: "Member '{0}' with type '{1}'is marked as IsPacked but the item type does not support packed encoding. The IsPacked option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w003"
            ),
            locations.FirstOrDefault() ?? Location.None,
            additionalLocations: locations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic StringInternAttribute_Applied_NonString_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W004",
                title: "StringInternAttribute applied to non-string type",
                messageFormat: "Member '{0}' with type '{1}'is marked with StringInternAttribute but is not of type string. The attribute will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w004"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic CompatibilityLevel240_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W005",
                title: "Member has CompatibilityLevel.Level240 but the type does not support it",
                messageFormat: "Member '{0}' with type '{1}'is of a well-known type that is not supported in CompatibilityLevel.Level240. The CompatibilityLevel.Level240 option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w005"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic CompatibilityLevel300_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W006",
                title: "Member has CompatibilityLevel.Level300 but the type does not support it",
                messageFormat: "Member '{0}' with type '{1}'is of a well-known type that is not supported in CompatibilityLevel.Level300. The CompatibilityLevel.Level300 option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w006"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_DataFormat_ZigZag_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W007",
                title: "Member has DataFormat.ZigZag but the type does not support it",
                messageFormat: "Member '{0}' with type '{1}'is marked with DataFormat.ZigZag but the type does not support it. The DataFormat.ZigZag option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w007"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_DataFormat_FixedSize_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W008",
                title: "Member has DataFormat.FixedSize but the type does not support it",
                messageFormat: "Member '{0}' with type '{1}' is marked with DataFormat.FixedSize but the type does not support it. The DataFormat.FixedSize option will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w008"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_ProtoMapAttribute_But_Not_Dictionary(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W009",
                title: "Member has ProtoMapAttribute but is not a dictionary type",
                messageFormat: "Member '{0}' with type '{1}'is marked with ProtoMapAttribute but is not a dictionary type. The ProtoMapAttribute will be ignored.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w009"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_ProtoMap_Key_DataFormat_ZigZag_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W010",
                title: "Member has ProtoMapAttribute with DataFormat.ZigZag on key but the key type does not support it",
                messageFormat: "Key type '{0}' is marked with ProtoMapAttribute with DataFormat.ZigZag on key but the key type does not support it. The DataFormat.ZigZag option will be ignored for the key.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w010"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_ProtoMap_Key_DataFormat_FixedSize_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W011",
                title: "Member has ProtoMapAttribute with DataFormat.FixedSize on key but the key type does not support it",
                messageFormat: "Key type '{0}' is marked with ProtoMapAttribute with DataFormat.FixedSize on key but the key type does not support it. The DataFormat.FixedSize option will be ignored for the key.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w011"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_ProtoMap_Value_DataFormat_ZigZag_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W012",
                title: "Member has ProtoMapAttribute with DataFormat.ZigZag on value but the value type does not support it",
                messageFormat: "Value type '{0}' is marked with ProtoMapAttribute with DataFormat.ZigZag on value but the value type does not support it. The DataFormat.ZigZag option will be ignored for the value.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w012"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }

    public static Diagnostic Member_ProtoMap_Value_DataFormat_FixedSize_Not_Supported_Type(
        string memberName,
        string typeName,
        ImmutableArray<Location> memberLocations
    )
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "LIGHT_PROTO_W013",
                title: "Member has ProtoMapAttribute with DataFormat.FixedSize on value but the value type does not support it",
                messageFormat: "Value type '{0}' is marked with ProtoMapAttribute with DataFormat.FixedSize on value but the value type does not support it. The DataFormat.FixedSize option will be ignored for the value.",
                category: "Usage",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                helpLinkUri: "https://github.com/dameng324/LightProto/blob/main/docs/Diagnostics.md#light_proto_w013"
            ),
            memberLocations.FirstOrDefault() ?? Location.None,
            additionalLocations: memberLocations.Skip(1),
            memberName,
            typeName
        );
    }
}
