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
}
