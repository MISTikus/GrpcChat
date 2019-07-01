using Grpc.Core;
using GrpcChat.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GrpcChat.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const int port = 50001;
            var cancel = new CancellationTokenSource();
            void log(Models.Message msg)
            {
                System.Console.WriteLine($"{msg.Timestamp.ToDateTime().ToString("[yyyy.mm.dd hh:MM:ss]")}: [{msg.UserName}]: {msg.Text}");
            }

            var authCache = new Dictionary<string, (User user, DateTime tokenExpiration)>();
            var users = new List<User>
            {
                new User{ Name = "Admin", PasswordHash = "admin".GetHashCode() },
                new User{ Name = "User", PasswordHash = "user".GetHashCode() },
                new User{ Name = "OtherUser", PasswordHash = "user".GetHashCode() },
            };

            var server = new Server
            {
                Services =
                {
                    Service.Authentication.BindService(new Services.Authentication(authCache, users)),
                    Service.Room.BindService(new Services.Room(authCache, log, cancel.Token))
                },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };
            server.Start();

            System.Console.WriteLine("Greeter server listening on port " + port);
            System.Console.WriteLine("Press any key to stop the server...");
            System.Console.ReadKey();
            cancel.Cancel();

            server.ShutdownAsync().Wait();
        }
    }
}
