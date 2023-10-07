namespace Protoc.Runtime.Clients;

/// <summary>
/// Unary rpc description
/// </summary>
/// <param name="Rpc">Rpc name</param>
/// <param name="RequestType">Rpc request type</param>
/// <param name="ResponseType">Rpc response type</param>
public record UnaryCallRpcMeta(string Rpc, Type RequestType, Type ResponseType);
