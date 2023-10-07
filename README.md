# Protoc Runtime Library

The Protoc Runtime Library is a .NET library, which allows you to generate C# code from Protocol Buffer files at runtime.

## Features

- Compiles Protocol Buffer files to C# code
- Supports generating code for gRPC
- Call grpc clients created in runtime

## Getting Started

1. Clone this repository.

2. Build the solution using .NET CLI:

   ```csharp
   dotnet build *.sln
   ```

3. Run SampleServer for test:

   ```csharp
   cd SampleServer && dotnet run
   ```

4. Run Sample project with examples 

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

ProtocAssemblyParser protocAssemblyParser = new();

IReadOnlyCollection<string> services = protocAssemblyParser.GetGrpcServices(generatedAssembly);

IRuntimeGrpcClient client = protocAssemblyParser.GetGrpcClient(
    generatedAssembly,
    "Greeter",
    GrpcChannel.ForAddress("http://localhost:5251"))!;

UnaryCallRpcMeta unaryCallRpcMeta  = client.GetUnaryCallRpcMeta("SayHello")!;
Console.WriteLine(unaryCallRpcMeta);

IReadOnlyCollection<string> unaryRpcs = client.GetUnaryRpcs();
Console.WriteLine(string.Join(", ", unaryRpcs));

string response = await client.UnaryCallAsJsonAsync("SayHello",
    "{\"name\": \"123\"}");

Console.WriteLine(response);
```

## License

This project is licensed under the [Apache 2.0 License](LICENSE).

## TODO

- Normal async call gRPC Clients

- gRPC Clients for streaming

- Implementing and Map Server side at Runtime

- Helper Sets for Parsing Generated Assembly

- Customization of Generation via Options

- Create unit, component test

- Create github page
