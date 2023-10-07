using System.Reflection;

namespace Protoc.Runtime.Generator;

/// <summary>
/// Service responsible for generating assemblies from Protocol Buffer files.
/// </summary>
public interface IProtoGenerator
{
    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="directories">The directories containing the Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> GenerateAll(string root, IEnumerable<string> directories);

    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> Generate(string root, params string[] files);

    /// <summary>
    /// Generates an assembly containing code generated from Protocol Buffer files.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The Protocol Buffer files.</param>
    /// <returns>An assembly containing the generated code.</returns>
    Task<Assembly> Generate(string root, IEnumerable<string> files);
}
