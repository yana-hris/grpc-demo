using Grpc.Core;
using Greet;
using Calculator;

using static Greet.GreetingService;
using static Calculator.CalculatorService;

// Setup channel & clients 
var channel = new Channel("localhost:50051", ChannelCredentials.Insecure);
await channel.ConnectAsync();
Console.WriteLine("Client connected successfully");

var greetClient = new GreetingServiceClient(channel);
var calcClient = new CalculatorServiceClient(channel);

var exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("*** gRPC Demo Menu ***");
    Console.WriteLine("1) Unary (Sum)");
    Console.WriteLine("2) Unary (Greet)");
    Console.WriteLine("3) Server streaming (PrimeNumberDecomposition)");
    Console.WriteLine("4) Server streaming (GreetManyTimes)");
    Console.WriteLine("5) Client streaming (ComputeAverage)");
    Console.WriteLine("6) Client streaming (LongGreet)");
    Console.WriteLine("7) Bi-directional streaming (FindMaximumNumber)");
    Console.WriteLine("8) Bi-directional streaming (GreetEveryone)");
    Console.WriteLine("q) Quit");
    Console.Write("Choice: ");

    var choice = Console.ReadKey();
    Console.WriteLine();

    try
    {
        switch (choice.KeyChar)
        {
            case '1': await RunUnaryCalculatorServiceAsync(calcClient); break;
            case '2': await RunUnaryGreetingServiceAsync(greetClient); break;
            case '3': await RunServerStreamingCalculatorServiceAsync(calcClient); break;
            case '4': await RunServerStreamingGreetingServiceAsync(greetClient); break;
            case '5': await RunClientStreamingCalculatorServiceAsync(calcClient); break;
            case '6': await RunClientStreamingGreetingServiceAsync(greetClient); break;
            case '7': await RunBidiStreamingCalculatorServiceAsync(calcClient); break;
            case '8': await RunBidiStreamingGreetingServiceAsync(greetClient); break;
            case 'Q': exit = true; break;
            default: Console.WriteLine("Unknown option."); break;
        }
    }
    catch (RpcException ex)
    {
        Console.WriteLine($"gRPC error: {ex.StatusCode} - {ex.Status.Detail}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

await channel.ShutdownAsync();
Console.WriteLine("Bye!");


// Helpers 
// Unary call
static async Task RunUnaryCalculatorServiceAsync(CalculatorServiceClient client)
{
    Console.WriteLine("Enter two integers separated by space (or 'q' to quit):");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        var parts = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts == null || parts.Length != 2 ||
            !int.TryParse(parts[0], out int firstNum) ||
            !int.TryParse(parts[1], out int secondNum))
        {
            Console.WriteLine("❌ Please enter exactly two integers, e.g. `3 10` (or 'q' to quit).");
            continue;
        }

        var req = new SumRequest { FirstNumber = firstNum, SecondNumber = secondNum };
        var res = await client.SumAsync(req);

        Console.WriteLine($"Result: {firstNum} + {secondNum} = {res.Result}");
        Console.WriteLine();
        Console.WriteLine("Enter two more integers (or 'q' to quit):");
    }
}

static async Task RunUnaryGreetingServiceAsync(GreetingServiceClient client)
{
    Console.WriteLine("Enter first and last name, separated by space (or 'q' to quit):");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        var parts = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts == null || parts.Length != 2)
        {
            Console.WriteLine("❌ Please enter exactly two names, e.g. `John Doe` (or 'q' to quit).");
            continue;
        }

        Greeting result = new Greeting { FirstName = parts[0], LastName = parts[1] };
        var req = new GreetingRequest { Greeting = result};
        var res = client.Greet(req);

        Console.WriteLine($"Result: {res.Result}");
        Console.WriteLine();
        Console.WriteLine("Enter two more names (or 'q' to quit):");
    }
}

// Server streaming
static async Task RunServerStreamingCalculatorServiceAsync(CalculatorServiceClient client)
{
    Console.WriteLine("Enter a number to decompose into primes (or 'q' to quit):");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        if (!int.TryParse(input, out int number) || number <= 1)
        {
            Console.WriteLine("❌ Please enter an integer greater than 1 (or 'q' to quit).");
            continue;
        }

        var req = new PrimeNumberDecompositionRequest { Number = number };
        using var call = client.PrimeNumberDecomposition(req);

        Console.WriteLine($"Prime factors of {number}:");
        while (await call.ResponseStream.MoveNext())
        {
            Console.WriteLine($"  {call.ResponseStream.Current.PrimeFactor}");
        }

        Console.WriteLine();
        Console.WriteLine("Enter another number (or 'q' to quit):");
    }
}

