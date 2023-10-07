using Grpc.Core;
using Protoc.Runtime.Clients;
using System.Reflection;

namespace Protoc.Runtime;

/// <summary>
/// Assembly grpc client detector - impl
/// </summary>
public class ProtocAssemblyParser : IProtocAssemblyParser
{
    private const string Client = "Client";

    /// <summary>
    /// Find grpc client
    /// </summary>
    /// <param name="protocAssembly"></param>
    /// <param name="service">service name to search</param>
    /// <param name="channelBase">channel to invoke</param>
    /// <returns></returns>
    public IRuntimeGrpcClient? GetGrpcClient(Assembly protocAssembly, string service, ChannelBase channelBase)
    {
        string findValue = $"{service}{Client}";

        TypeInfo? typeInfo = GetClientTypes(protocAssembly)
            .FirstOrDefault(t => t.Name == findValue);

        if (typeInfo is null)
        {
            return null;
        }

        return new RuntimeGrpcClient(typeInfo, channelBase);
    }

    /// <summary>
    /// Find all grpc services
    /// </summary>
    /// <param name="protocAssembly"></param>
    /// <returns></returns>
    public IReadOnlyCollection<string> GetGrpcServices(Assembly protocAssembly)
    {
        return GetClientTypes(protocAssembly)
            .Select(t => t.Name[..^Client.Length])
            .ToArray();
    }

    private static IEnumerable<TypeInfo> GetClientTypes(Assembly protocAssembly)
    {
        return protocAssembly.DefinedTypes
            .Where(d => d.BaseType?.BaseType == typeof(ClientBase));
    }


}
