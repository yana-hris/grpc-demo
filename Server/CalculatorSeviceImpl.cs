using Calculator;
using Grpc.Core;

using static Calculator.CalculatorService;

namespace Server
{
    public class CalculatorSeviceImpl : CalculatorServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Received: {request.ToString()}");  
            
            var response = new SumResponse { Result = request.FirstNumber + request.SecondNumber };

            Console.WriteLine($"Sending: {response.ToString()}");

            return Task.FromResult(response);
        }

        public override async Task PrimeNumberDecomposition(PrimeNumberDecompositionRequest request, 
                                                IServerStreamWriter<PrimeNumberDecompositionResponse> responseStream, 
                                                ServerCallContext context)
        {
            Console.WriteLine($"Received: {request.ToString()}");

            int number = request.Number;
            int k = 2;
            while (number > 1)
            {
                if (number % k == 0)
                {
                    var response = new PrimeNumberDecompositionResponse { PrimeFactor = k };
                    Console.WriteLine($"Sending: {response.ToString()}");
                    await responseStream.WriteAsync(response);
                    number /= k;
                }
                else
                {
                    k += 1;
                }
            }

        }

        public override async Task<ComputeAverageResponse> ComputeAverage(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            Console.WriteLine($"Received: {requestStream.Current.ToString()}");
            double sum = 0;
            int count = 0;
            while(await requestStream.MoveNext())
            {
                sum += requestStream.Current.Number;
                count++;
            }

            var response = new ComputeAverageResponse
            {
                Average = sum / count
            };

            Console.WriteLine($"Sending: {response.ToString()}");

            return response;
        }

        public override async Task FindMaximum(IAsyncStreamReader<FindMaximumRequest> requestStream, IServerStreamWriter<FindMaximumResponse> responseStream, ServerCallContext context)
        {
            var max =  int.MinValue;

            while(await requestStream.MoveNext())
            {
                Console.WriteLine($"Received: {requestStream.Current.ToString()}");
                int currNum = requestStream.Current.Number;
                if(currNum > max)
                {
                    max = currNum;
                    var response = new FindMaximumResponse
                    {
                        Max = max
                    };
                    Console.WriteLine($"Sending response: {response.ToString()}");
                    await responseStream.WriteAsync(response);
                }
            }
        }
    }
}
