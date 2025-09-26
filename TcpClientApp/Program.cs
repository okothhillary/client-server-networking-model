// TcpClientApp/Program.cs
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

string host = args.Length > 0 ? args[0] : "127.0.0.1";
int port = args.Length > 1 && int.TryParse(args[1], out var p) ? p : 5000;

try
{
    using var client = new TcpClient();
    Console.WriteLine($"[Client] Connecting to {host}:{port}...");
    await client.ConnectAsync(host, port);
    Console.WriteLine("[Client] Connected. Type commands, or QUIT to exit.");

    using var ns = client.GetStream();
    using var reader = new StreamReader(ns, Encoding.UTF8);
    using var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

    // Read welcome
    var welcome = await reader.ReadLineAsync();
    if (welcome != null) Console.WriteLine($"<- {welcome}");

    // If there are extra args after host and port, send them as a single command and exit.
    if (args.Length > 2)
    {
        var cmd = string.Join(' ', args, 2, args.Length - 2);
        Console.WriteLine($"> {cmd}");
        await writer.WriteLineAsync(cmd);
        var resp = await reader.ReadLineAsync();
        Console.WriteLine($"<- {resp}");
        return;
    }

    while (true)
    {
        Console.Write("> ");
        var line = Console.ReadLine();
        if (line == null) break;

        await writer.WriteLineAsync(line);
        var response = await reader.ReadLineAsync();
        if (response == null)
        {
            Console.WriteLine("[Client] Server closed the connection.");
            break;
        }
        Console.WriteLine($"<- {response}");

        if (line.Equals("QUIT", StringComparison.OrdinalIgnoreCase))
            break;
    }
}
catch (SocketException sx)
{
    Console.WriteLine($"[Client] Socket error: {sx.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Client] Error: {ex.Message}");
}
