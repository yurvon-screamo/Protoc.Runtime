using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using System.Buffers;
using System.Reflection;
using System.Runtime.Loader;

namespace Protoc.Runtime.CodeCompiler;

/// <summary>
/// Class for compiling C# code and generating an assembly.
/// </summary>
public class CSharpCodeCompiler : ICodeCompiler
{
    /// <summary>`
    /// Compiles the provided code and generates an assembly using an empty set of assembly tags.
    /// </summary>
    /// <param name="code">The C# code to compile.</param>
    /// <returns>The compiled assembly.</returns>
    Assembly ICodeCompiler.CompileCode(IReadOnlyCollection<string> code) => CompileCodeImpl(code, Type.EmptyTypes);

    /// <summary>
    /// Compiles the provided code and generates an assembly using the specified assembly tags.
    /// </summary>
    /// <param name="code">The C# code to compile.</param>
    /// <param name="assemblyTags">The types representing assembly tags.</param>
    /// <returns>The compiled assembly.</returns>
    Assembly ICodeCompiler.CompileCode(IReadOnlyCollection<string> code, IReadOnlyCollection<Type> assemblyTags) => CompileCodeImpl(code, assemblyTags);

    private Assembly CompileCodeImpl(IReadOnlyCollection<string> code, IReadOnlyCollection<Type> assemblyTags)
    {
        string randomName = Ulid.NewUlid().ToString();

        using MemoryStream memoryStream = new();

        CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp11);

        IEnumerable<SyntaxTree> parsedSyntaxTrees = code
            .Select(f => SyntaxFactory.ParseSyntaxTree(f, options));

        IEnumerable<PortableExecutableReference> referencedAssemblies = Assembly
              .GetExecutingAssembly()
              .GetReferencedAssemblies()
              .Select(a => MetadataReference.CreateFromFile(Assembly.Load(a).Location));

        List<MetadataReference> references = new()
        {
            MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Private.CoreLib, Version=7.0.0.0").Location),
            MetadataReference.CreateFromFile(typeof(Google.Protobuf.ByteString).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Grpc.Core.CallInvoker).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ReadOnlySequence<byte>).Assembly.Location),
        };

        references.AddRange(referencedAssemblies);

        foreach (Type assemblyTag in assemblyTags)
        {
            references.Add(MetadataReference.CreateFromFile(assemblyTag.Assembly.Location));
        }

        CSharpCompilationOptions cSharpCompilationOptions = new(
            OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel: OptimizationLevel.Release,
            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);

        EmitResult emitResult = CSharpCompilation
            .Create(randomName,
                parsedSyntaxTrees,
                references: references,
                options: cSharpCompilationOptions)
            .Emit(memoryStream);

        if (!emitResult.Success)
        {
            IEnumerable<Diagnostic> failures = emitResult
                .Diagnostics
                .Where(d => d.IsWarningAsError || d.Severity is DiagnosticSeverity.Error);

            Diagnostic? error = failures.FirstOrDefault();

            throw new InvalidDataException($"{error?.Id}: {error?.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}");
        }

        AssemblyLoadContext assemblyLoadContext = new(randomName);

        memoryStream.Seek(0, SeekOrigin.Begin);

        Assembly assembly = assemblyLoadContext.LoadFromStream(memoryStream);

        return assembly;
    }
}
