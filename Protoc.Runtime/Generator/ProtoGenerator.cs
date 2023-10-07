using Protoc.Runtime.Generator.CodeCompiler;
using Protoc.Runtime.Generator.CodeGenerator;

using System.Reflection;

namespace Protoc.Runtime.Generator;

/// <summary>
/// Class responsible for generating assemblies from Protocol Buffer files.
/// </summary>
public class ProtoGenerator : IProtoGenerator
{
    /// <summary>
    /// Creates a new instance of <see cref="ProtoGenerator"/> with default implementations of the code compiler and generator.
    /// </summary>
    /// <returns>A new instance of <see cref="ProtoGenerator"/> with default implementations.</returns>
    public static ProtoGenerator CreateDefault() => new(new CSharpCodeCompiler(), new ProtocCodeGenerator());

    private readonly ICodeCompiler _codeCompiler;
    private readonly ICodeGenerator _codeGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoGenerator"/> class.
    /// </summary>
    /// <param name="codeCompiler">The code compiler instance to use for compiling code.</param>
    /// <param name="codeGenerator">The code generator instance to use for generating code.</param>
    public ProtoGenerator(ICodeCompiler codeCompiler, ICodeGenerator codeGenerator)
    {
        _codeCompiler = codeCompiler;
        _codeGenerator = codeGenerator;
    }

    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="directories">The directories containing the Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> IProtoGenerator.GenerateAll(string root, IEnumerable<string> directories)
    {
        IEnumerable<string> files = directories.SelectMany(d => new DirectoryInfo(d)
            .GetFiles()
            .Select(f => f.FullName));

        return GenerateImpl(root, files);
    }

    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> IProtoGenerator.Generate(string root, params string[] files)
    {
        return GenerateImpl(root, files);
    }

    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> IProtoGenerator.Generate(string root, IEnumerable<string> files)
    {
        return GenerateImpl(root, files);
    }

    private async Task<Assembly> GenerateImpl(string root, IEnumerable<string> files)
    {
        IReadOnlyCollection<string> code = await _codeGenerator.GenerateCodeFromProto(root, files);

        Assembly assembly = _codeCompiler.CompileCode(code);

        return assembly;
    }
}