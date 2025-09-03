# gRPC Demo Project (.NET 9)

This is a sample project built with **.NET 9** and **gRPC**, showcasing the four main types of gRPC communication:

1. **Unary RPC** – the client sends a single request and receives a single response.  
2. **Server Streaming RPC** – the client sends a single request and receives a stream of responses.  
3. **Client Streaming RPC** – the client sends a stream of requests and receives a single response.  
4. **Bi-directional Streaming RPC** – the client and server both send streams of messages asynchronously.

---

## 📂 Project Structure

grpc-demo/
│── Client/ # Console client
│ ├── Protos/ # Shared .proto files
│ └── Program.cs # Client entry point
│
│── Server/ # gRPC server
│ ├── Protos/ # Shared .proto files
│ ├── CalculatorServiceImpl.cs
│ ├── GreetingServiceImpl.cs
│ └── Program.cs # Server entry point
│
└── .gitignore

---

## 🚀 Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/yana-hris/grpc-demo.git
   cd grpc-demo
   ```

2. Run the server:
```bash
cd Server
dotnet run
```


3. Run the client (in a new terminal window):
```bash
cd Client
dotnet run
```
🧪 Demonstrations

Unary RPC (Sum)
Input two numbers in the client → server returns their sum.

Server Streaming RPC (Prime Decomposition)
Input a number → server streams back its prime factors.

Client Streaming RPC (Compute Average)
Input multiple numbers → server responds with their average.

Bi-directional Streaming RPC (GreetEveryone / FindMaximum)
Client and server exchange greetings or numbers dynamically until the stream is closed.

⚙️ Technologies

.NET 9

gRPC

Grpc.Tools

Google.Protobuf

📖 Useful Resources

gRPC for .NET Documentation

Introduction to gRPC

📝 License

This project is open-source and available under the MIT License.
