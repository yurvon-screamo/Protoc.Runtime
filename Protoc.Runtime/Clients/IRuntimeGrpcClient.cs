using Google.Protobuf;

using Grpc.Core;

namespace Protoc.Runtime.Clients;

/// <summary>
/// Grpc service client
/// </summary>
public interface IRuntimeGrpcClient
{
    /// <summary>
    /// Get service name for this client
    /// </summary>
    /// <returns></returns>
    string GetServiceName();

    /// <summary>
    /// Get all service rpc (unary call only)
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<string> GetUnaryRpcs();

    /// <summary>
    /// Get unary rpc description
    /// </summary>
    /// <param name="rpc">Rcp name</param>
    /// <returns></returns>
    UnaryCallRpcMeta? GetUnaryCallRpcMeta(string rpc);

    /// <summary>
    /// Invoke rpc async (unary only)
    /// </summary>
    /// <param name="rpc">Rpc name</param>
    /// <param name="request">Request to invoke</param>
    /// <param name="callOptions">Options to invoke</param>
    /// <returns></returns>
    Task<IMessage> UnaryCallAsync(string rpc, IMessage request, CallOptions callOptions = default);

    /// <summary>
    /// Invoke rpc from json async (unary only)
    /// </summary>
    /// <param name="rpc">Rpc name</param>
    /// <param name="request">Request to invoke</param>
    /// <param name="callOptions">Options to invoke</param>
    /// <returns></returns>
    Task<string> UnaryCallAsJsonAsync(string rpc, string request, CallOptions callOptions = default);
}
