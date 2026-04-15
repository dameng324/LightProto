using System.CommandLine;
using System.CommandLine.Parsing;
using Google.Protobuf.Reflection;
using LightProto.ProtoGen;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using ProtoBuf.Reflection;

var protoOption = new Option<string[]>("--proto")
{
    Description =
        "Path(s) to .proto file(s). Supports glob patterns such as *.proto or **/*.proto. "
        + "If omitted, proto schema is read from stdin.",
    AllowMultipleArgumentsPerToken = false,
};
protoOption.Arity = ArgumentArity.ZeroOrMore;

var outputOption = new Option<string?>("--output")
{
    Description = "Output directory for generated .cs files. " + "If omitted, generated code is written to stdout.",
};

var namespaceOption = new Option<string?>("--namespace")
{
    Description =
        "Override C# namespace for all generated files. "
        + "If omitted, the csharp_namespace option from the .proto file is used; "
        + "otherwise the file name is used.",
};

var typeShapeOption = new Option<TypeShape>("--type-shape")
{
    Description =
        "C# type shape for generated message types. "
        + "Accepted values: Default (partial class), Record (partial record), "
        + "Struct (partial struct), RecordStruct (partial record struct).",
    DefaultValueFactory = _ => TypeShape.Default,
};

var nullabilityOption = new Option<NullabilityMode>("--nullability")
{
    Description =
        "Nullability inference rule for non-repeated, non-map fields. "
        + "Default: every field is nullable (all proto3 fields are optional on the wire). "
        + "StrictOptional: only fields explicitly declared with the 'optional' keyword are nullable.",
    DefaultValueFactory = _ => NullabilityMode.Default,
};

var oneofOption = new Option<OneofHandling>("--oneof")
{
    Description =
        "How oneof groups are translated to C# members. "
        + "Default: all oneof fields are emitted as nullable properties. "
        + "ProtoInclude: a oneof group whose fields are all message-typed is promoted to "
        + "[ProtoInclude] attributes on the containing type.",
    DefaultValueFactory = _ => OneofHandling.Default,
};

var rootCommand = new RootCommand { Description = "lightproto-gen - Generate LightProto [ProtoContract] C# classes from .proto files." };
rootCommand.Options.Add(protoOption);
rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(namespaceOption);
rootCommand.Options.Add(typeShapeOption);
rootCommand.Options.Add(nullabilityOption);
rootCommand.Options.Add(oneofOption);

