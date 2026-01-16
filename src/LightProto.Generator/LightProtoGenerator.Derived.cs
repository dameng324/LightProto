using System.Text;
using Microsoft.CodeAnalysis;
using SourceProductionContext = Microsoft.CodeAnalysis.SourceProductionContext;
using SymbolDisplayFormat = Microsoft.CodeAnalysis.SymbolDisplayFormat;

namespace LightProto.Generator;

public partial class LightProtoGenerator
{
    private void GenerateDerivedClassBody(
        CodeWriter writer,
        ProtoContract contract,
        SourceProductionContext spc,
        string typeDeclarationString,
        bool net8OrGreater
    )
    {
        var targetType = contract.Type;
        var compilation = contract.Compilation;
        var className = targetType.Name;
        var proxyFor = contract.ProxyFor;
        bool skipConstructor = contract.SkipConstructor;
        var protoMembers = contract.Members;

        var proxyOrClassName = proxyFor?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? className;
        var derivedTypes = contract.DerivedTypeContracts.ToList();
        INamedTypeSymbol? baseType = Helper.GetBaseProtoType(targetType);

        string baseParserTypeName =
            baseType?.TypeKind is TypeKind.Interface
                ? baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "ProtoParser"
                : baseType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    ?? targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        writer.WriteLine(
            $"{typeDeclarationString} {className}{(targetType.TypeKind is TypeKind.Interface ? "ProtoParser" : "")} :IProtoParser<{proxyOrClassName}>"
        );
        using (writer.IndentScope())
        {
            if (targetType.TypeKind is TypeKind.Interface)
            {
                writer.WriteLine($"[System.Runtime.CompilerServices.ModuleInitializer]");
                writer.WriteLine(
                    $"internal static void RegisterParser() => LightProto.Serializer.RegisterParser<{targetType}>(ProtoReader, ProtoWriter);"
                );
            }

            #region ProtoReader/ProtoWriter

            writer.WriteLine($"public static new IProtoReader<{proxyOrClassName}> ProtoReader {{get;}} = new LightProtoReader();");

            if (baseType is null || targetType.TypeKind is TypeKind.Struct)
                writer.WriteLine($"public static IProtoWriter<{proxyOrClassName}> ProtoWriter {{get;}} = new LightProtoWriter();");
            else
                writer.WriteLine($"public static new IProtoWriter<{proxyOrClassName}> ProtoWriter => {baseParserTypeName}.ProtoWriter;");

            writer.WriteLine(
                "internal static new IProtoReader<MemberStruct> MemberStructReader {get; } = new MemberStructLightProtoReader();"
            );
            writer.WriteLine(
                "internal static new IProtoWriter<MemberStruct> MemberStructWriter {get; } = new MemberStructLightProtoWriter();"
            );

            #endregion

            // Write LightProtoReader
            writer.WriteLine($"internal sealed new class LightProtoReader: IProtoReader, IProtoReader<{proxyOrClassName}>");
            using (writer.IndentScope())
            {
                writer.WriteLine("object IProtoReader.ParseFrom(ref ReaderContext input) => ParseFrom(ref input);");
                writer.WriteLine("public bool IsMessage => true;");
                writer.WriteLine("public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;");
                writer.WriteLine(
                    baseType is null
                        ? $"public {proxyOrClassName} ParseFrom(ref ReaderContext input) => MemberStructReader.ParseFrom(ref input).ToMessage();"
                        : $"public {proxyOrClassName} ParseFrom(ref ReaderContext input) => ({proxyOrClassName}){baseParserTypeName}.ProtoReader.ParseFrom(ref input);"
                );
            }

            if (baseType is null)
            {
                GenDerivedLightProtoWriter(writer, proxyOrClassName, "MemberStructWriter", "MemberStruct.FromMessage(message)");
            }
            else if (targetType.TypeKind is TypeKind.Struct)
            {
                GenDerivedLightProtoWriter(writer, proxyOrClassName, $"{baseParserTypeName}.ProtoWriter", "message");
            }

            // Write MemberStruct
            writer.WriteLine("internal new struct MemberStruct");
            using (writer.IndentScope())
            {
                // define members
                foreach (var member in protoMembers)
                {
                    var nullableSuffix = member.Type.IsValueType ? "?" : "";
                    writer.WriteLine($"public {member.Type}{nullableSuffix} {member.Name};");
                }

                foreach (var member in derivedTypes)
                {
                    writer.WriteLine($"public {member.Contract.Type}.MemberStruct? {member.Contract.Type.Name}_MemberStruct;");
                }

                // define MemberStruct.FromMessage
                writer.WriteLine($"public static MemberStruct FromMessage({className} message)");
                using (writer.IndentScope())
                {
                    writer.WriteLine("var memberStruct = new MemberStruct()");
                    using (writer.IndentScope(braceEnd: "};"))
                    {
                        foreach (var member in protoMembers)
                        {
                            writer.WriteLine($"{member.Name}=message.{member.Name},");
                        }
                    }

                    for (var index = 0; index < derivedTypes.Count; index++)
                    {
                        var member = derivedTypes[index];
                        writer.WriteLine($"if (message is {member.Contract.Type} derived{index})");
                        using (writer.IndentScope())
                        {
                            writer.WriteLine(
                                $"memberStruct.{member.Contract.Type.Name}_MemberStruct = {member.Contract.Type}.MemberStruct.FromMessage(derived{index});"
                            );
                        }
                    }

                    writer.WriteLine("return memberStruct;");
                }

                // define MemberStruct.ToMessage
                if (baseType is null)
                {
                    writer.WriteLine($"public {className} ToMessage()");
                    using (writer.IndentScope())
                    {
                        foreach (var member in derivedTypes)
                        {
                            writer.WriteLine($"if({member.Contract.Type.Name}_MemberStruct.HasValue)");
                            writer.WriteLine(
                                $"    return {member.Contract.Type}.MemberStruct.ToMessage(this,{member.Contract.Type.Name}_MemberStruct.Value);"
                            );
                        }

                        if (targetType.TypeKind is TypeKind.Interface || targetType.IsAbstract)
                        {
                            writer.WriteLine(
                                $"throw new InvalidOperationException(\"Unknown sub type of {targetType} when Deserializing\");"
                            );
                        }
                        else
                        {
                            GenerateBaseDerivedTypeConstructor(writer, contract, net8OrGreater, skipConstructor, className, protoMembers);
                        }
                    }
                }
                else
                {
                    var rootBaseType = GetRootProtoBaseClass(baseType);
                    string rootBaseTypeMemberStructName = $"{rootBaseType}.MemberStruct";
                    if (rootBaseType.TypeKind is TypeKind.Interface)
                    {
                        rootBaseTypeMemberStructName = $"{rootBaseType}ProtoParser.MemberStruct";
                    }

                    writer.WriteLine(
                        $"public static {className} ToMessage(in {rootBaseTypeMemberStructName} rootMemberStruct, in MemberStruct memberStruct)"
                    );
                    using (writer.IndentScope()) //ToMessage
                    {
                        foreach (var member in derivedTypes)
                        {
                            writer.WriteLine(
                                $"if(memberStruct.{member.Contract.Type.Name}_MemberStruct.HasValue) return {member.Contract.Type}.MemberStruct.ToMessage(rootMemberStruct,memberStruct.{member.Contract.Type.Name}_MemberStruct.Value);"
                            );
                        }

                        var currentBaseType = baseType;
                        List<INamedTypeSymbol> derivedLevelTypes = new();
                        while (true)
                        {
                            if (currentBaseType is null || !Helper.IsProtoBufMessage(currentBaseType))
                            {
                                break;
                            }

                            derivedLevelTypes.Add(currentBaseType);
                            currentBaseType = currentBaseType.BaseType;
                        }

                        derivedLevelTypes.Reverse();

                        List<ProtoMember> allMembers = new();
                        Dictionary<string, string> memberAccesses = new();
                        foreach (var member in protoMembers)
                        {
                            allMembers.Add(member);
                            memberAccesses[member.Name] = $"memberStruct.{member.Name}";
                        }

                        var memberStructNameBuilder = new StringBuilder("rootMemberStruct");
                        for (var index = 0; index < derivedLevelTypes.Count; index++)
                        {
                            var derivedType = derivedLevelTypes[index];

                            if (index > 0)
                            {
                                memberStructNameBuilder.Append($".{derivedType.Name}_MemberStruct.Value");
                            }

                            var memberStructName = memberStructNameBuilder.ToString();

                            var baseProtoMembers = ProtoContract.GetProtoContract(compilation, derivedType, spc)!.Members;
                            foreach (var member in baseProtoMembers)
                            {
                                allMembers.Add(member);
                                memberAccesses[member.Name] = $"{memberStructName}.{member.Name}";
                            }
                        }

                        if (skipConstructor)
                        {
                            GenerateSkipConstructor(
                                writer,
                                net8OrGreater,
                                className,
                                allMembers,
                                getMemberAccess: member =>
                                    member.Type.IsValueType ? $"{memberAccesses[member.Name]}.Value" : $"{memberAccesses[member.Name]}",
                                getNullCheck: member =>
                                    member.Type.IsValueType
                                        ? $"{memberAccesses[member.Name]}.HasValue"
                                        : $"{memberAccesses[member.Name]} != null"
                            );
                            writer.WriteLine("return parsed;");
                        }
                        else
                        {
                            GenerateGeneralConstructor(
                                writer,
                                net8OrGreater,
                                className,
                                contract,
                                allMembers,
                                getMemberAccess: member =>
                                    member.Type.IsValueType ? $"{memberAccesses[member.Name]}.Value" : $"{memberAccesses[member.Name]}",
                                getNullCheck: member =>
                                    member.Type.IsValueType
                                        ? $"{memberAccesses[member.Name]}.HasValue"
                                        : $"{memberAccesses[member.Name]} != null"
                            );
                            writer.WriteLine("return parsed;");
                        }
                    }
                }
            }

            writer.WriteLine("internal sealed new class MemberStructLightProtoWriter: IProtoWriter<MemberStruct>");
            using (writer.IndentScope())
            {
                writer.WriteLine("public bool IsMessage => true;");
                writer.WriteLine("public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;");
                GenerateMemberProtoParsers(writer, protoMembers, compilation, targetType, "Writer");
                GenerateMemberStructWriteToMethod(writer, protoMembers, compilation, derivedTypes);
                GenerateMemberStructCalculateSizeMethod(writer, protoMembers, compilation, derivedTypes);
            }

            writer.WriteLine("internal sealed new class MemberStructLightProtoReader: IProtoReader<MemberStruct>");
            using (writer.IndentScope())
            {
                writer.WriteLine("public bool IsMessage => true;");
                writer.WriteLine("public WireFormat.WireType WireType => WireFormat.WireType.LengthDelimited;");
                GenerateMemberProtoParsers(writer, protoMembers, compilation, targetType, "Reader");

                writer.WriteLine("public MemberStruct ParseFrom(ref ReaderContext input)");
                ;
                using (writer.IndentScope())
                {
                    foreach (var member in protoMembers)
                    {
                        var nullableSuffix = member.Type.IsValueType ? "?" : "";
                        writer.WriteLine($"{member.Type}{nullableSuffix} _{member.Name} = null;");
                    }

                    foreach (var member in derivedTypes)
                    {
                        writer.WriteLine($"{member.Contract.Type}.MemberStruct? _{member.Contract.Type.Name}_memberStruct = null;");
                    }

                    writer.WriteLine("uint tag;");
                    writer.WriteLine("while ((tag = input.ReadTag()) != 0) ");
                    using (writer.IndentScope())
                    {
                        writer.WriteLine("if ((tag & 7) == 4)");
                        using (writer.IndentScope())
                        {
                            writer.WriteLine("break;");
                        }

                        writer.WriteLine("switch(tag) ");
                        using (writer.IndentScope())
                        {
                            writer.WriteLine("default: ");
                            using (writer.IndentScope())
                            {
                                writer.WriteLine("input.SkipLastField();");
                                writer.WriteLine("break;");
                            }

                            foreach (var member in protoMembers)
                            {
                                writer.WriteLine($"case {member.RawTag}:");
                                if (Helper.IsCollectionType(compilation, member.Type))
                                {
                                    var elementType = Helper.GetElementType(compilation, member.Type);
                                    var tag2 = ProtoMember.GetRawTag(
                                        member.FieldNumber,
                                        ProtoMember.GetPbWireType(compilation, elementType, member.DataFormat)
                                    );
                                    if (tag2 != member.RawTag)
                                    {
                                        writer.WriteLine($"case {tag2}:");
                                    }
                                }

                                using (writer.IndentScope())
                                {
                                    if (TryGetInternalTypeName(member.Type, member.DataFormat, member.StringIntern, out var name))
                                    {
                                        writer.WriteLine($"_{member.Name} = input.Read{name}();");
                                    }
                                    else if (
                                        Helper.IsCollectionType(compilation, member.Type)
                                        || Helper.IsDictionaryType(compilation, member.Type)
                                    )
                                    {
                                        writer.WriteLine($"_{member.Name} = {member.Name}_ProtoReader.ParseFrom(ref input);");
                                    }
                                    else
                                    {
                                        writer.WriteLine($"_{member.Name} = {member.Name}_ProtoReader.ParseMessageFrom(ref input);");
                                    }

                                    writer.WriteLine($"break;");
                                }
                            }

                            foreach (var member in derivedTypes)
                            {
                                writer.WriteLine($"case {member.RawTag}:");
                                using (writer.IndentScope())
                                {
                                    writer.WriteLine(
                                        $"_{member.Contract.Type.Name}_memberStruct = {member.Contract.Type}.MemberStructReader.ParseMessageFrom(ref input);"
                                    );
                                    writer.WriteLine("break;");
                                }
                            }
                        }
                    }

                    writer.WriteLine("var parsed = new MemberStruct()");
                    using (writer.IndentScope(braceEnd: "};"))
                    {
                        foreach (var member in protoMembers)
                        {
                            writer.WriteLine($"{member.Name} = _{member.Name},");
                        }

                        foreach (var member in derivedTypes)
                        {
                            writer.WriteLine($"{member.Contract.Type.Name}_MemberStruct = _{member.Contract.Type.Name}_memberStruct,");
                        }
                    }

                    writer.WriteLine("return parsed;");
                }
            }
        }
    }

