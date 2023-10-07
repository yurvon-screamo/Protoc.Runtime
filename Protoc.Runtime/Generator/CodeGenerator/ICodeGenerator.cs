namespace Protoc.Runtime.Generator.CodeGenerator;

/// <summary>
/// Service for generating C# code from Protocol Buffer files using protoc.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Generates C# code from Protocol Buffer files asynchronously.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The list of Protocol Buffer files to generate code from.</param>
    /// <returns>A collection of generated C# code as strings.</returns>
    Task<IReadOnlyCollection<string>> GenerateCodeFromProto(string root, params string[] files);

    /// <summary>
    /// Generates C# code from Protocol Buffer files asynchronously.
    /// </summary>
    /// <param name="root">The root directory for the Protocol Buffer files.</param>
    /// <param name="files">The collection of Protocol Buffer files to generate code from.</param>
    /// <returns>A collection of generated C# code as strings.</returns>
    Task<IReadOnlyCollection<string>> GenerateCodeFromProto(string root, IEnumerable<string> files);
}
