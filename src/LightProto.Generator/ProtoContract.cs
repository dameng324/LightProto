using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightProto.Generator;

class ProtoContract
{
    public Compilation Compilation { get; set; } = null!;
    public INamedTypeSymbol Type { get; set; } = null!;
    public TypeDeclarationSyntax TypeDeclaration { get; set; } = null!;
    public List<ProtoMember> Members { get; set; } = new();
    public ImplicitFields ImplicitFields { get; set; }
    public uint ImplicitFirstTag { get; set; }
    public ImmutableArray<AttributeData> AttributeData { get; set; }
    public bool SkipConstructor { get; set; }
    public ITypeSymbol? ProxyFor { get; set; }

    //public ProtoContract? BaseTypeContract { get; set; }
    public List<(uint RawTag, ProtoContract Contract)> DerivedTypeContracts { get; set; } = new();

    internal static ProtoContract? GetProtoContract(Compilation compilation, ISymbol? type, SourceProductionContext spc)
    {
        if (type is not INamedTypeSymbol targetType)
        {
            return null;
        }
        // Look for ProtoContract attribute
        var protoContractAttr = targetType
            .GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == LightProtoGenerator.ProtoContractAttributeFullName);

        if (protoContractAttr is null)
            return null;

        ImplicitFields implicitFields = (ImplicitFields)(
            protoContractAttr.NamedArguments.FirstOrDefault(arg => arg.Key == "ImplicitFields").Value.Value ?? ImplicitFields.None
        );

        uint implicitFirstTag = uint.TryParse(
            protoContractAttr.NamedArguments.FirstOrDefault(arg => arg.Key == "ImplicitFirstTag").Value.Value?.ToString(),
            out var _implicitFirstTag
        )
            ? _implicitFirstTag
            : 1;

        var skipConstructor =
            protoContractAttr.NamedArguments.FirstOrDefault(arg => arg.Key == "SkipConstructor").Value.Value?.ToString() == "True";
        var typeDeclaration = (targetType.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax)!;

        var proxyFor = GetProxyFor(targetType.GetAttributes());
        var members = GetProtoMembers(compilation, targetType, typeDeclaration, implicitFields, implicitFirstTag, skipConstructor, spc);
        var protoIncludeAttributes = targetType
            .GetAttributes()
            .Where(attr => attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoIncludeAttribute")
            .ToList();
        var derivedTypeContracts = new List<(uint RawTag, ProtoContract Contract)>();
        foreach (var attribute in protoIncludeAttributes)
        {
            if (!uint.TryParse(attribute.ConstructorArguments[0].Value?.ToString(), out var tag))
            {
                throw LightProtoGeneratorException.ProtoInclude_Tag_Invalid(
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                );
            }
            var rawTag = ProtoMember.GetRawTag(tag, ProtoMember.PbWireType.LengthDelimited);
            var derivedType = attribute.ConstructorArguments[1].Value as INamedTypeSymbol;
            if (derivedType is null)
            {
                throw LightProtoGeneratorException.ProtoInclude_Type_Invalid(
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                );
            }

            if (targetType.TypeKind == TypeKind.Interface)
            {
                if (!derivedType.AllInterfaces.Any(it => SymbolEqualityComparer.Default.Equals(it, targetType)))
                {
                    throw LightProtoGeneratorException.ProtoInclude_Type_Not_Inherit(attribute, derivedType, targetType);
                }
            }
            else
            {
                var checkType = derivedType;
                while (true)
                {
                    if (checkType is null)
                    {
                        throw LightProtoGeneratorException.ProtoInclude_Type_Not_Inherit(attribute, derivedType, targetType);
                    }

                    if (SymbolEqualityComparer.Default.Equals(checkType.BaseType, targetType))
                    {
                        break;
                    }

                    checkType = checkType.BaseType;
                }
            }

            var contract = GetProtoContract(compilation, derivedType, spc);
            if (contract is null)
            {
                throw LightProtoGeneratorException.ProtoInclude_Type_Should_Be_ProtoContract(
                    derivedType.ToDisplayString(),
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                );
            }

            if (members.Any(x => x.FieldNumber == tag))
            {
                throw LightProtoGeneratorException.ProtoInclude_DerivedType_Tag_Conflicts_With_Member(
                    derivedType.ToDisplayString(),
                    tag,
                    attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                );
            }

            derivedTypeContracts.Add((rawTag, contract));
        }

        if (derivedTypeContracts.Any() && proxyFor is not null)
        {
            throw LightProtoGeneratorException.ProtoSurrogateFor_With_ProtoInclude(typeDeclaration.GetLocation());
        }
        if (
            derivedTypeContracts.GroupBy(o => o.Contract.Type, SymbolEqualityComparer.Default).FirstOrDefault(x => x.Count() > 1) is { } grp
        )
        {
            throw LightProtoGeneratorException.ProtoInclude_Duplicate_Type(grp.Key!.ToDisplayString(), typeDeclaration.GetLocation());
        }

        return new ProtoContract
        {
            Compilation = compilation,
            Type = targetType,
            TypeDeclaration = typeDeclaration,
            Members = members,
            ImplicitFields = implicitFields,
            ImplicitFirstTag = implicitFirstTag,
            AttributeData = default,
            SkipConstructor = skipConstructor,
            ProxyFor = proxyFor,
            //BaseTypeContract = GetProtoContract(compilation, targetType.BaseType),
            DerivedTypeContracts = derivedTypeContracts,
        };
    }

