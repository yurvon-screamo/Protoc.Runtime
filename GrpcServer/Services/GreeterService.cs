using Grpc.Core;
using Grpc.Net.Client;

using GrpcServer;

using Tester;

using static Tester.Greeter;

namespace GrpcServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}