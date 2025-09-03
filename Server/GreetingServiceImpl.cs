using Greet;
using Grpc.Core;
using Grpc.Core.Utils;
using static Greet.GreetingService;


namespace Server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received: {request.ToString()}");

            string result = String.Format("Hello, {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            var response = new GreetingResponse { Result = result };

            Console.WriteLine($"Sending: {response.ToString()}");

            return Task.FromResult(response);
        }

        public override async Task GreetManyTimes(GreetManyTimesRequest request, IServerStreamWriter<GreetManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"Received: {request.ToString()}");

            string result = String.Format("Hello, {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);
            var random = new Random();
            int responseCount = random.Next(2, 10);

            var response = new GreetManyTimesResponse { Result = result };
            Console.WriteLine($"Total greets will be: {responseCount}");

            foreach (int i in Enumerable.Range(1, responseCount))
            {
                int delayMs = random.Next(200, 3000);
                Console.WriteLine($"Sending: {response.ToString()}, next one will be in {delayMs}ms");
                await responseStream.WriteAsync(response);

                
                await Task.Delay(delayMs);
            }
            
        }

        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            string result = "";

            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Received: {requestStream.Current.ToString()}");
                result += String.Format($"Hello {requestStream.Current.Name}{Environment.NewLine}");
            }

            var response = new LongGreetResponse { Result = result };
            Console.WriteLine($"Sending: {response.ToString()}");
            return response;
        }

        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, 
                                                    IServerStreamWriter<GreetEveryoneResponse> responseStream, 
                                                    ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                Console.WriteLine($"Received: {requestStream.Current.ToString()}");

                var result = String.Format($"Hello {requestStream.Current.Greeting.FirstName} {requestStream.Current.Greeting.LastName}");

                var response = new GreetEveryoneResponse
                {
                    Result = result
                };

                Console.WriteLine($"Sending: {response}");

                await responseStream.WriteAsync(response);
            }
        }
    }
}
