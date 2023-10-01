using Protoc.Runtime;
using Protoc.Runtime.CodeCompiler;
using Protoc.Runtime.CodeGenerator;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

//cd proto && \
//	protoc --plugin=protoc-gen-grpc=~/.nuget/packages/grpc.tools/2.58.0/tools/windows_x64/grpc_csharp_plugin.exe \
//	--csharp_out=.. \
//	--grpc_out=./.. \
//	dispatcher.proto cacher/api.proto cacher/legacy/action_service.proto \
//	cacher/grpc/device_network_service.proto cacher/models/device_network.proto \
//	cacher/grpc/hypervisor_service.proto cacher/models/hypervisor.proto \
//	cacher/grpc/hypervisor_state_service.proto cacher/models/hypervisor_state.proto

string root = "C:\\Users\\YVTURBIN\\samples\\runtime-protoc\\runtime-protoc\\proto";
string[] files = new string[] { "C:\\Users\\YVTURBIN\\samples\\runtime-protoc\\runtime-protoc\\proto\\cat.proto" };

IProtoGenerator protoGenerator = ProtoGenerator.CreateDefault();

Assembly assembly = await protoGenerator.Generate(root, files);

Console.WriteLine(assembly);
