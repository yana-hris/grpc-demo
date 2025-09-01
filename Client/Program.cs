// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Greet;


var channel = new Channel("localhost:50051", ChannelCredentials.Insecure);
await channel.ConnectAsync();

var client = new GreetingService.GreetingServiceClient(channel);
var greeting = new Greeting { FirstName = "John", LastName = "Doe" };
var request = new GreetingRequest { Greeting = greeting };

var response = client.Greet(request);

Console.WriteLine(response.Result);
Console.ReadKey();

await channel.ShutdownAsync();
