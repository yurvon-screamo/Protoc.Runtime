using Protoc.Runtime;

using System.Reflection;

string root = Path.GetFullPath("../../../proto");
string[] files = new string[] { root + "/cat.proto" };

IProtoGenerator protoGenerator = ProtoGenerator.CreateDefault();

Assembly assembly = await protoGenerator.Generate(root, files);

Console.WriteLine(assembly);