    private static ITypeSymbol? GetProxyFor(ImmutableArray<AttributeData> attributeDatas)
    {
        if (
            attributeDatas.FirstOrDefault(o =>
                o.AttributeClass?.ToDisplayString().StartsWith("LightProto.ProtoSurrogateForAttribute<") == true
            ) is
            { } proxyAttr2
        )
        {
            return proxyAttr2.AttributeClass!.TypeArguments[0];
        }
        if (
            attributeDatas.FirstOrDefault(o => o.AttributeClass?.ToDisplayString() == ("LightProto.ProtoSurrogateForAttribute")) is
            { } proxyAttr
        )
        {
            return proxyAttr.ConstructorArguments[0].Value as ITypeSymbol;
        }

        return null;
    }

    internal static ITypeSymbol? GetProxyType(ITypeSymbol type)
    {
        return GetProxyType(type.GetAttributes());
    }

    private static ITypeSymbol? GetProxyType(IEnumerable<AttributeData> attributeDatas)
    {
        if (
            attributeDatas.FirstOrDefault(o => o.AttributeClass?.ToDisplayString().StartsWith("LightProto.ProtoProxyAttribute<") == true) is
            { } proxyAttr2
        )
        {
            return proxyAttr2.AttributeClass!.TypeArguments[0];
        }

        return null;
    }

    private static List<ProtoMember> GetProtoMembers(
        Compilation compilation,
        INamedTypeSymbol targetType,
        TypeDeclarationSyntax typeDeclaration,
        ImplicitFields implicitFields,
        uint firstImplicitTag,
        bool skipConstructor,
        SourceProductionContext spc
    )
    {
        var members = new List<ProtoMember>();
        var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
        var emitter = new ExpressionEmitter(semanticModel);
        foreach (var member in targetType.GetMembers())
        {
            if (member.IsStatic)
            {
                continue;
            }

            if (member.IsImplicitlyDeclared)
            {
                continue;
            }

            uint tag;
            DataFormat dataFormat;
            bool isPacked;
            bool isProtoMemberRequired;
            AttributeData? protoMemberAttr = member
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoMemberAttribute");
            if (protoMemberAttr is not null)
            {
                tag = (uint)protoMemberAttr.ConstructorArguments[0].Value!;

                dataFormat = Enum.TryParse<DataFormat>(
                    protoMemberAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "DataFormat").Value.Value?.ToString(),
                    out var _dataFormat
                )
                    ? _dataFormat
                    : DataFormat.Default;

                isPacked =
                    bool.TryParse(
                        protoMemberAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "IsPacked").Value.Value?.ToString(),
                        out var _isPacked
                    ) && _isPacked;

                isProtoMemberRequired =
                    bool.TryParse(
                        protoMemberAttr.NamedArguments.FirstOrDefault(kv => kv.Key == "IsRequired").Value.Value?.ToString(),
                        out var _isProtoMemberRequired
                    ) && _isProtoMemberRequired;
            }
            else
            {
                if (implicitFields is ImplicitFields.None)
                {
                    continue;
                }

                if (implicitFields is ImplicitFields.AllPublic && member.DeclaredAccessibility is not Accessibility.Public)
                {
                    continue;
                }

                if (member is not IPropertySymbol and not IFieldSymbol)
                {
                    continue;
                }

                if (member is IPropertySymbol { IsReadOnly: true })
                {
                    continue;
                }

                if (member is IFieldSymbol { IsReadOnly: true } or IFieldSymbol { IsConst: true })
                {
                    continue;
                }
                if (member.GetAttributes().Any(o => o.AttributeClass?.ToDisplayString() == "LightProto.ProtoIgnoreAttribute"))
                {
                    continue;
                }

                tag = firstImplicitTag;
                firstImplicitTag++;
                dataFormat = DataFormat.Default;
                isPacked = false;
                isProtoMemberRequired = false;
            }

