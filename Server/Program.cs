// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Greet;
using Server;

var server = new Grpc.Core.Server
{
    Services = { GreetingService.BindService(new GreetingServiceImpl()) },
    Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
};

server.Start();
Console.WriteLine("Server listening on port 50051");
Console.ReadLine();
await server.ShutdownAsync();



