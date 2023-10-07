using System.Reflection;
using Grpc.Core;

using Protoc.Runtime.Clients;

namespace Protoc.Runtime;

/// <summary>
/// Assembly grpc client detector
/// </summary>
public interface IProtocAssemblyParser
{
    /// <summary>
    /// Find all grpc services
    /// </summary>
    /// <param name="protocAssembly"></param>
    /// <returns></returns>
    IReadOnlyCollection<string> GetGrpcServices(Assembly protocAssembly);

    /// <summary>
    /// Find grpc client
    /// </summary>
    /// <param name="protocAssembly"></param>
    /// <param name="service">service name to search</param>
    /// <param name="channelBase">Channel to invoke</param>
    /// <returns></returns>
    IRuntimeGrpcClient? GetGrpcClient(Assembly protocAssembly, string service, ChannelBase channelBase);
}
