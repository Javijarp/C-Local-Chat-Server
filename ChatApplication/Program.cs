using System;
using ServerClasses;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ChatApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Do you want to host or join the server? (host/join)");
            string choice = Console.ReadLine().Trim().ToLower();

            const string ipAddress = "127.0.0.1";
            const int port = 6000;

            if (choice == "host")
            {
                await HostServer(ipAddress, port);
            }
            else if (choice == "join")
            {
                await JoinServer(ipAddress, port);
            }
            else
            {
                Console.WriteLine("Invalid choice. Exiting...");
            }
        }

        private static async Task HostServer(string ipAddress, int port)
        {
            Console.WriteLine("Starting the server...");
            var server = new Server(ipAddress, port);
            var serverTask = Task.Run(() => server.StartAsync());
            Console.WriteLine("Server started. Press any key to stop...");
            Console.ReadKey(true);

            server.Stop();
            Console.WriteLine("Server stopped.");
        }

        private static async Task JoinServer(string ipAddress, int port)
        {
            Console.WriteLine("Enter your client name:");
            string clientName = Console.ReadLine();
            var client = new Client(clientName);

            await client.ConnectAsync(ipAddress, port);
            Console.WriteLine("Connected to the server. You can start chatting now! ('/quit' to disconnect)");

            await Task.Run(() => client.ListenForMessages());

            bool onOff = true;
            while (onOff)
            {
                string messageToSend = ReadLineWithBackspace();

                if (messageToSend.ToLower().Trim() == "/quit"){
                    onOff = false;
                }
                await client.SendMessageAsync(messageToSend);

            }

            Console.WriteLine("Press any key to disconnect...");
            Console.ReadKey();

            client.Disconnect();
            Console.WriteLine("Disconnected from the server.");
        }

        private static string ReadLineWithBackspace()
        {
                string input = "";
                ConsoleKeyInfo key;
            
                do
                {
                    key = Console.ReadKey(true);
            
                    if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        // Move the cursor left, replace the character with space, move the cursor left again
                        Console.Write("\b \b");
                        input = input[..^1];
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        Console.Write(key.KeyChar);
                        input += key.KeyChar;
                    }
                } while (key.Key != ConsoleKey.Enter);
            
                // Clear the input line by moving the cursor to the beginning of the line and writing spaces
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, Console.CursorTop);
            
                return input;
        }
    }
}