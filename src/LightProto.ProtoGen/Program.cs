using Google.Protobuf.Reflection;
using ProtoBuf.Reflection;

namespace LightProto.ProtoGen;

/// <summary>
/// lightproto-gen – generates LightProto [ProtoContract] C# classes from .proto files.
///
/// Usage:
///   lightproto-gen --proto &lt;path.proto&gt; [--proto &lt;other.proto&gt;...] [--output &lt;dir&gt;] [--namespace &lt;ns&gt;]
///
/// Options:
///   --proto      &lt;file&gt;   Path to a .proto file (repeatable).
///   --output     &lt;dir&gt;    Output directory for generated .cs files. Defaults to current directory.
///   --namespace  &lt;ns&gt;     Override the C# namespace for all generated files.
///   --help, -h            Print this help message.
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        var protoFiles = new List<string>();
        string outputDir = ".";
        string? namespaceOverride = null;

        // Simple argument parser
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--help":
                case "-h":
                    PrintHelp();
                    return 0;

                case "--proto":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.Error.WriteLine("error: --proto requires a file path argument.");
                        return 1;
                    }
                    protoFiles.Add(args[i]);
                    break;

                case "--output":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.Error.WriteLine("error: --output requires a directory path argument.");
                        return 1;
                    }
                    outputDir = args[i];
                    break;

                case "--namespace":
                    i++;
                    if (i >= args.Length)
                    {
                        Console.Error.WriteLine("error: --namespace requires a namespace argument.");
                        return 1;
                    }
                    namespaceOverride = args[i];
                    break;

                default:
                    Console.Error.WriteLine($"error: Unknown argument '{args[i]}'. Use --help for usage.");
                    return 1;
            }
        }

        if (protoFiles.Count == 0)
        {
            Console.Error.WriteLine("error: At least one --proto <file> argument is required.");
            PrintHelp();
            return 1;
        }

        // Validate input files
        foreach (var protoFile in protoFiles)
        {
            if (!File.Exists(protoFile))
            {
                Console.Error.WriteLine($"error: Proto file not found: {protoFile}");
                return 1;
            }
        }

        // Ensure output directory exists
        Directory.CreateDirectory(outputDir);

        int exitCode = 0;
        var generator = new LightProtoCSharpGenerator();

        foreach (var protoFile in protoFiles)
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

    private static string GenerateFromProtoFile(string protoFile, LightProtoCSharpGenerator generator, string? namespaceOverride)
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

    private static void PrintHelp()
    {
        Console.WriteLine(
            """
            lightproto-gen – Generate LightProto [ProtoContract] C# classes from .proto files.

            Usage:
              lightproto-gen --proto <file> [--proto <file>...] [--output <dir>] [--namespace <ns>]

            Options:
              --proto      <file>   Path to a .proto file (repeatable).
              --output     <dir>    Output directory for generated .cs files.
                                    Defaults to the current directory.
              --namespace  <ns>     Override the C# namespace for all generated files.
                                    If omitted, the csharp_namespace option from the .proto
                                    file is used; otherwise the file name is used.
              --help, -h            Print this message and exit.

            Examples:
              lightproto-gen --proto messages.proto --output ./Generated
              lightproto-gen --proto a.proto --proto b.proto --output ./Out --namespace MyApp.Models
            """
        );
    }
}
