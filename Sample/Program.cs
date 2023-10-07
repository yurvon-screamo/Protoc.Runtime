using Grpc.Net.Client;

using Protoc.Runtime;
using Protoc.Runtime.Clients;
using Protoc.Runtime.Generator;
using System.Reflection;

string root = Path.GetFullPath("../../../proto");
string[] files = new string[] { root + "/cat.proto" };

IProtoGenerator protoGenerator = ProtoGenerator.CreateDefault();

Assembly assembly = await protoGenerator.Generate(root, files);

ProtocAssemblyParser protocAssemblyParser = new();

IReadOnlyCollection<string> services = protocAssemblyParser.GetGrpcServices(assembly);

IRuntimeGrpcClient client = protocAssemblyParser.GetGrpcClient(
    assembly,
    "Greeter",
    GrpcChannel.ForAddress("http://localhost:5251"))!;

UnaryCallRpcMeta unaryCallRpcMeta  = client.GetUnaryCallRpcMeta("SayHello")!;
Console.WriteLine(unaryCallRpcMeta);

IReadOnlyCollection<string> unaryRpcs = client.GetUnaryRpcs();
Console.WriteLine(string.Join(", ", unaryRpcs));

string response = await client.UnaryCallAsJsonAsync("SayHello",
    "{\"name\": \"123\"}");

Console.WriteLine(response);