            string memberName;
            string? initializer;
            NullableAnnotation nullableAnnotation;
            ITypeSymbol memberType;
            bool isReadOnly;
            bool isRequired;
            bool isInitOnly;
            MemberDeclarationSyntax? memberDeclarationSyntax;
            if (member is IPropertySymbol property)
            {
                memberName = property.Name;
                var propertyDeclarationSyntax = property
                    .DeclaringSyntaxReferences.Select(x => x.GetSyntax())
                    .OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault();
                memberDeclarationSyntax = propertyDeclarationSyntax;
                var initializerSyntax = propertyDeclarationSyntax?.Initializer?.Value;
                initializer = initializerSyntax is null ? null : emitter.Emit(initializerSyntax);
                nullableAnnotation = property.NullableAnnotation;
                memberType = property.Type;
                isReadOnly = property.IsReadOnly;
                isRequired = property.IsRequired;
                isInitOnly = property.SetMethod?.IsInitOnly == true;
            }
            else if (member is IFieldSymbol field)
            {
                memberName = field.Name;

                var fieldDeclarationSyntax = field
                    .DeclaringSyntaxReferences.Select(x =>
                        x.GetSyntax() is VariableDeclaratorSyntax v
                        && v.Parent is VariableDeclarationSyntax vd
                        && vd.Parent is FieldDeclarationSyntax f
                            ? f
                            : null
                    )
                    .OfType<FieldDeclarationSyntax>()
                    .FirstOrDefault();

                memberDeclarationSyntax = fieldDeclarationSyntax;
                var initializerSyntax = fieldDeclarationSyntax?.Declaration.Variables.FirstOrDefault()?.Initializer?.Value;
                initializer = initializerSyntax is null ? null : emitter.Emit(initializerSyntax);
                nullableAnnotation = field.NullableAnnotation;
                memberType = field.Type;
                isReadOnly = field.IsReadOnly;
                isRequired = field.IsRequired;
                isInitOnly = false;
            }
            else
            {
                continue;
            }

            if (memberDeclarationSyntax is null)
            {
                throw LightProtoGeneratorException.Member_DeclarationSyntax_Not_Found($"{targetType.ToDisplayString()}.{member.Name}");
            }

