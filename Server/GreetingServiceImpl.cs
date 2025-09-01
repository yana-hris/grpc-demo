using Greet;
using Grpc.Core;
using static Greet.GreetingService;


namespace Server
{
    public class GreetingServiceImpl : GreetingServiceBase
    {
        public override Task<GreetingResponse> Greet(GreetingRequest request, ServerCallContext context)
        {
            string result = String.Format("Hello, {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            return Task.FromResult(new GreetingResponse { Result = result });
        }
    }
}
