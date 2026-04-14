using System.CommandLine;
using System.CommandLine.Parsing;
using Google.Protobuf.Reflection;
using LightProto.ProtoGen;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using ProtoBuf.Reflection;

var protoOption = new Option<string[]>("--proto")
{
    Description = "Path(s) to .proto file(s). Supports glob patterns such as *.proto or **/*.proto.",
    AllowMultipleArgumentsPerToken = false,
};
protoOption.Arity = ArgumentArity.OneOrMore;

var outputOption = new Option<string>("--output")
{
    Description = "Output directory for generated .cs files.",
    DefaultValueFactory = _ => ".",
};

var namespaceOption = new Option<string?>("--namespace")
{
    Description =
        "Override C# namespace for all generated files. "
        + "If omitted, the csharp_namespace option from the .proto file is used; "
        + "otherwise the file name is used.",
};

var strictOptionalOption = new Option<bool>("--strict-optional")
{
    Description =
        "Only treat fields explicitly declared with the 'optional' keyword as nullable. "
        + "By default all non-repeated, non-map fields are nullable because they are optional on the wire.",
};

var useStructOption = new Option<bool>("--struct") { Description = "Emit 'partial struct' instead of 'partial class'." };

var useRecordOption = new Option<bool>("--record")
{
    Description = "Emit 'partial record'. Combined with --struct emits 'partial record struct'.",
};

var oneofAsIncludeOption = new Option<bool>("--oneof-as-include")
{
    Description =
        "Promote oneof groups where all fields are message types to [ProtoInclude] attributes "
        + "on the containing type. By default, oneof fields are always emitted as nullable properties.",
};

var rootCommand = new RootCommand { Description = "lightproto-gen - Generate LightProto [ProtoContract] C# classes from .proto files." };
rootCommand.Options.Add(protoOption);
rootCommand.Options.Add(outputOption);
rootCommand.Options.Add(namespaceOption);
rootCommand.Options.Add(strictOptionalOption);
rootCommand.Options.Add(useStructOption);
rootCommand.Options.Add(useRecordOption);
rootCommand.Options.Add(oneofAsIncludeOption);

rootCommand.SetAction(
    (ParseResult parseResult) =>
    {
        var protoPatterns = parseResult.GetValue(protoOption) ?? [];
        var outputDir = parseResult.GetValue(outputOption) ?? ".";
        var namespaceOverride = parseResult.GetValue(namespaceOption);
        var strictOptional = parseResult.GetValue(strictOptionalOption);
        var useStruct = parseResult.GetValue(useStructOption);
        var useRecord = parseResult.GetValue(useRecordOption);
        var oneofAsInclude = parseResult.GetValue(oneofAsIncludeOption);

        // Expand glob patterns into concrete file paths
        var resolvedFiles = ResolveProtoFiles(protoPatterns);

        if (resolvedFiles.Count == 0)
        {
            Console.Error.WriteLine("error: No .proto files matched the provided patterns.");
            return 1;
        }

        // Ensure output directory exists
        Directory.CreateDirectory(outputDir);

        var options = new GeneratorOptions
        {
            StrictOptional = strictOptional,
            UseStruct = useStruct,
            UseRecord = useRecord,
            UseProtoIncludeForOneof = oneofAsInclude,
        };

        int exitCode = 0;
        var generator = new LightProtoCSharpGenerator(options);

        foreach (var protoFile in resolvedFiles)
        {
            try
            {
                var code = GenerateFromProtoFile(protoFile, generator, namespaceOverride);
                var outputFileName = Path.GetFileNameWithoutExtension(protoFile) + ".g.cs";
                var outputPath = Path.Combine(outputDir, outputFileName);

                File.WriteAllText(outputPath, code, System.Text.Encoding.UTF8);
                Console.WriteLine($"Generated: {outputPath}");
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
