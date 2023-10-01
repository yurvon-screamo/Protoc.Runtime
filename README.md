# Protoc Runtime Library

The Protoc Runtime Library is a .NET library that facilitates the generation of C# code from Protocol Buffer files using protoc.

## Features

- Compiles Protocol Buffer files to C# code
- Supports generating code for gRPC

## Prerequisites

- .NET 7.0 SDK
- Protoc compiler
- gRPC Tools

## Getting Started

1. Clone this repository.

2. Build the solution using .NET CLI:
   ```
   dotnet build
   ```

3. Run the tests (optional):
   ```
   dotnet test
   ```

## Usage

Use the `ProtoGenerator` to generate C# code from Protocol Buffer files:

```csharp
using Protoc.Runtime;

// Instantiate ProtoGenerator with desired ICodeCompiler and ICodeGenerator implementations
ProtoGenerator protoGenerator = ProtoGenerator.CreateDefault();

// Specify the root directory and Protocol Buffer files
string rootDirectory = "Path/To/Your/Proto/Root";
string[] protoFiles = { "file1.proto", "file2.proto" }; // List of Protocol Buffer files

// Generate the assembly
Assembly generatedAssembly = await protoGenerator.Generate(rootDirectory, protoFiles);
```

## License

This project is licensed under the [Apache 2.0 License](LICENSE).

## TODO

* Calling gRPC Clients

* Implementing Services at Runtime

* Helper Sets for Parsing Generated Assembly

* Customization of Generation via Options
