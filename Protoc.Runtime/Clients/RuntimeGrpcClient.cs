using Google.Protobuf;

using Grpc.Core;

using Newtonsoft.Json;

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Protoc.Runtime.Clients;

/// <summary>
/// Grpc service client - impl
/// </summary>
internal class RuntimeGrpcClient : IRuntimeGrpcClient
{
    private const string Client = "Client";

    private readonly TypeInfo _clientType;
    private readonly ClientBase _client;

    public RuntimeGrpcClient(TypeInfo clientType, ChannelBase channelBase)
    {
        _clientType = clientType;
        _client = (ClientBase?)Activator.CreateInstance(_clientType, channelBase)!;
    }

    string IRuntimeGrpcClient.GetServiceName()
    {
        return _clientType.Name[..^Client.Length];
    }

    UnaryCallRpcMeta? IRuntimeGrpcClient.GetUnaryCallRpcMeta(string rpc)
    {
        MethodInfo? method = _clientType.DeclaredMethods
            .SingleOrDefault(m => UnaryRpcsPredicate(m) && m.Name == rpc);

        if (method is null)
        {
            return null;
        }

        return new(
            rpc, 
            method.GetParameters()[0].ParameterType, 
            method.ReturnParameter.ParameterType);
    }

    IReadOnlyCollection<string> IRuntimeGrpcClient.GetUnaryRpcs()
    {
        return _clientType.DeclaredMethods
            .Where(UnaryRpcsPredicate)
            .Select(c => c.Name)
            .ToArray();
    }

    async Task<string> IRuntimeGrpcClient.UnaryCallAsJsonAsync(string rpc, string request, CallOptions callOptions)
    {
        Type requestType = _clientType.DeclaredMethods
          .Single(r => UnaryRpcsPredicate(r) && r.Name == rpc)
          .GetParameters()[0]
          .ParameterType;

        IMessage requestObj = (IMessage?)JsonConvert.DeserializeObject(request, requestType)!;

        IMessage response = await UnaryCallImplement(rpc, requestObj, callOptions);

        return JsonConvert.SerializeObject(response);
    }

    Task<IMessage> IRuntimeGrpcClient.UnaryCallAsync(string rpc, IMessage request, CallOptions callOptions)
    {
        return UnaryCallImplement(rpc, request, callOptions);
    }

    private Task<IMessage> UnaryCallImplement(string rpc, object request, CallOptions callOptions)
    {
        MethodInfo method = _clientType.DeclaredMethods
            .Single(r => UnaryRpcsPredicate(r, true) && r.Name == rpc + "Async");

        object task = method.Invoke(
            _client,
            new object?[] { request, callOptions })!;

        object awaiter = task.GetType()
            .GetMethod(nameof(Task.GetAwaiter))!
            .Invoke(task, Array.Empty<object>())!;

        IMessage result = (IMessage?)awaiter.GetType()
            .GetMethod(nameof(TaskAwaiter.GetResult))!
            .Invoke(awaiter, Array.Empty<object>())!;

        return Task.FromResult(result);
    }

    private static bool UnaryRpcsPredicate(MethodInfo d) => UnaryRpcsPredicate(d, false);
    private static bool UnaryRpcsPredicate(MethodInfo d, bool asyncMode)
    {
        ParameterInfo[] parameters = d.GetParameters();

        return parameters.Length > 1 &&
            parameters[1].ParameterType == typeof(CallOptions) &&
            ((asyncMode && d.ReturnType.Namespace == "Grpc.Core") || (!asyncMode && d.ReturnType.Namespace != "Grpc.Core"));
    }
}