            if (initializer is null)
            {
                if (nullableAnnotation == NullableAnnotation.Annotated)
                {
                    initializer = "null";
                }
                else if (HasEmptyStaticField(memberType))
                {
                    initializer = $"{memberType}.Empty";
                }
                else if (Helper.IsCollectionType(compilation, memberType) || Helper.IsDictionaryType(compilation, memberType))
                {
                    if (memberType is IArrayTypeSymbol arrayTypeSymbol)
                    {
                        initializer = $"Array.Empty<{arrayTypeSymbol.ElementType}>()";
                    }
                    else if (memberType.TypeKind == TypeKind.Interface || memberType.IsAbstract)
                    {
                        var concreteType = Helper.ResolveConcreteTypeSymbol(compilation, (memberType as INamedTypeSymbol)!);
                        initializer = $"new {concreteType.ToDisplayString()}()";
                    }
                    else
                    {
                        initializer = "default";
                    }
                }
                else
                {
                    initializer = "default";
                }
            }
            else
            {
                if (
                    !skipConstructor
                    && memberType.IsValueType
                    && !Helper.IsCollectionType(compilation, memberType)
                    && !Helper.IsDictionaryType(compilation, memberType)
                )
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.MemberDefaultValueMayBreakDeserialization(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
            }

            if (isPacked)
            {
                if (Helper.IsCollectionType(compilation, memberType, out var itemType))
                {
                    if (Helper.SupportsPackedEncoding(compilation, itemType) == false)
                    {
                        spc.ReportDiagnostic(
                            LightProtoGeneratorWarning.MemberIsPackedButItemNotSupportPacked(
                                $"{targetType}.{member.Name}",
                                member.Locations
                            )
                        );
                    }
                }
                else
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.MemberIsPackedButNotCollection($"{targetType}.{member.Name}", member.Locations)
                    );
                }
            }

            if (dataFormat is DataFormat.ZigZag)
            {
                if (Helper.SupportZigZag(memberType))
                {
                    //OK
                }
                else if (Helper.IsCollectionType(compilation, memberType, out var itemType) && Helper.SupportZigZag(itemType!))
                {
                    //OK
                }
                else
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_DataFormat_ZigZag_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
            }

            if (dataFormat is DataFormat.FixedSize)
            {
                if (Helper.SupportFixedSize(memberType))
                {
                    //OK
                }
                else if (Helper.IsCollectionType(compilation, memberType, out var itemType) && Helper.SupportFixedSize(itemType!))
                {
                    //OK
                }
                else
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_DataFormat_FixedSize_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
            }

            bool HasEmptyStaticField(ITypeSymbol type)
            {
                return type.GetMembers().OfType<IFieldSymbol>().Any(o => o.IsStatic && o.Name == "Empty");
            }

            ITypeSymbol? ProxyType = GetProxyType(member.GetAttributes()) ?? GetProxyType(memberType);

            bool HasStringInternAttribute(IEnumerable<AttributeData> attributeDatas)
            {
                return attributeDatas.Any(attr => attr.AttributeClass?.ToDisplayString() == "LightProto.StringInternAttribute");
            }

            var memberStringIntern = HasStringInternAttribute(member.GetAttributes());
            bool stringIntern =
                memberStringIntern
                || HasStringInternAttribute(targetType.GetAttributes())
                || HasStringInternAttribute(targetType.ContainingModule.GetAttributes())
                || HasStringInternAttribute(targetType.ContainingAssembly.GetAttributes());

            if (memberStringIntern)
            {
                if (memberType.SpecialType == SpecialType.System_String)
                {
                    // OK
                }
                else if (memberType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_String })
                {
                    // OK
                }
                else if (
                    memberType is INamedTypeSymbol namedType
                    && namedType.TypeArguments.Any(x => x.SpecialType == SpecialType.System_String)
                )
                {
                    // OK
                }
                else
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.StringInternAttribute_Applied_NonString_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
            }

            AttributeData? GetCompatibilityLevelAttribute(IEnumerable<AttributeData> attributeDatas)
            {
                return attributeDatas.FirstOrDefault(attr =>
                    attr.AttributeClass?.ToDisplayString() == "LightProto.CompatibilityLevelAttribute"
                );
            }

            var memberCompatibilityLevelAttr = GetCompatibilityLevelAttribute(member.GetAttributes());
            AttributeData? compatibilityLevelAttr =
                memberCompatibilityLevelAttr
                ?? GetCompatibilityLevelAttribute(targetType.GetAttributes())
                ?? GetCompatibilityLevelAttribute(targetType.ContainingModule.GetAttributes())
                ?? GetCompatibilityLevelAttribute(targetType.ContainingAssembly.GetAttributes());

            CompatibilityLevel compatibilityLevel = Enum.TryParse<CompatibilityLevel>(
                compatibilityLevelAttr?.ConstructorArguments[0].Value?.ToString(),
                out var _compatibilityLevel
            )
                ? _compatibilityLevel
                : CompatibilityLevel.Level200;

            if (memberCompatibilityLevelAttr is not null)
            {
                if (compatibilityLevel == CompatibilityLevel.Level240)
                {
                    if (memberType.SpecialType is SpecialType.System_DateTime)
                    {
                        //OK
                    }
                    else if (Helper.IsTimeSpanType(memberType))
                    {
                        //OK
                    }
                    else
                    {
                        spc.ReportDiagnostic(
                            LightProtoGeneratorWarning.CompatibilityLevel240_Only_Support_DateTime_TimeSpan(
                                $"{targetType}.{member.Name}",
                                member.Locations
                            )
                        );
                    }
                }
                else if (compatibilityLevel == CompatibilityLevel.Level300)
                {
                    if (memberType.SpecialType is SpecialType.System_Decimal)
                    {
                        //OK
                    }
                    else if (Helper.IsGuidType(memberType))
                    {
                        //OK
                    }
                    else
                    {
                        spc.ReportDiagnostic(
                            LightProtoGeneratorWarning.CompatibilityLevel300_Only_Support_Decimal_Guid(
                                $"{targetType}.{member.Name}",
                                member.Locations
                            )
                        );
                    }
                }
            }

            if (
#pragma warning disable CS0618 // Type or member is obsolete
                dataFormat is DataFormat.WellKnown
#pragma warning restore CS0618 // Type or member is obsolete
                && compatibilityLevel <= CompatibilityLevel.Level200)
            {
                compatibilityLevel = CompatibilityLevel.Level240;
            }

            AttributeData? protoMapAttr = member
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "LightProto.ProtoMapAttribute");

            if (protoMapAttr is not null && Helper.IsDictionaryType(compilation, memberType) == false)
            {
                spc.ReportDiagnostic(
                    LightProtoGeneratorWarning.Member_ProtoMapAttribute_But_Not_Dictionary($"{targetType}.{member.Name}", member.Locations)
                );
            }

            var keyFormat = Enum.TryParse<DataFormat>(
                protoMapAttr?.NamedArguments.FirstOrDefault(kv => kv.Key == "KeyFormat").Value.Value?.ToString(),
                out var _keyFormat
            )
                ? _keyFormat
                : DataFormat.Default;

            var valueFormat = Enum.TryParse<DataFormat>(
                protoMapAttr?.NamedArguments.FirstOrDefault(kv => kv.Key == "ValueFormat").Value.Value?.ToString(),
                out var _valueFormat
            )
                ? _valueFormat
                : DataFormat.Default;

            if (memberType is INamedTypeSymbol { TypeArguments: { Length: 2 } typeArguments })
            {
                var keyType = typeArguments[0];
                if (keyFormat is DataFormat.ZigZag && !Helper.SupportZigZag(keyType))
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_ProtoMap_Key_DataFormat_ZigZag_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
                if (keyFormat is DataFormat.FixedSize && !Helper.SupportFixedSize(keyType))
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_ProtoMap_Key_DataFormat_FixedSize_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }

                var valueType = typeArguments[1];
                if (valueFormat is DataFormat.ZigZag && !Helper.SupportZigZag(valueType))
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_ProtoMap_Value_DataFormat_ZigZag_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }

                if (valueFormat is DataFormat.FixedSize && !Helper.SupportFixedSize(valueType))
                {
                    spc.ReportDiagnostic(
                        LightProtoGeneratorWarning.Member_ProtoMap_Value_DataFormat_FixedSize_Not_Supported_Type(
                            $"{targetType}.{member.Name}",
                            member.Locations
                        )
                    );
                }
            }

            if (members.Any(o => o.FieldNumber == tag))
            {
                throw LightProtoGeneratorException.Member_Tag_Duplicated(member.Name, tag, memberDeclarationSyntax.GetLocation());
            }

            members.Add(
                new ProtoMember
                {
                    Name = memberName,
                    Type = memberType,
                    ContractType = targetType,
                    FieldNumber = tag,
                    Compilation = compilation,
                    IsRequired = isRequired,
                    IsProtoMemberRequired = isProtoMemberRequired,
                    IsInitOnly = isInitOnly,
                    AttributeData = member.GetAttributes(),
                    DataFormat = dataFormat,
                    MapFormat = (keyFormat, valueFormat),
                    ProxyType = ProxyType,
                    Initializer = initializer,
                    IsPacked = isPacked,
                    CompatibilityLevel = compatibilityLevel,
                    IsReadOnly = isReadOnly,
                    StringIntern = stringIntern,
                    DeclarationSyntax = memberDeclarationSyntax,
                }
            );
        }

        return members.OrderBy(m => m.FieldNumber).ToList();
    }
}