rootCommand.SetAction(
    (ParseResult parseResult) =>
    {
        var protoPatterns = parseResult.GetValue(protoOption) ?? [];
        var outputDir = parseResult.GetValue(outputOption);
        var namespaceOverride = parseResult.GetValue(namespaceOption);
        var typeShape = parseResult.GetValue(typeShapeOption);
        var nullability = parseResult.GetValue(nullabilityOption);
        var oneofHandling = parseResult.GetValue(oneofOption);

        var options = new GeneratorOptions
        {
            TypeShape = typeShape,
            Nullability = nullability,
            OneofHandling = oneofHandling,
        };

        var generator = new LightProtoCSharpGenerator(options);
        bool useStdout = outputDir is null;

        // --------------- stdin mode: no --proto patterns supplied ---------------
        if (protoPatterns.Length == 0)
        {
            if (!Console.IsInputRedirected)
            {
                Console.Error.WriteLine(
                    "error: No .proto files specified and no input on stdin. " + "Provide --proto <file> or pipe proto schema to stdin."
                );
                return 1;
            }

            string stdinContent;
            try
            {
                stdinContent = Console.In.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: Failed to read stdin: {ex.Message}");
                return 1;
            }

            try
            {
                var code = GenerateFromProtoSchema(stdinContent, "stdin.proto", generator, namespaceOverride);
                if (useStdout)
                {
                    Console.Write(code);
                }
                else
                {
                    Directory.CreateDirectory(outputDir!);
                    var outputPath = Path.Combine(outputDir!, "stdin.g.cs");
                    File.WriteAllText(outputPath, code, System.Text.Encoding.UTF8);
                    Console.Error.WriteLine($"Generated: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: Failed to generate from stdin: {ex.Message}");
                return 1;
            }

            return 0;
        }

        // --------------- file mode: --proto patterns supplied -------------------
        var resolvedFiles = ResolveProtoFiles(protoPatterns);

        if (resolvedFiles.Count == 0)
        {
            Console.Error.WriteLine("error: No .proto files matched the provided patterns.");
            return 1;
        }

        if (!useStdout)
            Directory.CreateDirectory(outputDir!);

        int exitCode = 0;

        foreach (var protoFile in resolvedFiles)
        {
            try
            {
                var code = GenerateFromProtoFile(protoFile, generator, namespaceOverride);

                if (useStdout)
                {
                    Console.Write(code);
                }
                else
                {
                    var outputFileName = Path.GetFileNameWithoutExtension(protoFile) + ".g.cs";
                    var outputPath = Path.Combine(outputDir!, outputFileName);
                    File.WriteAllText(outputPath, code, System.Text.Encoding.UTF8);
                    Console.Error.WriteLine($"Generated: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: Failed to process '{protoFile}': {ex.Message}");
                exitCode = 1;
            }
        }

        return exitCode;
    }
);

return rootCommand.Parse(args).Invoke();

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

static List<string> ResolveProtoFiles(string[] patterns)
{
    var results = new List<string>();

    foreach (var pattern in patterns)
    {
        // Normalize path separators
        var normalized = pattern.Replace('\\', '/');

        // If the pattern contains no glob characters, treat it as a literal path
        if (!normalized.Contains('*') && !normalized.Contains('?') && !normalized.Contains('['))
        {
            var fullPath = Path.GetFullPath(pattern);
            if (File.Exists(fullPath))
                results.Add(fullPath);
            else
                Console.Error.WriteLine($"warning: Proto file not found: {pattern}");
            continue;
        }

        // Determine the search root: the non-glob prefix of the pattern
        var searchRoot = GetGlobRoot(normalized);
        var relativePattern = normalized.Length > searchRoot.Length ? normalized[searchRoot.Length..].TrimStart('/') : "**/*.proto";

        var matcher = new Matcher();
        matcher.AddInclude(relativePattern);

        var rootDir = string.IsNullOrEmpty(searchRoot) ? Directory.GetCurrentDirectory() : Path.GetFullPath(searchRoot);

        if (!Directory.Exists(rootDir))
        {
            Console.Error.WriteLine($"warning: Directory not found for pattern '{pattern}': {rootDir}");
            continue;
        }

        var dirWrapper = new DirectoryInfoWrapper(new DirectoryInfo(rootDir));
        var matchResult = matcher.Execute(dirWrapper);

        foreach (var file in matchResult.Files)
            results.Add(Path.GetFullPath(Path.Combine(rootDir, file.Path)));
    }

    // Deduplicate while preserving order
    return results.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
}

static string GetGlobRoot(string pattern)
{
    // Return the directory prefix before the first glob character
    var firstGlobIdx = pattern.IndexOfAny(['*', '?', '[']);
    if (firstGlobIdx < 0)
        return Path.GetDirectoryName(pattern) ?? "";

    var prefix = pattern[..firstGlobIdx];
    var lastSep = prefix.LastIndexOf('/');
    return lastSep < 0 ? "" : prefix[..lastSep];
}

static string GenerateFromProtoSchema(string protoSchema, string fileName, LightProtoCSharpGenerator generator, string? namespaceOverride)
{
    var set = new FileDescriptorSet();
    using var reader = new StringReader(protoSchema);
    set.Add(fileName, source: reader);
    set.Process();

    var errors = set.GetErrors();
    if (errors.Length > 0)
    {
        var messages = string.Join(Environment.NewLine, errors.Select(e => e.ToString()));
        throw new InvalidOperationException($"Proto parse errors:{Environment.NewLine}{messages}");
    }

    var file = set.Files.FirstOrDefault(f => f.Name == fileName);
    if (file is null)
        throw new InvalidOperationException($"Proto file '{fileName}' was not found in the parsed set.");

    return generator.Generate(file, namespaceOverride);
}

static string GenerateFromProtoFile(string protoFile, LightProtoCSharpGenerator generator, string? namespaceOverride)
{
    var set = new FileDescriptorSet();

    // Add the file's directory as an import path so relative imports resolve
    var importPath = Path.GetDirectoryName(Path.GetFullPath(protoFile));
    if (!string.IsNullOrEmpty(importPath))
        set.AddImportPath(importPath);

    using var reader = new StreamReader(protoFile, System.Text.Encoding.UTF8);
    set.Add(Path.GetFileName(protoFile), source: reader);
    set.Process();

    var errors = set.GetErrors();
    if (errors.Length > 0)
    {
        var messages = string.Join(Environment.NewLine, errors.Select(e => e.ToString()));
        throw new InvalidOperationException($"Proto parse errors:{Environment.NewLine}{messages}");
    }

    var file = set.Files.FirstOrDefault(f => f.Name == Path.GetFileName(protoFile));
    if (file is null)
        throw new InvalidOperationException($"Proto file '{protoFile}' was not found in the parsed set.");

    return generator.Generate(file, namespaceOverride);
}
