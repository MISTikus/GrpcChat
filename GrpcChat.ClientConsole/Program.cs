using Grpc.Core;
using GrpcChat.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcChat.ClientConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome!");
            Console.WriteLine("Tell me your name:");
            var name = Console.ReadLine();
            Console.WriteLine("Tell me your password:");
            var password = Console.ReadLine();
            string authToken = null;

            var cancel = new CancellationTokenSource();

            void log(Models.Message msg)
            {
                Console.WriteLine($"{msg.Timestamp.ToDateTime().ToString("[yyyy.mm.dd hh:MM:ss]")}: [{msg.UserName}]: {msg.Text}");
            }

            var channel = new Channel("localhost:50001", ChannelCredentials.Insecure);

            var authClient = new Clients.Authentication(channel, log, cancel.Token);

            var (registeredUserName, token) = authClient.GetAuthnticationToken(name, password).Result;
            if (token == null)
            {
                Console.WriteLine("Failed to authenticate... Press any key to exit...");
                Console.ReadKey();
                return;
            }

            name = registeredUserName;
            authToken = token;

            var roomClient = new Clients.Room(channel, authToken, log, cancel.Token);

            roomClient.GetHistory();
            roomClient.Listen();

            Task.Run(async () =>
            {
                var line = Console.ReadLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                while (!string.IsNullOrWhiteSpace(line))
                {
                    await roomClient.Publish(name, line);
                    line = Console.ReadLine();
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
                cancel.Cancel();
            }, cancel.Token).Wait();

            channel.ShutdownAsync().Wait();
        }
    }
}
