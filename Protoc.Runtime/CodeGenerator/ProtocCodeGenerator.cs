using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Protoc.Runtime.CodeGenerator;

/// <summary>
/// Service for generating C# code from Protocol Buffer files using protoc.
/// </summary>
public class ProtocCodeGenerator : ICodeGenerator
{
    /// <summary>
    /// Initializes static members of the <see cref="ProtocCodeGenerator"/> class.
    /// Detects the target directory based on the platform and architecture.
    /// </summary>
    static ProtocCodeGenerator()
    {
        Architecture architecture = RuntimeInformation.ProcessArchitecture;

        Assembly assembly = typeof(ProtocCodeGenerator).Assembly;

        string targetDirectory = assembly.GetName().Name + ".Tools.";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && architecture is Architecture.X64 or Architecture.X86)
        {
            targetDirectory += "windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && architecture is Architecture.X64 or Architecture.X86 or Architecture.Arm64)
        {
            targetDirectory += "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && architecture is Architecture.X64)
        {
            targetDirectory += "macosx";
        }
        else
        {
            ThrowNotSupported();
        }

        targetDirectory += "_" + architecture.ToString().ToLowerInvariant();

        ImmutableArray<string> protocFiles = assembly
            .GetManifestResourceNames()
            .Where(c => c.StartsWith(targetDirectory))
            .ToImmutableArray();

        string grpcPluginPath = protocFiles[0];
        string protocPath = protocFiles[1];

        DirectoryInfo toolDir = Directory.CreateDirectory("Tools");

        s_grpcPluginPath = DetectBinPath(assembly, grpcPluginPath, toolDir.FullName);
        s_protocPath = DetectBinPath(assembly, protocPath, toolDir.FullName);
    }

    private static string DetectBinPath(Assembly assembly, string resourcePath, string targetDirPath)
    {
        using Stream recourseStream = assembly.GetManifestResourceStream(resourcePath)!;
        using FileStream fileStream = File.Create(targetDirPath + "/" + resourcePath.Split('.', 4)[^1]);

        recourseStream.Seek(0, SeekOrigin.Begin);
        recourseStream.CopyTo(fileStream);

        return fileStream.Name;
    }

    private static void ThrowNotSupported()
    {
        throw new PlatformNotSupportedException(RuntimeInformation.OSDescription + " " + RuntimeInformation.OSArchitecture);
    }

    private static string s_protocPath;
    private static string s_grpcPluginPath;

    /// <summary>
    /// Generates C# code from Protocol Buffer files asynchronously.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The list of Protocol Buffer files to generate code from.</param>
    /// <returns>A collection of generated C# code as strings.</returns>
    Task<IReadOnlyCollection<string>> ICodeGenerator.GenerateCodeFromProto(string root, params string[] files) => GenerateCodeFromProtoImpl(root, files);

    /// <summary>
    /// Generates C# code from Protocol Buffer files asynchronously.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The collection of Protocol Buffer files to generate code from.</param>
    /// <returns>A collection of generated C# code as strings.</returns>
    Task<IReadOnlyCollection<string>> ICodeGenerator.GenerateCodeFromProto(string root, IEnumerable<string> files) => GenerateCodeFromProtoImpl(root, files);

    private async Task<IReadOnlyCollection<string>> GenerateCodeFromProtoImpl(string root, IEnumerable<string> files)
    {
        DirectoryInfo tmp = Directory.CreateDirectory(Ulid.NewUlid().ToString());

        string cmd = "--plugin=protoc-gen-grpc="
            + s_grpcPluginPath + " "
            + "--csharp_out=" + tmp.FullName + " "
            + "--grpc_out=" + tmp.FullName + " "
            + "-I " + root + " "
            + string.Join(" " + root + "/", files);

        Process process = new()
        {
            StartInfo = new ProcessStartInfo(s_protocPath, cmd)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new GenerateCodeFromProtoException(await process.StandardError.ReadToEndAsync());
        }

        DirectoryInfo directoryInfo = new(tmp.FullName);

        FileInfo[] generatedFiles = directoryInfo.GetFiles();

        List<string> code = new(generatedFiles.Length);

        foreach (FileInfo generatedFile in generatedFiles)
        {
            FileStream fileStream = generatedFile.OpenRead();

            using StreamReader reader = new(fileStream);

            code.Add(await reader.ReadToEndAsync());
        }

        return code;
    }
}
