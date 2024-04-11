using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerClasses
{
    public class Client
    {
        private readonly TcpClient _client;
        private IPAddress _serverAddress;
        private Int32 _serverPort;
        public string _clientName { get; private set; }

        public Client(string clientName)
        {
            this._clientName = clientName;
            this._client = new TcpClient();
        }

        public async Task ConnectAsync(string serverAddress, Int32 serverPort)
        {
            try
            {
                this._serverAddress = IPAddress.Parse(serverAddress);
                this._serverPort = serverPort;
                await _client.ConnectAsync(this._serverAddress, this._serverPort);
                await SendMessageAsync($"[NAME]{this._clientName}");
                ListenForMessages();
                Console.WriteLine($"{this._clientName} connected to the server at {serverAddress}:{serverPort}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
            }
        }

        public async void ListenForMessages()
        {
            try
            {
                var stream = this._client.GetStream();
                var reader = new StreamReader(stream);

                while (this._client.Connected)
                {
                    var response = await reader.ReadLineAsync();
                    if (response != null)
                    {
                        Console.WriteLine(response);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Connection was closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"You are disconnected from the server.");
            }
        }

        public async Task SendMessageAsync(string _message)
        {
            if (!this._client.Connected)
            {
                Console.WriteLine("Client is not connected to server.");
                return;
            }

            try
            {
                var stream = this._client.GetStream();
                var writer = new StreamWriter(stream, leaveOpen: true) { AutoFlush = true };
                await writer.WriteLineAsync(_message);
                // Console.WriteLine($"Me sent: {_message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async Task<string> RecieveMessageAsync()
        {
            if (!this._client.Connected)
            {
                Console.WriteLine("Client is not connected to server.");
                return string.Empty;
            }

            try
            {
                var stream = this._client.GetStream();
                var reader = new StreamReader(stream);
                var response = await reader.ReadLineAsync();
                Console.WriteLine($"{this._clientName} recieved: {response}");
                return response;
            }
            catch (Exception ex )
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                return string.Empty;
            }
        }

        public void Disconnect()
        {
            if (this._client.Connected)
            {
                this._client.Close();
                Console.WriteLine($"{this._clientName} has disconnected.");
            }
        }
    }
}
