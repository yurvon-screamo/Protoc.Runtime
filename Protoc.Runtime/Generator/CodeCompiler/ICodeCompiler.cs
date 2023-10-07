using System.Reflection;

namespace Protoc.Runtime.Generator.CodeCompiler;

/// <summary>
/// Service for compiling C# code and generating an assembly.
/// </summary>
public interface ICodeCompiler
{
    /// <summary>
    /// Compiles the provided code and generates an assembly using the specified assembly tags.
    /// </summary>
    /// <param name="code">The C# code to compile.</param>
    /// <param name="assemblyTags">The types representing assembly tags.</param>
    /// <returns>The compiled assembly.</returns>
    Assembly CompileCode(IReadOnlyCollection<string> code, IReadOnlyCollection<Type> assemblyTags);

    /// <summary>
    /// Compiles the provided code and generates an assembly using an empty set of assembly tags.
    /// </summary>
    /// <param name="code">The C# code to compile.</param>
    /// <returns>The compiled assembly.</returns>
    Assembly CompileCode(IReadOnlyCollection<string> code);
}
