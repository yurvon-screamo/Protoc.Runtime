using Grpc.Core;
using Grpc.Net.Client;

using Tester;

using static Tester.Greeter;

namespace SampleServer.Services;

public class GreeterService : GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}