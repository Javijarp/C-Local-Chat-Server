using ServerClasses;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using FileClasses;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.ComponentModel;

namespace ServerClasses
{
        public class Server
    {
        // Server Attributes
        private readonly IPAddress _IpAddress;
        private readonly Int32 _port;
        private readonly TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        // Server Client Dictionaries
        private readonly ConcurrentDictionary<Guid, string> _clientIdToNameMap;
        private readonly ConcurrentDictionary<Guid, NetworkStream> clientIdToStreamMap;

        // Text File Writer
        private TextFileWriter _textFileWriter;
        private readonly string _logFolder = @"ChatApplication\Logs";
        private string _currentFileName;

        public Server(string _IpAddress, Int32 _port)
        {
            // Make the listener
            this._listener = new TcpListener(IPAddress.Parse(_IpAddress), _port);

            // Create Dictionaries
            this._clientIdToNameMap = new ConcurrentDictionary<Guid, string>();
            this.clientIdToStreamMap = new ConcurrentDictionary<Guid, NetworkStream>();

            // Create the current file name and text file
            this._currentFileName = $"[{GetCurrentDate().ToString("MM'-'dd'-'yyyy")}][{GetCurrentTime().ToString("HH'-'mm'-'ss")}] Message Logs.txt";
            this._textFileWriter = new TextFileWriter(this._logFolder, this._currentFileName);
            
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"Starting server at {GetCurrentDate()}:{GetCurrentTime()}...");

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    var clientId = Guid.NewGuid();
                    var clientIp = GetClientId(client);
                    var networkStream = client.GetStream();

                    this.clientIdToStreamMap[clientId] = networkStream;

                    var clientName = clientId.ToString();
                    this._clientIdToNameMap[clientId] = clientName;

                    _ = HandleClientWithErrorHandling(client, clientId, clientName, _cancellationTokenSource.Token);

                }
            } finally
            {
                _listener.Stop();
            }
        }

        private async Task HandleClientWithErrorHandling(TcpClient client, Guid clientId, string clientName, CancellationToken _cancellationToken)
        {
            try
            {
                await HandleClientAsync(client, clientId, clientName, _cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {clientId}: {ex.Message}");
            }
        }

        private async Task HandleClientDisconnectionWithErrorHandling(TcpClient client, Guid clientId, string clientName)
        {
            try
            {
                await HandleClientDisconnectionAsync(client, clientId, clientName);
                await SendMessageToClient(clientId, "You have been disconnected from the server.");
            } catch (Exception ex)
            {
                Console.WriteLine($"Error with client {clientId}: {ex.Message}");
            }
        }

        private async Task HandleClientDisconnectionAsync(TcpClient client, Guid clientId, string clientName)
        {

            if (client != null && client.Connected)
            {
                client.Close();
            }

            this._clientIdToNameMap.TryRemove(clientId, out _);
            this.clientIdToStreamMap.TryRemove(clientId, out _);
            string clientDisconnectionString = $"{clientName} disconnected";
            await BroadcastMessage(clientDisconnectionString, clientId);
            Console.WriteLine(clientDisconnectionString);
            this._textFileWriter.WriteMessage(new Message(clientDisconnectionString, GetCurrentDate(), GetCurrentTime(), "Server:"));
        }

        private async Task HandleClientAsync(TcpClient client, Guid clientId, string clientName, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Handling client {clientId} asynchronously.");

            try
            {
                var networkStream = this.clientIdToStreamMap[clientId];
                var reader = new StreamReader(networkStream);
                var writer = new StreamWriter(networkStream) { AutoFlush = true };

                while (!cancellationToken.IsCancellationRequested && client.Connected)
                {
                    var textMessage = await reader.ReadLineAsync();

                    if (string.Equals(textMessage, "/quit", StringComparison.OrdinalIgnoreCase))
                    {
                        await HandleClientDisconnectionWithErrorHandling(client, clientId, clientName);
                        break;
                    }
                    if (textMessage == null)
                    {
                        continue;
                    }
                    else if (textMessage.StartsWith("[NAME]"))
                    {
                        clientName = textMessage.Substring(6);
                    }
                    else
                    {
                        Message message = new Message(textMessage, GetCurrentDate(), GetCurrentTime(), clientName);
                        Console.WriteLine($"{message}");

                        this._textFileWriter.WriteMessage(message);
                        await writer.WriteLineAsync(message.ToString());
                        await BroadcastMessage(message.ToString(), clientId);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with client {clientId}: {ex.Message}");
            }
            finally
            {
                await HandleClientDisconnectionWithErrorHandling(client, clientId, clientName);
            }
        }

        private async Task SendMessageToClient(Guid clientId, string message)
        {
            if (clientIdToStreamMap.TryGetValue(clientId, out NetworkStream stream))
            {
                using var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
                await writer.WriteLineAsync(message);
            }
        }

        private async Task BroadcastMessage(string message, Guid senderId)
        {
            foreach (var (clientId, stream) in clientIdToStreamMap)
            {
                if (clientId == senderId) continue; // Skip the sender

                try
                {
                    if (stream.CanWrite)
                    {
                        using var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
                        await writer.WriteLineAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message to client {clientId}: {ex.Message}");
                    // Optionally handle the client disconnection here
                }
            }
        }

        public void Stop(){
            Console.WriteLine("Stopping server...");
            this.DisconnectAllClients();
            this._cancellationTokenSource.Cancel();
        }

        private void DisconnectAllClients(){
                // Disconnect all clients
            foreach (var clientId in clientIdToStreamMap.Keys)
            {
                // Attempt to gracefully close each client connection
                try
                {
                    if (clientIdToStreamMap.TryGetValue(clientId, out NetworkStream stream))
                    {
                        // Explicitly close the stream and client connection
                        stream.Close();
                        clientIdToStreamMap.TryRemove(clientId, out _);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error closing connection for client {clientId}: {ex.Message}");
                }
            }

            this._listener.Stop();
            Console.WriteLine("Server stopped.");
        }


        public IPAddress GetClientId(TcpClient _client) => ((IPEndPoint)_client.Client.RemoteEndPoint).Address;
        public DateOnly GetCurrentDate() => DateOnly.FromDateTime(DateTime.Now);
        public TimeOnly GetCurrentTime() => TimeOnly.FromDateTime(DateTime.Now);
    }
}