    private static void GenerateBaseDerivedTypeConstructor(
        CodeWriter writer,
        ProtoContract contract,
        bool net8OrGreater,
        bool skipConstructor,
        string className,
        List<ProtoMember> protoMembers
    )
    {
        if (skipConstructor)
        {
            GenerateSkipConstructor(
                writer,
                net8OrGreater,
                className,
                protoMembers,
                (member) => member.Type.IsValueType ? $"{member.Name}.Value" : member.Name,
                (member) => member.Type.IsValueType ? $"{member.Name}.HasValue" : $"{member.Name} != null"
            );
            writer.WriteLine("return parsed;");
        }
        else
        {
            GenerateGeneralConstructor(
                writer,
                net8OrGreater,
                className,
                contract,
                protoMembers,
                getMemberAccess: (member) => member.Name,
                getNullCheck: (member) => member.Type.IsValueType ? $"{member.Name}.HasValue" : $"{member.Name} != null"
            );
            writer.WriteLine("return parsed;");
        }
    }

    private void GenerateMemberStructCalculateSizeMethod(
        CodeWriter writer,
        List<ProtoMember> protoMembers,
        Compilation compilation,
        List<(uint RawTag, ProtoContract Contract)> derivedTypes
    )
    {
        GenerateCommonCalculateSizeMethod(
            writer,
            "MemberStruct",
            protoMembers,
            compilation,
            "MemberStruct",
            member => member.Type.IsValueType ? $"message.{member.Name}.Value" : $"message.{member.Name}",
            member => member.Type.IsValueType ? $"message.{member.Name}.HasValue" : $"message.{member.Name} != null",
            derivedTypes
        );
    }

    private void GenerateMemberStructWriteToMethod(
        CodeWriter writer,
        List<ProtoMember> protoMembers,
        Compilation compilation,
        List<(uint RawTag, ProtoContract Contract)> derivedTypes
    )
    {
        GenerateCommonWriteToMethod(
            writer,
            "MemberStruct",
            protoMembers,
            compilation,
            "MemberStruct",
            member => member.Type.IsValueType ? $"message.{member.Name}.Value" : $"message.{member.Name}",
            member => member.Type.IsValueType ? $"message.{member.Name}.HasValue" : $"message.{member.Name} != null",
            derivedTypes
        );
    }

    private static INamedTypeSymbol GetRootProtoBaseClass(INamedTypeSymbol type)
    {
        if (type.BaseType is null)
        {
            return type;
        }

        if (type.AllInterfaces.FirstOrDefault(Helper.IsProtoBufMessage) is { } it)
        {
            return it;
        }

        if (!Helper.IsProtoBufMessage(type.BaseType))
        {
            return type;
        }

        return GetRootProtoBaseClass(type.BaseType);
    }
}
