# Overview:

My goal with this project was to deepen my understanding of network programming by building a client-server application using TCP sockets in C#. This project allowed me to explore socket programming, asynchronous communication, and command processing in a networked environment, to improve my skills in building reliable and interactive networked systems.

The software consists of two components: a **TCP Server** and a **TCP Client**. The server listens for incoming connections and processes commands from clients, while the client connects to the server to send commands and receive responses. The server supports commands like `HELP`, `TIME`, `ECHO`, `UPPER`, `REVERSE`, `MATH`, and `QUIT`, providing a simple yet extensible command-based protocol.

### How to Use the Software
1. **Starting the Server**:
   - Navigate to the `TcpServer` directory.
   - Run the server using: `dotnet run` or execute the compiled binary.
   - The server listens on port `5000` by default and will display a message indicating it is ready for connections.
   - Press `Ctrl+C` to gracefully shut down the server.

2. **Starting the Client**:
   - Navigate to the `TcpClientApp` directory.
   - Run the client using: `dotnet run [host] [port]` (e.g., `dotnet run 127.0.0.1 5000`).
   - If no arguments are provided, it defaults to `127.0.0.1:5000`.
   - For interactive mode, type commands and press Enter. Type `QUIT` to exit.
   - For non-interactive mode, pass a command as additional arguments (e.g., `dotnet run 127.0.0.1 5000 ECHO hello`), and the client will send the command and exit after receiving a response.

### Purpose
The purpose of this software was to create a practical implementation of a TCP-based client-server system to learn about socket programming, asynchronous I/O, and command parsing in C#. This project serves as a foundation for understanding networked applications, which are critical in distributed systems, and provides a extensible framework for adding more complex features in the future.

# Network Communication

### Architecture
The software uses a **client-server architecture**. The server runs continuously, accepting connections from multiple clients and handling each in a separate asynchronous task. The client initiates a connection to the server, sends commands, and receives responses.

### Protocol
- **Transport Protocol**: TCP (Transmission Control Protocol) is used to ensure reliable, ordered, and error-checked delivery of messages.
- **Port Number**: The server listens on port `5000` (configurable in the server code).
- **Message Format**:
  - Messages are text-based, encoded in UTF-8, and terminated with a newline (`\n`).
  - Client sends a command (e.g., `ECHO hello` or `MATH ADD 5 3`).
  - Server responds with a string starting with `OK` for success or `ERR` for errors, followed by the result or error message (e.g., `OK hello` or `ERR Unknown command`).
  - Commands are case-insensitive for the command keyword (e.g., `HELP`, `help`, or `HeLp` are equivalent).

# Development Environment

### Tools
- **IDE**: Visual Studio Code for code editing and debugging.
- **Build Tool**: .NET SDK (version 6.0 or later) for compiling and running the C# applications.
- **Version Control**: Git for source code management.
- **Terminal**: Used for running the server and client applications.

### Programming Language and Libraries
- **Language**: C# (C Sharp)
- **Libraries**:
  - `System.Net.Sockets`: For TCP socket programming (`TcpListener`, `TcpClient`).
  - `System.IO`: For stream handling (`StreamReader`, `StreamWriter`).
  - `System.Text`: For UTF-8 encoding.
  - `System.Threading.Tasks`: For asynchronous programming.
  - `System.Globalization`: For culture-invariant string and number parsing.

# Useful Websites

* [.NET Documentation - TcpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient)
* [.NET Documentation - TcpListener](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener)
* [C# Asynchronous Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/async)

# Future Work

* Add support for more complex commands, such as file transfer or user authentication.
* Implement client reconnection logic to handle temporary server downtime.
* Add logging to a file for server and client activities to aid debugging and monitoring.