static async Task RunServerStreamingGreetingServiceAsync(GreetingServiceClient client)
{
    Console.WriteLine("Enter first and last name, separated by space (or 'q' to quit):");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        var parts = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts == null || parts.Length != 2)
        {
            Console.WriteLine("❌ Please enter exactly two names, e.g. `John Doe` (or 'q' to quit).");
            continue;
        }

        Greeting result = new Greeting { FirstName = parts[0], LastName = parts[1] };
        var req = new GreetManyTimesRequest { Greeting = result };
        using var call = client.GreetManyTimes(req);

        Console.WriteLine($"Greetings from server for {req.Greeting.ToString()}:");
        while (await call.ResponseStream.MoveNext())
        {
            Console.WriteLine($"  {call.ResponseStream.Current.Result}");
        }

        Console.WriteLine();
        Console.WriteLine("Enter another name (or 'q' to quit):");
    }
}

// Client streaming
static async Task RunClientStreamingCalculatorServiceAsync(CalculatorServiceClient client)
{
    using var call = client.ComputeAverage();

    Console.WriteLine("Enter integers (one per line). Type 'q' to finish and compute the average:");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        if (int.TryParse(input, out var n))
        {
            await call.RequestStream.WriteAsync(new ComputeAverageRequest { Number = n });
        }
        else
        {
            Console.WriteLine("Please enter a valid integer or 'q' to finish.");
        }
    }

    // tell the server we're done sending
    await call.RequestStream.CompleteAsync();

    // wait for the server's single response
    var res = await call.ResponseAsync;
    Console.WriteLine($"Average: {res.Average}");
}

static async Task RunClientStreamingGreetingServiceAsync(GreetingServiceClient client)
{
    using var call = client.LongGreet();

    Console.WriteLine("Enter names of your friends to greet, one per line. Type 'q' to finish.");

    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;
        
        await call.RequestStream.WriteAsync(new LongGreetRequest { Name = input });
        
    }

    // tell the server we're done sending
    await call.RequestStream.CompleteAsync();

    // wait for the server's single response
    var res = await call.ResponseAsync;
    Console.WriteLine($"Response: {res.Result}");
}

// Bi-directional streaming
static async Task RunBidiStreamingCalculatorServiceAsync(CalculatorServiceClient client)
{
    using var call = client.FindMaximum();

    // Reader task
    var reader = Task.Run(async () =>
    {
        while (await call.ResponseStream.MoveNext())
        {
            Console.WriteLine($"Current Max: {call.ResponseStream.Current.Max}");
        }
    });

    Console.WriteLine("Start entering numbers and await the current max from the server (type 'q' to quit):");

    // Writer loop
    while(true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) break;

        if(int.TryParse(input, out int number))
        {
            var request = new FindMaximumRequest { Number = number };
            Console.WriteLine($"Sending: {request.ToString()}");
            await call.RequestStream.WriteAsync(request);
        }
        else
        {
            Console.WriteLine("Please enter a valid integer number or 'q' to quit.");
        }
    }

    await call.RequestStream.CompleteAsync();
    await reader;
}

static async Task RunBidiStreamingGreetingServiceAsync(GreetingServiceClient client)
{
    using var call = client.GreetEveryone();

    // Reader task
    var reader = Task.Run(async () =>
    {
        while (await call.ResponseStream.MoveNext())
        {
            Console.WriteLine($"Received: {call.ResponseStream.Current.Result}");
        }
    });

    Console.WriteLine("Enter first and last name (or 'q' to quit):");

    // Writer loop (console input)
    while (true)
    {
        var input = Console.ReadLine();
        if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
            break;

        var parts = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts == null || parts.Length < 2)
        {
            Console.WriteLine("Please enter both first and last name, e.g. `John Doe` (or 'q' to quit).");
            continue;
        }

        var greeting = new Greeting { FirstName = parts[0], LastName = parts[1] };
        Console.WriteLine($"Sending: {greeting.FirstName} {greeting.LastName}");

        await call.RequestStream.WriteAsync(new GreetEveryoneRequest { Greeting = greeting });
    }

    // Signal end of sending, then wait for server to finish
    await call.RequestStream.CompleteAsync();
    await reader;
}

