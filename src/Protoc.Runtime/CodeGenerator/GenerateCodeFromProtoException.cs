namespace Protoc.Runtime.CodeGenerator;

/// <summary>
/// Exception thrown when there is an error generating code from Protocol Buffer files.
/// </summary>
public class GenerateCodeFromProtoException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateCodeFromProtoException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the exception.</param>

    internal GenerateCodeFromProtoException(string message) : base(message) { }
}
