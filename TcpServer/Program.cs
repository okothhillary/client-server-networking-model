// TcpServer/Program.cs
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

const int PORT = 5000;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Shutdown requested...");
    e.Cancel = true;
    cts.Cancel();
};

var listener = new TcpListener(IPAddress.Any, PORT);
listener.Start();
Console.WriteLine($"[Server] Listening on port {PORT}. Press Ctrl+C to stop.");

try
{
    while (!cts.IsCancellationRequested)
    {
        var tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
        // handle each client in a separate task
        _ = Task.Run(() => HandleClientAsync(tcpClient, cts.Token));
    }
}
catch (OperationCanceledException) { /* expected on shutdown */ }
finally
{
    listener.Stop();
    Console.WriteLine("[Server] Stopped.");
}

async Task HandleClientAsync(TcpClient client, CancellationToken token)
{
    var endpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
    Console.WriteLine($"[Server] Client connected: {endpoint}");

    try
    {
        using var ns = client.GetStream();
        using var reader = new StreamReader(ns, Encoding.UTF8);
        using var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

        // Welcome
        await writer.WriteLineAsync("OK Welcome. Type HELP for commands.");

        while (!token.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await reader.ReadLineAsync();
            }
            catch (IOException)
            {
                // client disconnected abruptly
                break;
            }

            if (line is null) break; // connection closed
            line = line.Trim();
            if (line.Length == 0) continue;

            Console.WriteLine($"[Server] <- ({endpoint}) {line}");
            var response = ProcessCommand(line);
            await writer.WriteLineAsync(response);
            Console.WriteLine($"[Server] -> ({endpoint}) {response}");

            if (line.Equals("QUIT", StringComparison.OrdinalIgnoreCase))
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Server] Error with client {endpoint}: {ex.Message}");
    }
    finally
    {
        client.Close();
        Console.WriteLine($"[Server] Client disconnected: {endpoint}");
    }
}

string ProcessCommand(string line)
{
    var parts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0].ToUpperInvariant();
    var arg = parts.Length > 1 ? parts[1] : string.Empty;

    try
    {
        switch (cmd)
        {
            case "HELP":
                return "OK Commands: HELP, TIME, ECHO <text>, UPPER <text>, REVERSE <text>, MATH <ADD|SUB|MUL|DIV> <a> <b>, QUIT";
            case "TIME":
                return "OK " + DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture); // ISO 8601 UTC
            case "ECHO":
                return "OK " + arg;
            case "UPPER":
                return "OK " + arg.ToUpperInvariant();
            case "REVERSE":
                {
                    var arr = arg.ToCharArray();
                    Array.Reverse(arr);
                    return "OK " + new string(arr);
                }
            case "MATH":
                {
                    // arg expected: "<OP> <a> <b>"
                    var mathTokens = arg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (mathTokens.Length != 3)
                        return "ERR Usage: MATH <ADD|SUB|MUL|DIV> <a> <b>";

                    var op = mathTokens[0].ToUpperInvariant();
                    if (!double.TryParse(mathTokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var a))
                        return $"ERR Invalid number: {mathTokens[1]}";
                    if (!double.TryParse(mathTokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var b))
                        return $"ERR Invalid number: {mathTokens[2]}";

                    return op switch
                    {
                        "ADD" => "OK " + (a + b).ToString(CultureInfo.InvariantCulture),
                        "SUB" => "OK " + (a - b).ToString(CultureInfo.InvariantCulture),
                        "MUL" => "OK " + (a * b).ToString(CultureInfo.InvariantCulture),
                        "DIV" => b == 0 ? "ERR Division by zero" : "OK " + (a / b).ToString(CultureInfo.InvariantCulture),
                        _ => "ERR Unknown operation. Use ADD, SUB, MUL, or DIV"
                    };
                }
            case "QUIT":
                return "OK Goodbye";
            default:
                return "ERR Unknown command. Type HELP.";
        }
    }
    catch (Exception ex)
    {
        return "ERR Server error: " + ex.Message;
    }
}
